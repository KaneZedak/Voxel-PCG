using UnityEngine;

[CreateAssetMenu(fileName = "Cavity", menuName = "Scriptable Objects/Cavity")]
[System.Serializable]
public class CavityCreator : ProceduralModifier
{
    //public int[] locArray = new int[3];
    //public int[] sizeArray = new int[3];

    public Vector3 location; /*{
        get {
            return new Vector3(locArray[0], locArray[1], locArray[2]);
        }
        set {
            locArray[0] = (int)location.x;
            locArray[1] = (int)location.y;
            locArray[2] = (int)location.z;
        }
    }*/

    public Vector3 size; /*{
        get {
            return new Vector3(sizeArray[0], sizeArray[1], sizeArray[2]);
        }
        set {
            sizeArray[0] = (int)size.x;
            sizeArray[1] = (int)size.y;
            sizeArray[2] = (int)size.z;
        }
    }*/

    //available shapes for cavity generation
    public enum Shape {
        Sphere = 0,
        Square = 1
    }
    
    public Shape cavityShape;

    // Method to initialize the CavityCreator properties
    public void initializeCavity(Vector3 cLocation, Vector3 cSize, Shape cShape) {
        location = cLocation;
        size = cSize;
        cavityShape = cShape;
    }

    public override void Execute() {
        var voxel = voxelContainer.voxel;
        
        if(cavityShape == Shape.Square || cavityShape == Shape.Sphere) {

            int centerX = (int)location.x;
            int centerY = (int)location.y;
            int centerZ = (int)location.z;

            voxel[(int)location.x, (int)location.y, (int)location.z] = -1;
            
            int lowerX = (int)Mathf.Max(0, location.x - size.x + 1);
            int lowerY = (int)Mathf.Max(0, location.y - size.y + 1);
            int lowerZ = (int)Mathf.Max(0, location.z - size.z + 1);

            int upperX = (int)Mathf.Min(location.x + size.x - 1, voxelContainer.voxelSize - 1);
            int upperY = (int)Mathf.Min(location.y + size.y - 1, voxelContainer.voxelSize - 1);
            int upperZ = (int)Mathf.Min(location.z + size.z - 1, voxelContainer.voxelSize - 1);
            Debug.Log($"Cavity Bounds: Lower({lowerX}, {lowerY}, {lowerZ}), Upper({upperX}, {upperY}, {upperZ})");
            for(int i = lowerX; i <= upperX; i++) {
                for(int j = lowerY; j <= upperY; j++) {
                    for(int k = lowerZ; k <= upperZ; k++) {
                        if(cavityShape == Shape.Sphere) {
                            if(Mathf.Pow((i - centerX), 2) / Mathf.Pow(size.x, 2) + Mathf.Pow((j - centerY), 2) / Mathf.Pow(size.y, 2) + Mathf.Pow((k - centerZ), 2) / Mathf.Pow(size.z, 2) <= 1f) {
                                voxel[i,j,k] = -1;
                            }
                        } else {
                            voxel[i,j,k] = -1;
                        }
                    }
                }
            }
        }
        return;
    }    

    private float calculateEuclidean(float x1, float y1, float z1, float x2, float y2, float z2)
    {
        return Mathf.Pow(Mathf.Pow(x1 - x2, 2) + Mathf.Pow(y1 - y2, 2) + Mathf.Pow(z1 - z2, 2), 0.5f);
    }
}
