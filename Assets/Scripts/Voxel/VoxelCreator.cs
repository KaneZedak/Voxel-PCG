using UnityEngine;

public class VoxelCreator : MonoBehaviour
{
    public int voxelSize = 1000;
    public BlockType[,,] voxel {get {return _voxel;} set {_voxel = voxel;}}
    private BlockType[,,] _voxel;
    
    public ProceduralModifier[] pcgSteps;

    void Awake() {
        _voxel = new BlockType[voxelSize,voxelSize,voxelSize];
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
    
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //return the block type at given location
    public BlockType getBlockAt(int x, int y, int z) {
        if(inBound(x, y, z)) {
            return _voxel[x,y,z];
        }

        return null;
    }

    //set block to a block type at certain location
    public void setBlock(int x, int y, int z, BlockType bkType) {
        if(inBound(x, y, z)) {
            _voxel[x,y,z] = bkType;
        }
    }

    private bool inBound(int x, int y, int z) {
        return x >= 0 && y >= 0 && z >= 0 && x < voxelSize && y < voxelSize && z < voxelSize;
    }

}
