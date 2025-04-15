using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "BlockCleaning", menuName = "Scriptable Objects/BlockCleaning")]
public class BlockCleaning : ProceduralModifier
{
    class BlockPos {
        public int x;
        public int y;
        public int z;

        public BlockPos(int x, int y, int z) {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }

    private int[] adjacent = {-1, 0, 1};
    private bool[,,] mark;

    public override void Execute() {
        //0, 0, 0 always filled and not empty
        BlockPos startingBlock = new BlockPos(0, 0, 0);
        mark = new bool[voxelContainer.voxelSize,voxelContainer.voxelSize,voxelContainer.voxelSize];
        Queue<BlockPos> q = new Queue<BlockPos>();

        q.Enqueue(startingBlock);

        BlockPos currentBlock;
        int cnt = 0;

        while(q.TryDequeue(out currentBlock)) {
            mark[currentBlock.x, currentBlock.y, currentBlock.z] = true;
            for(int i = 0; i < 3; i++) {
                 for(int j = 0; j < 3; j++) {
                    for(int k = 0; k < 3; k++) {
                        if(adjacent[i] == 0 && adjacent[j] == 0 && adjacent[k] == 0) continue;
                        int adjacentX = adjacent[i] + currentBlock.x;
                        int adjacentY = adjacent[j] + currentBlock.y;
                        int adjacentZ = adjacent[k] + currentBlock.z;
                        if(voxelContainer.inBound(adjacentX, adjacentY, adjacentZ)) {
                            if(mark[adjacentX, adjacentY, adjacentZ] == false && voxelContainer.voxel[adjacentX, adjacentY, adjacentZ] != -1) {
                                q.Enqueue(new BlockPos(adjacentX, adjacentY, adjacentZ));
                                mark[adjacentX, adjacentY, adjacentZ] = true;
                            }
                        }
                    }
                 }
            }

            cnt++;
        }

        for(int i = 0;i < voxelContainer.voxelSize; i++) {
            for(int j = 0; j < voxelContainer.voxelSize; j++) {
                for(int k = 0; k < voxelContainer.voxelSize; k++) {
                    if(mark[i,j,k] == false) {
                        voxelContainer.voxel[i, j, k] = -1;
                    }
                }
            }
        }
    }
}
