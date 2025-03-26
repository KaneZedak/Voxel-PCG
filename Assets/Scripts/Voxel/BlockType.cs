using UnityEngine;

[CreateAssetMenu(fileName = "BlockType", menuName = "Scriptable Objects/BlockType")]
public class BlockType : ScriptableObject
{
    public Texture top;
    public Texture bottom;
    public Texture left;
    public Texture right;
    public Texture front;
    public Texture rear;

    public Vector2[,] uvVertexPos {
        get{return this._uvVertex;}
        set{this._uvVertex = uvVertexPos;}
    }
    private Vector2[,] _uvVertex;
}
