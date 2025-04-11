using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ConnectRooms : ProceduralModifier
{
    [System.Serializable]
    public class SerializableDictionary : Dictionary<int, List<int>> , ISerializationCallbackReceiver
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

            Vector3 roomALocation = voxelContainer.roomLocations[roomA];
            foreach (int roomB in pair.Value)
            {
                if (roomB < 0 || roomB >= voxelContainer.roomLocations.Length)
                {
                    Debug.LogError($"roomB index {roomB} is out of bounds for roomLocations array.");
                    continue;
                }

                if (roomA < roomB) // Avoid duplicate connections
                {
                    Vector3 roomBLocation = voxelContainer.roomLocations[roomB];

                    // Generate a tunnel with randomized intermediate points
                    GenerateTunnel(roomALocation, roomBLocation);
                }
            }
        }
    }

    private void GenerateTunnel(Vector3 start, Vector3 end)
    {
        int numPoints = 3; // Number of intermediate points
        float randomRange = 6f; // Range for randomizing perpendicular coordinates
        List<Vector3> points = new List<Vector3> { start };

        // Calculate equidistant points and randomize their positions
        for (int i = 1; i <= numPoints; i++)
        {
            float t = i / (float)(numPoints + 1);
            Vector3 point = Vector3.Lerp(start, end, t);

            // Randomize perpendicular coordinates
            point.x += Random.Range(-randomRange, randomRange);
            point.y += Random.Range(-randomRange, randomRange);
            point.z += Random.Range(-randomRange, randomRange);

            points.Add(point);
        }

        points.Add(end);

        // Generate tunnels between consecutive points
        for (int i = 0; i < points.Count - 1; i++)
        {
            GenerateTunnelSegment(points[i], points[i + 1]);
        }
    }

    private void GenerateTunnelSegment(Vector3 start, Vector3 end)
    {
        Vector3 direction = (end - start).normalized;
        float distance = Vector3.Distance(start, end);
        int steps = Mathf.CeilToInt(distance);

        for (int i = 0; i <= steps; i++)
        {
            Vector3 currentPosition = Vector3.Lerp(start, end, i / (float)steps);
            CreateTunnelSegment(currentPosition);
        }
    }

    private void CreateTunnelSegment(Vector3 position)
    {
        int tunnelWidth = 2; // Width of the tunnel
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
