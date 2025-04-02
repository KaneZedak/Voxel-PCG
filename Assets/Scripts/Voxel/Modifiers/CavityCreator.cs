using UnityEngine;

[CreateAssetMenu(fileName = "Cavity", menuName = "Scriptable Objects/Cavity")]
public class CavityCreator : ProceduralModifier
{
    public Vector3 location = new Vector3(0, 0, 0);
    public Vector3 size = new Vector3(5, 5, 5);
    public enum Shape {
        Oval = 0,
        Square = 1
    }
    public CavityCreator() {

    }

    //use this constructor to initialize location and size
    public CavityCreator(Vector3 cLocation, Vector3 cSize) {
        location = cLocation;
        size = cSize;
    }

    public override void Execute() {
        return;
    }    
}
