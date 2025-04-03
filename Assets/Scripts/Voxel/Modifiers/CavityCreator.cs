using UnityEngine;

[CreateAssetMenu(fileName = "Cavity", menuName = "Scriptable Objects/Cavity")]
[System.Serializable]
public class CavityCreator : ProceduralModifier
{
    public int[] locArray = new int[3];
    public int[] sizeArray = new int[3];

    public Vector3 location {
        get {
            return new Vector3(locArray[0], locArray[1], locArray[2]);
        }
        set {
            locArray[0] = (int)location.x;
            locArray[1] = (int)location.y;
            locArray[2] = (int)location.z;
        }
    }

    public Vector3 size {
        get {
            return new Vector3(sizeArray[0], sizeArray[1], sizeArray[2]);
        }
        set {
            sizeArray[0] = (int)size.x;
            sizeArray[1] = (int)size.y;
            sizeArray[2] = (int)size.z;
        }
    }



    //available shapes for cavity generation
    public enum Shape {
        Oval = 0,
        Square = 1
    }
    
    public Shape cavityShape;

    public CavityCreator() {
        for(int i = 0; i < 3; i++) {
            locArray[i] = 0;
            sizeArray[i] = 3;
        }
        cavityShape = Shape.Square;
    }

    //use this constructor to initialize location and size
    public CavityCreator(Vector3 cLocation, Vector3 cSize, Shape cShape = Shape.Oval) {
        location = cLocation;
        size = cSize;
        cavityShape = cShape;
    }
    //use this constructor to initialize location and size
    public CavityCreator(int[] cLocation, int[] cSize, Shape cShape = Shape.Oval) {
        locArray = cLocation;
        sizeArray = cSize;
        cavityShape = cShape;
    }

    public override void Execute() {
        var voxel = voxelContainer.voxel;
        
        if(cavityShape == Shape.Square) {
            voxel[(int)location.x, (int)location.y, (int)location.z] = -1;
            
            int lowerX = (int)Mathf.Max(0, location.x - size.x + 1);
            int lowerY = (int)Mathf.Max(0, location.y - size.y + 1);
            int lowerZ = (int)Mathf.Max(0, location.z - size.x + 1);

            int upperX = (int)Mathf.Min(location.x + size.x - 1, voxelContainer.voxelSize - 1);
            int upperY = (int)Mathf.Min(location.y + size.y - 1, voxelContainer.voxelSize - 1);
            int upperZ = (int)Mathf.Min(location.z + size.z - 1, voxelContainer.voxelSize - 1);

            for(int i = lowerX; i <= upperX; i++) {
                for(int j = lowerY; j <= upperY; j++) {
                    for(int k = lowerZ; k <+ upperZ; k++) {
                        voxel[i,j,k] = -1;
                    }
                }
            }
        }
        return;
    }    
}
