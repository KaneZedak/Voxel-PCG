using UnityEngine;

[CreateAssetMenu(fileName = "BlockType", menuName = "Scriptable Objects/BlockType")]
public class BlockType : ScriptableObject
{
    public Texture2D top;
    public Texture2D bottom;
    public Texture2D left;
    public Texture2D right;
    public Texture2D front;
    public Texture2D rear;
    public int id;

    private Texture2D blockTextureAtlas;
    private Texture2D[] faceTextures;
    private Rect[] textureRects;
    private int[] UVRectIndex = new int[6];

    private int maxTextureSize = 0;

    public int rectIndex {get;set;}
    public int textureTotalSize {get; private set;}
    
    public Vector2[,] uvVertexPos {
        get{return this._uvVertex;}
        set{this._uvVertex = uvVertexPos;}
    }
    private Vector2[,] _uvVertex;

    public void initialize() {
        Texture2D[] textures = {front, rear, left, right, bottom, top};
        faceTextures = textures;

        textureTotalSize = 0;
        
        for(int i = 0; i < textures.Length; i++) {
            if(textures[i] != null) {
                textureTotalSize += textures[i].width * textures[i].height;
            }
        }
    }

    public Texture2D getBlockTextureAtlas() {
        return blockTextureAtlas;
    }

    public Texture2D[] getBlockFaceTextures() {
        return faceTextures;
    }

    public Rect getUVVertices(int index) {
        return textureRects[index];
    }
}
