using UnityEngine;

[CreateAssetMenu(fileName = "Erosion", menuName = "Scriptable Objects/Erosion")]
[System.Serializable]
//use CA for iterations of erosion
public class Erosion : ProceduralModifier
{
    public int iteractions;

    private int[] adjacent = {-1, 0, 1};
    private int[,,] voxel;


    public override void Execute() {
        voxel = voxelContainer.voxel;
        for(int i = 0; i < voxelContainer.voxelSize; i++) {
            for(int j = 0; j < voxelContainer.voxelSize; j++) {
                for(int k = 0; k < voxelContainer.voxelSize; k++) {
                    int result = calculateNeighbor(i,j,k);
                    if(result == 0) voxel[i,j,k] = -1;
                    else {
                        if(Random.Range(0, 27) > result) voxel[i,j,k] = -1;
                    }
                }
            }
        }
    }

    private int calculateNeighbor(int x, int y, int z) {
        int sum = 0;
        for(int i = 0; i < 3; i++) {
            for(int j = 0; j < 3; j++) {
                for(int k = 0; k < 3; k++) {
                    if(!(adjacent[i] == 0 && adjacent[j] == 0 && adjacent[k] == 0)) {
                        int dx = x + adjacent[i];
                        int dy = y + adjacent[j];
                        int dz = z + adjacent[k];

                        if(dx >= 0 && dy >= 0 && dz >= 0 && dx < voxelContainer.voxelSize && dy < voxelContainer.voxelSize && dz < voxelContainer.voxelSize) {
                            if(voxel[dx,dy,dz] != -1) {
                                sum += 1;
                            }
                        }
                    }
                }
            }
        }

        return sum;
    }
}
