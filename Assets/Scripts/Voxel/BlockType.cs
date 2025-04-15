using UnityEngine;

[CreateAssetMenu(fileName = "BlockType", menuName = "Scriptable Objects/BlockType")]
public class BlockType : ScriptableObject
{
    public int id;

    
    public Texture2D top;
    public Texture2D bottom;
    public Texture2D left;
    public Texture2D right;
    public Texture2D front;
    public Texture2D rear;

   
    public Color[] aliveColors = new Color[6];
    public Color[] deadColors = new Color[6];
    public int[] caRules = new int[6]; 

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

        
        if (aliveColors == null || aliveColors.Length != 6)
            aliveColors = new Color[] { Color.red, Color.gray, Color.magenta, new Color(1f, 0.5f, 0f), new Color(0.4f, 0.2f, 0f), Color.green };

        if (deadColors == null || deadColors.Length != 6)
            deadColors = new Color[] { Color.black, Color.black, Color.black, Color.black, Color.black, Color.black };

        if (caRules == null || caRules.Length != 6)
            caRules = new int[] { 1, 0, 4, 3, 2, 0 };
    }

    public Texture2D[] getBlockFaceTextures()
    {
        return faceTextures;
    }
}
