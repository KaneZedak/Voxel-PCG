using UnityEngine;

public class VoxelCreator : MonoBehaviour
{
    public int voxelSize = 100;
    public int[,,] voxel {get {return _voxel;} set {_voxel = voxel;}}
    private int[,,] _voxel;
    
    public ProceduralModifier[] pcgSteps;
    public Vector3[] roomLocations; // Add this to store room locations

    /*void Awake() {
        _voxel = new int[voxelSize,voxelSize,voxelSize];
        for(int i = 0; i < voxelSize; i++) {
            for(int j = 0; j < voxelSize; j++) {
                for(int k = 0; k < voxelSize; k++) {
                    _voxel[i,j,k] = 0;
                }
            }
        }
        for(int i = 0; i < pcgSteps.Length;i++) {
            if(pcgSteps[i] != null) {
                pcgSteps[i] = Instantiate(pcgSteps[i]);
                pcgSteps[i].initialize(this);
            }
        }

        for(int i = 0; i < pcgSteps.Length; i++) {
            pcgSteps[i].Execute();
        }
    }*/

    public void Generate()
    {
        Debug.Log($"Voxel grid initialized with size: {voxelSize}");
        _voxel = new int[voxelSize, voxelSize, voxelSize];
        for (int i = 0; i < voxelSize; i++)
        {
            for (int j = 0; j < voxelSize; j++)
            {
                for (int k = 0; k < voxelSize; k++)
                {
                    _voxel[i, j, k] = 0;
                }
            }
        }

        if (pcgSteps == null || pcgSteps.Length == 0)
        {
            Debug.LogError("pcgSteps is not initialized or empty. Ensure it is properly set before calling Generate.");
            return;
        }

        for (int i = 0; i < pcgSteps.Length-1; i++)
        {
            if (pcgSteps[i] != null)
            {
                pcgSteps[i] = Instantiate(pcgSteps[i]); // Ensure this line is within bounds
                pcgSteps[i].initialize(this);
            }
            else
            {
                Debug.LogWarning($"pcgSteps[{i}] is null. Skipping this step.");
            }
        }

        for (int i = 0; i < pcgSteps.Length; i++)
        {
            if (pcgSteps[i] != null)
            {
                pcgSteps[i].Execute();
            }
        }
    }

    //return the block type at given location, return empty block if invalid
    public int getBlockAt(int x, int y, int z) {
        if(inBound(x, y, z)) {
            return _voxel[x,y,z];
        }

        return -1;
    }

    //set block to a block type at certain location
    public void setBlock(int x, int y, int z, int id) {
        if(inBound(x, y, z)) {
            _voxel[x,y,z] = id;
        }
    }

    public bool inBound(int x, int y, int z) {
        return x >= 0 && y >= 0 && z >= 0 && x < voxelSize && y < voxelSize && z < voxelSize;
    }

}
