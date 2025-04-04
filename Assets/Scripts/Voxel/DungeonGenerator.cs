using UnityEngine;
using System.Collections.Generic;

public class DungeonGenerator : MonoBehaviour
{
    public VoxelCreator creator;
    [SerializeField] int numRooms;
    CavityCreator cavity;
    private Dictionary<int, List<int>> adjacencyDict;
    private Dictionary<int, Vector3> cavityLocations;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Add VoxelCreator component to the GameObject
        // Initialize adjacency dictionary
        adjacencyDict = new Dictionary<int, List<int>>();
        cavityLocations = new Dictionary<int, Vector3>();

        // Populate adjacency dictionary
        for (int i = 0; i < numRooms; i++)
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
                if (j != i && Random.value > 0.5f && !adjacencyDict[i].Contains(j))
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
        cavityLocations[0] = new Vector3(50, 50, 50); // First cavity at a constant location

        while (roomQueue.Count > 0)
        {
            int currentRoom = roomQueue.Dequeue();

            foreach (int adjacentRoom in adjacencyDict[currentRoom])
            {
                if (!cavityLocations.ContainsKey(adjacentRoom))
                {
                    // Determine a location for the adjacent room
                    Vector3 referenceLocation = cavityLocations[currentRoom];
                    float distance = Random.Range(15f, 20f);
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
        creator.roomLocations = new Vector3[numRooms];

        // Initialize pcgSteps with numRooms cavities + erosion + connect rooms
        creator.pcgSteps = new ProceduralModifier[numRooms + 2];

        for (int i = 0; i < numRooms; i++)
        {
            Vector3 location = cavityLocations[i];
            creator.roomLocations[i] = location; // Store room locations for use in ConnectRooms
            cavity = ScriptableObject.CreateInstance<CavityCreator>();
            cavity.initializeCavity(location, new Vector3(3, 4, 4), CavityCreator.Shape.Square);
            creator.pcgSteps[i] = cavity;
            creator.pcgSteps[i].initialize(creator);
            Debug.Log($"Cavity {i + 1}: Location({cavity.location}), Size({cavity.size})");
        }

        Erosion erosion = ScriptableObject.CreateInstance<Erosion>();
        erosion.iterations = 2;
        erosion.initialize(creator);
        creator.pcgSteps[numRooms] = erosion;

        ConnectRooms connect = ScriptableObject.CreateInstance<ConnectRooms>();
        connect.SetAdjacencyDictionary(adjacencyDict);
        connect.initialize(creator);
        creator.pcgSteps[numRooms + 1] = connect;

        creator.Generate(); // Generate the voxel structure
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
