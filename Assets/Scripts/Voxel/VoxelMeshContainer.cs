using UnityEngine;
using System.Collections.Generic;

public class VoxelMeshContainer : MonoBehaviour
{
    public int dimensionSize = 20;
    public BlockType[,,] voxel;
    public int blockSize;
    private MeshFilter meshFilter;

    private List<Vector3> meshVertices = new List<Vector3>();
    private List<int> meshTriangles = new List<int>();
    private List<Vector2> meshUV = new List<Vector2>();

    private Vector3[] vertices = new Vector3[8]{
        new Vector3(0, 0, 0),
        new Vector3(1, 0, 0),
        new Vector3(1, 0, 1),
        new Vector3(0, 0, 1),
        new Vector3(0, 1, 0),
        new Vector3(1, 1, 0),
        new Vector3(1, 1, 1),
        new Vector3(0, 1, 1)
    };


    //order goes from bottom left, bottom right, top left, top right and assuming the face is facing outward

    private int[,] faceVertices = {
        {0, 1, 4, 5}, //front face
        {2, 3, 6, 7}, //rear face
        {3, 0, 7, 4}, // left face
        {1, 2, 5, 6}, // right face
        {1, 0, 2, 3}, // bottom face
        {4, 5, 7, 6} // top face
    };


    private int[] faceTriangleIndex = {
        0, 2, 1,
        1, 2, 3,
    };

    private int[,] adjOffset = {
        {0, 0, -1},
        {0, 0, 1},
        {-1, 0, 0},
        {1, 0, 0},
        {0, -1, 0},
        {0, 1, 0}
    };

    void Awake() {
        voxel = new BlockType[dimensionSize,dimensionSize, dimensionSize];
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        for(int i = 0; i < 5; i++) {
            voxel[1, 1 + i, 1] = new BlockType();
        }
        for(int i = 0; i < dimensionSize; i++) {
            for(int j = 0; j < dimensionSize; j++) {
                for(int k = 0; k < dimensionSize; k++) {
                    if(voxel[i,j,k] != null) renderQuad(i, j, k);
                }
            }
        }

        //for(int i = 0 ;i < meshVertices.Count; i++) Debug.Log(meshVertices[i]);
        //for(int i = 0; i < meshTriangles.Count; i++) Debug.Log(meshVertices[meshTriangles[i]]);
        meshFilter.mesh = new Mesh();
        meshFilter.mesh.vertices = meshVertices.ToArray();
        meshFilter.mesh.triangles = meshTriangles.ToArray();
        meshFilter.mesh.uv = meshUV.ToArray();
    }

    private void renderQuad(int x, int y, int z) {
        for(int i = 0; i < 6; i++) {
            int adjX = x + adjOffset[i,0];
            int adjY = y + adjOffset[i,1];
            int adjZ = z + adjOffset[i,2];
            
            if(adjX >= 0 && adjY >= 0 && adjZ >= 0 && adjX < dimensionSize && adjY < dimensionSize && adjZ < dimensionSize) {
                //Debug.Log("X:" + adjX + "Y:" + adjY + "Z:" + adjZ);
                
                if(voxel[adjX,adjY, adjZ] == null) {
                    addFace(x, y, z, i, voxel[x,y,z]);
                }
            } else {
                addFace(x, y, z, i, voxel[x,y,z]);
            }
        }
        return;
    }

    private void addFace(int x, int y, int z, int index, BlockType blockType) {
        int vertexIndex = meshVertices.Count;
        int[] verticesIndex = {vertexIndex, vertexIndex+1, vertexIndex + 2, vertexIndex + 3};

        for(int i = 0; i < 4; i++) {
            Vector3 devVector = vertices[faceVertices[index,i]];
            Vector3 VertexPosIndex = new Vector3(devVector.x + x, devVector.y + y, devVector.z + z);
            Vector3 VertexPos = new Vector3(VertexPosIndex.x * blockSize, VertexPosIndex.y * blockSize, VertexPosIndex.z * blockSize);
            
            meshVertices.Add(VertexPos);
        }

        
        for(int j = 0; j < 6; j++) {
            meshTriangles.Add(verticesIndex[faceTriangleIndex[j]]);
        }
        
        return;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
