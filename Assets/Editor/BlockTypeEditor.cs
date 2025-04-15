using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BlockType))]
public class BlockTypeEditor : Editor
{
    private readonly string[] faceNames = { "Front", "Rear", "Left", "Right", "Bottom", "Top" };

    public override void OnInspectorGUI()
    {
        BlockType block = (BlockType)target;

       
        block.top = (Texture2D)EditorGUILayout.ObjectField("Top Texture", block.top, typeof(Texture2D), false);
        block.bottom = (Texture2D)EditorGUILayout.ObjectField("Bottom Texture", block.bottom, typeof(Texture2D), false);
        block.left = (Texture2D)EditorGUILayout.ObjectField("Left Texture", block.left, typeof(Texture2D), false);
        block.right = (Texture2D)EditorGUILayout.ObjectField("Right Texture", block.right, typeof(Texture2D), false);
        block.front = (Texture2D)EditorGUILayout.ObjectField("Front Texture", block.front, typeof(Texture2D), false);
        block.rear = (Texture2D)EditorGUILayout.ObjectField("Rear Texture", block.rear, typeof(Texture2D), false);

        block.id = EditorGUILayout.IntField("Block ID", block.id);

       
        EnsureArrayLength(ref block.aliveColors, 6);
        EnsureArrayLength(ref block.deadColors, 6);
        EnsureArrayLength(ref block.caRules, 6);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Face Settings", EditorStyles.boldLabel);

        for (int i = 0; i < 6; i++)
        {
            EditorGUILayout.LabelField(faceNames[i], EditorStyles.boldLabel);
            block.aliveColors[i] = EditorGUILayout.ColorField("Alive Color", block.aliveColors[i]);
            block.deadColors[i] = EditorGUILayout.ColorField("Dead Color", block.deadColors[i]);
            block.caRules[i] = EditorGUILayout.IntField("CA Rule", block.caRules[i]);
            EditorGUILayout.Space();
        }

        if (GUI.changed)
            EditorUtility.SetDirty(block);
    }

    private void EnsureArrayLength<T>(ref T[] array, int length)
    {
        if (array == null || array.Length != length)
        {
            T[] newArray = new T[length];
            if (array != null)
            {
                for (int i = 0; i < Mathf.Min(array.Length, length); i++)
                    newArray[i] = array[i];
            }
            array = newArray;
        }
    }
}
