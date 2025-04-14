using UnityEngine;

[CreateAssetMenu(fileName = "ModuleSetting", menuName = "Scriptable Objects/ModuleSetting")]
[System.Serializable]
public class ModuleSetting : ScriptableObject
{
    [SerializeField] string moduleName = "";
}
