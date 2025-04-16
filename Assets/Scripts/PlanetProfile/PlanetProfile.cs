using UnityEngine;

[CreateAssetMenu(fileName = "PlanetProfile", menuName = "Scriptable Objects/PlanetProfile")]
public class PlanetProfile : ScriptableObject
{
    public string planetName;
    public int planetID;
    public float minDistance;
    public float maxDistance;
    public float distanceProportion;

    [System.Serializable]
    public struct RoomGenerationSettings {
        public int numberOfRooms;
        public int minNumberOfRooms;
        public int maxNumberOfRooms;
        public float minAdjacentRoomDistance;
        public float maxAdjacentRoomDistance;
    } 
    
    [System.Serializable]
    public struct PathwaySettings {
        public float middlePointDiff;
        public int numberOfMidpoints;
        public float levelOfAdjacency;
    }

    [System.Serializable]
    public struct ErosionSettings {
        public float baseSingularityStrength;
        public float baseSingularityRange;
        public int baseSingularityEffect;
    }

    [System.Serializable]
    public struct TextureSettings {
        public BlockType[] blockTypes;
    }

    [System.Serializable]
    public struct RenderSettings {
        public Color fogColor;
        public Color ambientColor;
    }


    public RoomGenerationSettings roomGenerationSetting;
    public PathwaySettings paythwaySetting;
    public ErosionSettings erosionSetting;
    public RenderSettings renderSettings;
    public TextureSettings blockTextureSettings;
    
    public void initialize(float currentDistance)
    {
        if (maxDistance > minDistance)
        {
            // Linear interpolation to calculate distanceProportion
            distanceProportion = Mathf.Clamp01((currentDistance - minDistance) / (maxDistance - minDistance));
        }
        else
        {
            Debug.LogWarning("Invalid minDistance and maxDistance values. Setting distanceProportion to 0.");
            distanceProportion = 0f;
        }
        Debug.Log("current distance "+ currentDistance);
        Debug.Log("distanceProportion "+ distanceProportion);
        // Set numberOfRooms to range from 4 to 8 based on distanceProportion
        roomGenerationSetting.numberOfRooms += Mathf.RoundToInt(Mathf.Lerp(0, 4, distanceProportion));
        roomGenerationSetting.minAdjacentRoomDistance = 15 + Mathf.Lerp(-5, 10, distanceProportion);
        roomGenerationSetting.maxAdjacentRoomDistance = 20 + Mathf.Lerp(-5, 15, distanceProportion);
        erosionSetting.baseSingularityStrength = Mathf.Lerp(0.3f, 1f,distanceProportion);
    }
}
