using UnityEngine;
using System.Collections.Generic;

public class VoxelMeshContainer : MonoBehaviour
{
    private VoxelCreator voxelContainer = null;
    private int[,,] _voxel = null;
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
    public int chunkSize = 16;
    public bool renderOuter = false;

    private List<Vector3> meshVertices = new List<Vector3>();
    private List<int> meshTriangles = new List<int>();
    private List<Vector2> meshUV = new List<Vector2>();
    private List<Vector3> meshNormals = new List<Vector3>();
    private GameObject[,,] subMeshes;
    private Queue<Vector3> subMeshQueue = new Queue<Vector3>();

    public BlockType[] blockTypes;
    public TextureManager textureManager;
   
    private Material sharedMaterial;
    private Material material;
    private bool firstTimeRendering = true;
    private bool[,,] taintedChunk;
    private int chunkDimension;

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
       
        textureManager.blockTypes = blockTypes;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        textureManager.Initialize();

        sharedMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        sharedMaterial.mainTexture = textureManager.GetTexture();  
        sharedMaterial.enableInstancing = true;
    }

    void Update() {
        if(voxelContainer == null) {
            voxelContainer = GameObject.Find("VoxelCreator").GetComponent<VoxelCreator>();
            
        } else {
            
            if(_voxel == null && voxelContainer.voxel != null) {
                //if(voxelContainer.voxel != null) Debug.Log("set");
                updateVoxel(voxelContainer.voxel);
            }
        }

        if(voxel != null && voxelContainer != null) {
            if(firstTimeRendering) {
                renderMesh();
                firstTimeRendering = false;
            } else {
                Vector3 activeMesh;
                if(subMeshQueue.TryDequeue(out activeMesh)) {
                    subMeshQueue.Enqueue(activeMesh);
                    renderChunkMesh((int)activeMesh.x, (int)activeMesh.y, (int)activeMesh.z);
                }
            }
        }
    }

    private void renderChunkMesh(int x, int y, int z) {
        for(int i = x * chunkSize; i < (x + 1) * chunkSize; i++) {
            for(int j = y * chunkSize; j < (y + 1) * chunkSize; j++) {
                for(int k = z * chunkSize; k < (z + 1) * chunkSize; k++) {
                    if(i < dimensionSize && j < dimensionSize && k < dimensionSize) {
                        if(voxel[i,j,k] != -1) renderQuad(i,j,k);
                    }
                }
            }
        }
        GameObject chunkObject;
        MeshFilter meshFilter;
        MeshRenderer meshRenderer;
        MeshCollider meshCollider;


        //create new game object for the chunk if not exit
        if(subMeshes[x,y,z] == null) {
            chunkObject = new GameObject(x + " " + y + " " + z);
            chunkObject.isStatic = true;
            subMeshes[x,y,z] = chunkObject;
            chunkObject.AddComponent<MeshFilter>();
            chunkObject.AddComponent<MeshRenderer>();
            chunkObject.GetComponent<MeshFilter>().mesh = new Mesh();
            chunkObject.AddComponent<MeshCollider>();
            
        } else chunkObject = subMeshes[x,y,z];

        meshFilter = chunkObject.GetComponent<MeshFilter>();
        meshRenderer = chunkObject.GetComponent<MeshRenderer>();
        meshCollider = chunkObject.GetComponent<MeshCollider>();
        meshCollider.sharedMesh = chunkObject.GetComponent<MeshFilter>().mesh;
        
        chunkObject = subMeshes[x,y,z];

        chunkObject.transform.parent = this.transform;
        meshRenderer.material = sharedMaterial;

        //meshRenderer.material.mainTexture = textureManager.GetTexture();
     

        //Debug.Log(ReferenceEquals(meshRenderer.material.mainTexture, textureManager.GetTexture()));

        meshFilter.mesh.vertices = meshVertices.ToArray();
        meshFilter.mesh.triangles = meshTriangles.ToArray();
        meshFilter.mesh.uv = meshUV.ToArray();
        meshFilter.mesh.normals = meshNormals.ToArray();

        meshNormals.Clear();
        meshVertices.Clear();
        meshTriangles.Clear();
        meshUV.Clear();
    }

    private void renderMesh() {
        subMeshQueue.Clear();
        chunkDimension = dimensionSize / chunkSize + 1;
        subMeshes = new GameObject[chunkDimension,chunkDimension,chunkDimension];
        for(int i = 0; i < chunkDimension; i++) {
            for(int j = 0; j < chunkDimension; j++) {
                for(int k = 0; k < chunkDimension; k++) {
                    renderChunkMesh(i,j,k);
                    subMeshQueue.Enqueue(new Vector3(i, j, k));
                }
            }
        }

        //for(int i = 0; i < meshUV.Count;i ++) Debug.Log(meshUV[i]);
        //Debug.Log(meshFilter.mesh.triangles.Length);
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
            } else if(renderOuter) {
                addFace(x, y, z, i);
            }
        }
        return;
    }

    private void addFace(int x, int y, int z, int index) {
        int vertexIndex = meshVertices.Count;
        int[] verticesIndex = {vertexIndex, vertexIndex+1, vertexIndex + 2, vertexIndex + 3};
        Rect faceUVRect = textureManager.GetTextureRectById(voxel[x,y,z], index);
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
            meshNormals.Add(new Vector3(0,0,0));
        }

        int triangleIndex = meshTriangles.Count;

        for(int j = 0; j < 6; j++) {
            meshTriangles.Add(verticesIndex[faceTriangleIndex[j]]);
        }

        for(int j = triangleIndex; j < triangleIndex + 6; j += 3) {
            Vector3 calculatedNormal = CalculateSurfaceNormal(meshVertices[meshTriangles[j]], meshVertices[meshTriangles[j+1]], meshVertices[meshTriangles[j+2]]);
            meshNormals[meshTriangles[j]] += calculatedNormal;
            meshNormals[meshTriangles[j+1]] += calculatedNormal;
            meshNormals[meshTriangles[j+2]] += calculatedNormal;
        }
        
        return;
    }

    public void updateVoxel(int[,,] newVoxel) {
        if(newVoxel == null) return;
        _voxel = newVoxel;
        dimensionSize = newVoxel.GetLength(0);
    }

    private Vector3 CalculateSurfaceNormal(Vector3 pointA, Vector3 pointB, Vector3 pointC) {
        Vector3 sideAB = pointB - pointA;
        Vector3 sideAC = pointC - pointA;
        return Vector3.Cross(sideAB, sideAC).normalized;
    }
    public void markForUpdate(int x, int y, int z) {
        
    }
}
