using UnityEngine;

[CreateAssetMenu(fileName = "BlockType", menuName = "Scriptable Objects/BlockType")]
public class BlockType : ScriptableObject
{
    public int defaultTextureSize;
    public Texture2D top;
    public Texture2D bottom;
    public Texture2D left;
    public Texture2D right;
    public Texture2D front;
    public Texture2D rear;

    private Texture2D blockTextureAtlas;
    private Texture2D[] faceTextures;
    private Rect[] textureRects;
    private int[] UVRectIndex;

    private int maxTextureSize = 0;

    public int minRectIndex {get;set;}
    public int maxRectIndex {get;set;}
    
    public Vector2[,] uvVertexPos {
        get{return this._uvVertex;}
        set{this._uvVertex = uvVertexPos;}
    }
    private Vector2[,] _uvVertex;

    public void initialize() {
        Texture2D[] textures = {front, rear, left, right, bottom, top};
        faceTextures = textures;

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
