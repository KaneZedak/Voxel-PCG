using UnityEngine;

[CreateAssetMenu(fileName = "Erosion", menuName = "Scriptable Objects/Erosion")]
[System.Serializable]
//use CA for iterations of erosion
public class Erosion : ProceduralModifier
{
    public int iterations;
    public bool adjacentErosion = false;    //erode by counting only blocks with adjacent face as neighbors. overrides noCorner
    public bool noCorner = false;   //erode without counting corner neighbors.
    private int[] adjacent = {-1, 0, 1};
    private int[,,] voxel;
    

    public override void Execute() {
        if(voxelContainer == null) Debug.Log("Container is not set");
        voxel = voxelContainer.voxel;

        Debug.Log("Running Erosion: iteraion = " + iterations + ", adjacent = " + adjacentErosion);
        for(int m = 0; m < iterations; m++) {
            for(int i = 0; i < voxelContainer.voxelSize; i++) {
                for(int j = 0; j < voxelContainer.voxelSize; j++) {
                    for(int k = 0; k < voxelContainer.voxelSize; k++) {
                        if(voxel[i,j,k] == -1) continue;
                        int result = calculateNeighbor(i,j,k, adjacentErosion);
                        if(result == 0) voxel[i,j,k] = -1;
                        /*
                        else if(result < 26) {
                            float chance = 0.1f;
                            
                            if(result < 20) {
                                chance = 0.6f;
                            }

                            if(result < 10) {
                                chance = 0.8f;
                            }

                            if(result == 0) chance = 1.0f;

                            if(Random.Range(0f, 1.0f) <= chance) voxel[i,j,k] = -1;
                        }*/
                        int total = (adjacentErosion == true)?6:26;
                        if(Random.Range(0f, total) > result) {
                            voxel[i,j,k] = -1;
                        }
                    }
                }
            }
        }
    }

    private int calculateNeighbor(int x, int y, int z, bool adjacentOnly = false) {
        int sum = 0;
        for(int i = 0; i < 3; i++) {
            for(int j = 0; j < 3; j++) {
                for(int k = 0; k < 3; k++) {
                    if(!(adjacent[i] == 0 && adjacent[j] == 0 && adjacent[k] == 0)) {
                        if(adjacentOnly && ((int)Mathf.Abs(adjacent[i]) + (int)Mathf.Abs(adjacent[j]) + (int)Mathf.Abs(adjacent[k])) > 1) continue;
                        int dx = x + adjacent[i];
                        int dy = y + adjacent[j];
                        int dz = z + adjacent[k];

                        if(dx >= 0 && dy >= 0 && dz >= 0 && dx < voxelContainer.voxelSize && dy < voxelContainer.voxelSize && dz < voxelContainer.voxelSize) {
                            if(voxel[dx,dy,dz] != -1) {
                                sum += 1;
                            }
                        } else {
                            sum += 1;
                        }
                    }
                }
            }
        }

        return sum;
    }
}
