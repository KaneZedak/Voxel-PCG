using UnityEngine;
using System.Collections.Generic;

public class VoxelMeshContainer : MonoBehaviour
{
    private int[,,] _voxel;
    private int dimensionSize;

    public int[,,] voxel {
        get {
            return _voxel;
        }
        set {
            _voxel = voxel;
        }
    }


    public float blockSize;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    private List<Vector3> meshVertices = new List<Vector3>();
    private List<int> meshTriangles = new List<int>();
    private List<Vector2> meshUV = new List<Vector2>();

    public BlockType[] blockTypes;
    private TextureManager textureManager;
    private Material material;

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
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();

        textureManager = new TextureManager();
        textureManager.blockTypes = blockTypes;
        

        

        
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        textureManager.initialize();
        meshRenderer.material.mainTexture = textureManager.getTexture();

        updateVoxel(GameObject.Find("VoxelCreator").GetComponent<VoxelCreator>().voxel);
        
        //createMesh();
        renderMesh();
    }

    private void createMesh() {
        voxel[2, 2, 3] = 0;
    }

    private void renderMesh() {
        for(int i = 0; i < dimensionSize; i++) {
            for(int j = 0; j < dimensionSize; j++) {
                for(int k = 0; k < dimensionSize; k++) {
                    if(voxel[i,j,k] != -1) renderQuad(i, j, k);
                }
            }
        }

        meshFilter.mesh = new Mesh();
        meshFilter.mesh.vertices = meshVertices.ToArray();
        meshFilter.mesh.triangles = meshTriangles.ToArray();
        meshFilter.mesh.uv = meshUV.ToArray();
        //for(int i = 0; i < meshUV.Count;i ++) Debug.Log(meshUV[i]);
        Debug.Log(meshFilter.mesh.triangles.Length);
    }

    private void renderQuad(int x, int y, int z) {
        for(int i = 0; i < 6; i++) {
            int adjX = x + adjOffset[i,0];
            int adjY = y + adjOffset[i,1];
            int adjZ = z + adjOffset[i,2];
            
            if((adjX >= 0 && adjY >= 0 && adjZ >= 0 && adjX < dimensionSize && adjY < dimensionSize && adjZ < dimensionSize)) {
                if((voxel[adjX,adjY, adjZ] == -1)) {
                    addFace(x, y, z, i);
                }
            } else {
                addFace(x, y, z, i);
            }
        }
        return;
    }

    private void addFace(int x, int y, int z, int index) {
        int vertexIndex = meshVertices.Count;
        int[] verticesIndex = {vertexIndex, vertexIndex+1, vertexIndex + 2, vertexIndex + 3};
        Rect faceUVRect = textureManager.getTextureRectById(voxel[x,y,z], index);
        Vector2[] UVCoord = {new Vector2(faceUVRect.x, faceUVRect.y + faceUVRect.height),
                         new Vector2(faceUVRect.x + faceUVRect.width, faceUVRect.y + faceUVRect.height),
                         new Vector2(faceUVRect.x, faceUVRect.y),
                         new Vector2(faceUVRect.x + faceUVRect.width, faceUVRect.y)
                         };

        for(int i = 0; i < 4; i++) {
            Vector3 devVector = vertices[faceVertices[index,i]];
            Vector3 VertexPosIndex = new Vector3(devVector.x + x, devVector.y + y, devVector.z + z);
            Vector3 VertexPos = new Vector3(VertexPosIndex.x * blockSize, VertexPosIndex.y * blockSize, VertexPosIndex.z * blockSize);
            
            meshVertices.Add(VertexPos);      
            meshUV.Add(UVCoord[i]);
        }

        
        for(int j = 0; j < 6; j++) {
            meshTriangles.Add(verticesIndex[faceTriangleIndex[j]]);
        }
        
        return;
    }

    public void updateVoxel(int[,,] newVoxel) {
        if(newVoxel == null) return;
        _voxel = newVoxel;
        dimensionSize = newVoxel.GetLength(0);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
