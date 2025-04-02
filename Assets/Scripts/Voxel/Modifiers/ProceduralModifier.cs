using UnityEngine;

//class for PCG steps;
[System.Serializable]
public abstract class ProceduralModifier : ScriptableObject
{
    //user voxelContainer to get access to the linked VoxelCreator
    protected VoxelCreator voxelContainer;

    public void initialize(VoxelCreator vxContainer) {
        this.voxelContainer = vxContainer;
    }

    public abstract void Execute();
    //function for executing the procedural generating step
    
}
