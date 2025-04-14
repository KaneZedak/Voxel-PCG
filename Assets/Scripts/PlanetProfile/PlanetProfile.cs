using UnityEngine;

[CreateAssetMenu(fileName = "PlanetProfile", menuName = "Scriptable Objects/PlanetProfile")]
public class PlanetProfile : ScriptableObject
{
    public string planetName;
    public int planetID;
    public float avgDistance;
    public float distanceProportion {get;private set;}

    [System.Serializable]
    public struct RoomGenerationSettings {
        public int minNumberOfRooms;
        public int maxNumberOfRooms;
        public float minAdjacentRoomDistance;
        public float maxAdjacentRoomDistance;
    } 
    
    [System.Serializable]
    public struct PathwaySettings {
        public float middlePointDiff;
        public int numberOfMidpoints;
    }

    [System.Serializable]
    public struct ErosionSettings {
        public float baseSingularityStrength;
        public float baseSingularityRange;
        public int baseSingularityEffect;
    }

    public RoomGenerationSettings roomGenerationSetting;
    public PathwaySettings paythwaySetting;
    public ErosionSettings erosionSetting;

    public void initialize(float currentDistance) {
        distanceProportion = currentDistance / (avgDistance + 0.0f);
    }
}
