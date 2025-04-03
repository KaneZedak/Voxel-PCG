using UnityEngine;

public class VoxelCreator : MonoBehaviour
{
    public int voxelSize = 10;
    public int[,,] voxel {get {return _voxel;} set {_voxel = voxel;}}
    private int[,,] _voxel;
    
    public ProceduralModifier[] pcgSteps;

    void Awake() {
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
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        /*
        for(int i = 0; i < pcgSteps.Length; i++) {
            pcgSteps[i].Execute();
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        
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

    private bool inBound(int x, int y, int z) {
        return x >= 0 && y >= 0 && z >= 0 && x < voxelSize && y < voxelSize && z < voxelSize;
    }

}
