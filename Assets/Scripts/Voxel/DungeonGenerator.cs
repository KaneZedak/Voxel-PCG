using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DungeonGenerator : MonoBehaviour
{
    public static DungeonGenerator Instance { get; private set; }
    public VoxelCreator creator;
    CavityCreator cavity;
    private Dictionary<int, List<int>> adjacencyDict;
    private Dictionary<int, Vector3> cavityLocations;
    string planetName;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(Instance == null)
        {
            Instance = this; // Set the singleton instance
        }
        else if (Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate instance
        }
    }

    //calls to create dungeon coroutine;
    public void CreateDungeon() {
        StartCoroutine(CreateDungeonCoroutine());
    }

    private IEnumerator CreateDungeonCoroutine()
    {
        //Wait for Planet Profile To Load
        while(!PlanetProfileLoader.finishedLoading) yield return null;
        // Add VoxelCreator component to the GameObject
        // Initialize adjacency dictionary
        adjacencyDict = new Dictionary<int, List<int>>();
        cavityLocations = new Dictionary<int, Vector3>();

        // Populate adjacency dictionary
        for (int i = 0; i < PlanetProfileLoader.activeProfile.roomGenerationSetting.numberOfRooms; i++)
        {
            adjacencyDict[i] = new List<int>();

            // Ensure each room is adjacent to at least one other room
            if (i > 0)
            {
                int randomAdjacent = Random.Range(0, i);
                adjacencyDict[i].Add(randomAdjacent);
                adjacencyDict[randomAdjacent].Add(i);
            }

            // Randomly determine additional adjacencies
            for (int j = 0; j < i; j++)
            {
                if (j != i && Random.value > PlanetProfileLoader.activeProfile.paythwaySetting.levelOfAdjacency && !adjacencyDict[i].Contains(j))
                {
                    adjacencyDict[i].Add(j);
                    adjacencyDict[j].Add(i);
                }
            }
        }

        // Debug adjacency dictionary
        foreach (var pair in adjacencyDict)
        {
            Debug.Log($"Room {pair.Key} is adjacent to: {string.Join(", ", pair.Value)}");
        }

        // Assign locations to cavities using breadth-first traversal
        Queue<int> roomQueue = new Queue<int>();
        roomQueue.Enqueue(0);
        cavityLocations[0] = new Vector3(100, 100, 100); // First cavity at a constant location
        CameraController.Instance.SetCameraStartLocation(cavityLocations[0]); // Set startLocation to the center of the first cavity

        while (roomQueue.Count > 0)
        {
            int currentRoom = roomQueue.Dequeue();

            foreach (int adjacentRoom in adjacencyDict[currentRoom])
            {
                if (!cavityLocations.ContainsKey(adjacentRoom))
                {
                    // Determine a location for the adjacent room
                    Vector3 referenceLocation = cavityLocations[currentRoom];
                    float distance = Random.Range(PlanetProfileLoader.activeProfile.roomGenerationSetting.minAdjacentRoomDistance, PlanetProfileLoader.activeProfile.roomGenerationSetting.maxAdjacentRoomDistance);
                    Vector3 offset = Random.onUnitSphere * distance;
                    Vector3 location = referenceLocation + offset;

                    // Clamp location to ensure it stays within voxel bounds
                    location.x = Mathf.Clamp(location.x, 0, creator.voxelSize - 1);
                    location.y = Mathf.Clamp(location.y, 0, creator.voxelSize - 1);
                    location.z = Mathf.Clamp(location.z, 0, creator.voxelSize - 1);

                    cavityLocations[adjacentRoom] = location;
                    roomQueue.Enqueue(adjacentRoom);
                }
            }
        }

        // Initialize roomLocations with numRooms
        creator.roomLocations = new Vector3[PlanetProfileLoader.activeProfile.roomGenerationSetting.numberOfRooms];

        // Initialize pcgSteps with numRooms cavities + erosion + connect rooms
        //creator.pcgSteps = new ProceduralModifier[numRooms + 2];
        
        // Use list to store newly created modifiers first
        List<ProceduralModifier> pcgModifiers = new List<ProceduralModifier>();

        for (int i = 0; i < PlanetProfileLoader.activeProfile.roomGenerationSetting.numberOfRooms; i++)
        {
            Vector3 location = cavityLocations[i];
            creator.roomLocations[i] = location; // Store room locations for use in ConnectRooms
            cavity = ScriptableObject.CreateInstance<CavityCreator>();
            cavity.initializeCavity(location, new Vector3(3, 4, 4), CavityCreator.Shape.Sphere);
            pcgModifiers.Add(cavity);
            cavity.initialize(creator);
            Debug.Log($"Cavity {i + 1}: Location({cavity.location}), Size({cavity.size})");
        }

        Erosion erosion = ScriptableObject.CreateInstance<Erosion>();
        erosion.iterations = 1;
        //erosion.linear = false;
        erosion.initialize(creator);
        pcgModifiers.Add(erosion);
        
        Erosion adjErosion = ScriptableObject.CreateInstance<Erosion>();
        adjErosion.iterations = 1;
        adjErosion.adjacentErosion = true;
        adjErosion.initialize(creator);
        pcgModifiers.Add(adjErosion);
        
        ConnectRooms connect = ScriptableObject.CreateInstance<ConnectRooms>();
        connect.SetAdjacencyDictionary(adjacencyDict);
        connect.initialize(creator);
        pcgModifiers.Add(connect);

        BlockCleaning cleaning = ScriptableObject.CreateInstance<BlockCleaning>();
        cleaning.initialize(creator);
        pcgModifiers.Add(cleaning);
        creator.pcgSteps = pcgModifiers.ToArray();

        creator.Generate(); // Generate the voxel structure

        yield break;
    }
}
