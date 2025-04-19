#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[CustomEditor(typeof(PlanetProfile))]
public class PlanetProfileEditor: Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
    }
}

#endif