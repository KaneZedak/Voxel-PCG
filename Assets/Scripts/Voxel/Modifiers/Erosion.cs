using UnityEngine;
using System;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "Erosion", menuName = "Scriptable Objects/Erosion")]
[System.Serializable]
//use CA for iterations of erosion
public class Erosion : ProceduralModifier
{
    [System.Serializable]
    public class Singularity {
        public int x;
        public int y;
        public int z;
        public float strength;
        public int maxRange;
        public int maxDeviation;

        public Singularity(int x, int y, int z, float strength, int maxRange, int maxDeviation) {
            this.x = x;
            this.y = y;
            this.z = z;
            this.strength = strength;
            this.maxRange = maxRange;
            this.maxDeviation = maxDeviation;
        }
    }

    public int iterations;
    public bool adjacentErosion = false;    //erode by counting only blocks with adjacent face as neighbors. overrides noCorner
    public Vector3 direction;
    public bool linear = true;
    public Singularity[] singularities;
    private int[] adjacent = {-1, 0, 1};
    private int[,,] voxel;
    
    
    public override void Execute() {
        if(voxelContainer == null) Debug.Log("Container is not set");
        voxel = voxelContainer.voxel;
        
        List<Singularity> singularityList = new List<Singularity>();
        for(int i = 0; i < voxelContainer.roomLocations.Length; i++) {
            singularityList.Add(new Singularity((int)voxelContainer.roomLocations[i].x, (int)voxelContainer.roomLocations[i].y, (int)voxelContainer.roomLocations[i].z, 9999f, 5, 2));
        }
        singularities = singularityList.ToArray();

        Debug.Log("Running Erosion: iteraion = " + iterations + ", adjacent = " + adjacentErosion);
        for(int m = 0; m < iterations; m++) {
            for(int i = 0; i < voxelContainer.voxelSize; i++) {
                for(int j = 0; j < voxelContainer.voxelSize; j++) {
                    for(int k = 0; k < voxelContainer.voxelSize; k++) {
                        if(voxel[i,j,k] == -1) continue;
                        int result = calculateNeighbor(i,j,k, adjacentErosion);
                        int total = (adjacentErosion == true)?6:26;

                        if(result == 0) {
                            voxel[i,j,k] = -1;
                            continue;
                        } else if(result == total) {
                            continue;
                        }

                        
                        int deviation = 0;

                        for(int s = 0; s < singularities.Length; s++) {
                            if(calculateManhattan(i, j, k, singularities[s].x, singularities[s].y, singularities[s].z) / (singularities[s].maxRange + 0.0f) * singularities[s].strength
                             < UnityEngine.Random.Range(0f, 1f)) {
                                deviation += UnityEngine.Random.Range(0, singularities[s].maxDeviation);
                            }
                        }

                        result -= deviation;

                        if(result < 0) result = 0;

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
                        
                        if(!linear) {
                            if(UnityEngine.Random.Range(0f, 1f)  > Mathf.Pow(result / (total + 0.0f),2)) {
                                voxel[i,j,k] = -1;
                            }
                        } else {
                            if(UnityEngine.Random.Range(0f, 1f)  > result / (total + 0.0f)) {
                                voxel[i,j,k] = -1;
                            }
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

    private int calculateManhattan(int x1, int y1, int z1, int x2, int y2, int z2)
    {
        return Math.Abs(x1 - x2) + Math.Abs(y1 - y2) + Math.Abs(z1 - z2);
    }
}
