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

    private Texture2D[] faceTextures;

    public void Initialize()
    {
        faceTextures = new Texture2D[]
        {
            front  ?? Texture2D.blackTexture,
            rear   ?? Texture2D.blackTexture,
            left   ?? Texture2D.blackTexture,
            right  ?? Texture2D.blackTexture,
            bottom ?? Texture2D.blackTexture,
            top    ?? Texture2D.blackTexture
        };
    }

    public Texture2D[] getBlockFaceTextures()
    {
        return faceTextures;
    }
}