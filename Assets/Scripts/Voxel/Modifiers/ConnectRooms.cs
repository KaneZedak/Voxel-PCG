using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ConnectRooms : ProceduralModifier
{
    [System.Serializable]
    public class SerializableDictionary : Dictionary<int, List<int>>, ISerializationCallbackReceiver
    {
        [SerializeField] private List<int> keys = new List<int>();
        [SerializeField] private List<List<int>> values = new List<List<int>>();

        public void OnBeforeSerialize()
        {
            keys.Clear();
            values.Clear();
            foreach (var pair in this)
            {
                keys.Add(pair.Key);
                values.Add(pair.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            this.Clear();
            for (int i = 0; i < keys.Count; i++)
            {
                this[keys[i]] = values[i];
            }
        }
    }

    [SerializeField] private SerializableDictionary adjDict = new SerializableDictionary();

    public SerializableDictionary AdjDict
    {
        get => adjDict;
        set
        {
            adjDict = value;
        }
    }

    public void SetAdjacencyDictionary(Dictionary<int, List<int>> adjacencyDict)
    {
        adjDict.Clear();
        foreach (var pair in adjacencyDict)
        {
            adjDict[pair.Key] = pair.Value;
        }
    }

    private void OnEnable()
    {
        if (adjDict == null)
        {
            return;
        }
    }

    public override void Execute()
    {
        if (adjDict == null || adjDict.Count == 0)
        {
            return;
        }

        foreach (var pair in adjDict)
        {
            int roomA = pair.Key;
            if (roomA < 0 || roomA >= voxelContainer.roomLocations.Length)
            {
                Debug.LogError($"roomA index {roomA} is out of bounds for roomLocations array.");
                continue;
            }

            Vector3 roomALocation = voxelContainer.roomLocations[roomA]; // Use roomLocations for room coordinates
            foreach (int roomB in pair.Value)
            {
                if (roomB < 0 || roomB >= voxelContainer.roomLocations.Length)
                {
                    Debug.LogError($"roomB index {roomB} is out of bounds for roomLocations array.");
                    continue;
                }

                if (roomA < roomB) // Avoid duplicate connections
                {
                    Vector3 roomBLocation = voxelContainer.roomLocations[roomB]; // Use roomLocations for room coordinates

                    // Calculate the direction vector from roomA to roomB
                    Vector3 direction = (roomBLocation - roomALocation).normalized;

                    // Generate a tunnel between the two rooms
                    Vector3 currentPosition = roomALocation;
                    while (Vector3.Distance(currentPosition, roomBLocation) > 1.0f)
                    {
                        CreateTunnelSegment(currentPosition);
                        currentPosition += direction;
                    }
                }
            }
        }
    }

    private void CreateTunnelSegment(Vector3 position)
    {
        int tunnelWidth = 3; // Width of the tunnel
        int halfWidth = tunnelWidth / 2;

        for (int x = -halfWidth; x <= halfWidth; x++)
        {
            for (int y = -halfWidth; y <= halfWidth; y++)
            {
                for (int z = -halfWidth; z <= halfWidth; z++)
                {
                    Vector3 voxelPosition = position + new Vector3(x, y, z);
                    if (voxelContainer.inBound((int)voxelPosition.x, (int)voxelPosition.y, (int)voxelPosition.z))
                    {
                        voxelContainer.setBlock((int)voxelPosition.x, (int)voxelPosition.y, (int)voxelPosition.z, -1); // Set to hollow
                    }
                }
            }
        }
    }
}
