using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public class AstronomyAPIResponse
{
    public Data data;
}

[Serializable]
public class Data
{
    public Dates dates;
    public Observer observer;
    public Table table;
}

[Serializable]
public class Dates
{
    public string from;
    public string to;
}

[Serializable]
public class Observer
{
    public Location location;
}

[Serializable]
public class Location
{
    public float longitude;
    public float latitude;
    public float elevation;
}

[Serializable]
public class Table
{
    public string[] header;
    public Row[] rows;
}

[Serializable]
public class Row
{
    public Entry entry;
    public Cell[] cells;
}

[Serializable]
public class Entry
{
    public string id;
    public string name;
}

[Serializable]
public class Cell
{
    public string date;
    public string id;
    public string name;
    public Distance distance;
    public Position position;
    public Constellation constellation;
    public ExtraInfo extraInfo;
}

[Serializable]
public class Distance
{
    public FromEarth fromEarth;
}

[Serializable]
public class FromEarth
{
    public string km; // Distance in kilometers (as string in JSON)
    public string au; // Distance in astronomical units (as string in JSON)
}

[Serializable]
public class Position
{
    public Horizontal horizontal;
    public Equatorial equatorial;
}

[Serializable]
public class Horizontal
{
    public Altitude altitude;
    public Azimuth azimuth;
}

[Serializable]
public class Altitude
{
    public string degrees; // Altitude in degrees (as string in JSON)
    public string literal; // Literal representation of altitude
}

[Serializable]
public class Azimuth
{
    public string degrees; // Azimuth in degrees (as string in JSON)
    public string literal; // Literal representation of azimuth
}

[Serializable]
public class Equatorial
{
    public RightAscension rightAscension;
    public Declination declination;
}

[Serializable]
public class RightAscension
{
    public string hours; // Right Ascension in hours (as string in JSON)
    public string literal; // Literal representation of Right Ascension
}

[Serializable]
public class Declination
{
    public string degrees; // Declination in degrees (as string in JSON)
    public string literal; // Literal representation of Declination
}

[Serializable]
public class Constellation
{
    public string id;
    public string shortName;
    public string name;
}

[Serializable]
public class ExtraInfo
{
    public float elongation;
    public float magnitude;
}

[Serializable]
public class Phase
{
    public float angle;
    public float fraction;
}

public class AstronomyDistanceFetcher : MonoBehaviour
{
    
    public string appId = "30638eab-c664-4f3d-93c6-cb4d7413e0db";
    public string appSecret = "a4a6756c6d6745733dfe64aaad45e2d2d077aa8861e224551b3435ede37cf0ae1ea186b4440db7cb6611874fbfbf261a3edcb898fa594cd53d13138ed622cce3f326839acde86e7eddbc2e1be9d558e81cc69047529a0708b583768e9db1c39806cdc310e7d81af2a0c64403a00d388b";

    private Dictionary<string, float> planetDistances = new Dictionary<string, float>();
    public string planetName;

    void Start()
    {
        StartCoroutine(FetchPlanetDistances());
    }

    IEnumerator FetchPlanetDistances()
    {
        string url = "https://api.astronomyapi.com/api/v2/bodies/positions" +
                     "?latitude=0&longitude=0&elevation=0" +
                     "&from_date=" + DateTime.UtcNow.ToString("yyyy-MM-dd") +
                     "&to_date=" + DateTime.UtcNow.ToString("yyyy-MM-dd") +
                     "&time=" + DateTime.UtcNow.ToString("HH:mm:ss") +
                     "&bodies=mercury,venus,mars,jupiter,saturn,uranus,neptune";

        UnityWebRequest request = UnityWebRequest.Get(url);

        // Dynamically generate the Base64-encoded Authorization header
        string credentials = $"{appId}:{appSecret}";
        string encodedCredentials = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(credentials));
        request.SetRequestHeader("Authorization", "Basic " + encodedCredentials);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;

            AstronomyAPIResponse response = JsonUtility.FromJson<AstronomyAPIResponse>(json);

            if (response.data == null || response.data.table == null || response.data.table.rows == null)
            {
                yield break;
            }

            foreach (Row row in response.data.table.rows)
            {
                foreach (Cell cell in row.cells)
                {
                    if (cell.distance != null && cell.distance.fromEarth != null)
                    {
                        float km = float.Parse(cell.distance.fromEarth.km);
                        planetDistances[cell.name.ToLower()] = km; // Store the distance in kilometers
                    }
                }
            }

            planetName = GetRandomPlanetName(); // Get a random planet name
            Debug.Log("planet name " + planetName);
            DungeonGenerator.Instance.CreateDungeon(); // Call CreateDungeon after fetching distances
        }
        else
        {
            if (request.downloadHandler != null)
            {
                string errorResponse = request.downloadHandler.text;
            }

            var responseHeaders = request.GetResponseHeaders();
            if (responseHeaders != null)
            {
                foreach (var header in responseHeaders)
                {
                    // ...existing code...
                }
            }
        }
    }

    public string GetRandomPlanetName()
    {
        string[] planets = { "mercury", "venus", "mars", "jupiter", "saturn", "uranus", "neptune" };
        int randomIndex = UnityEngine.Random.Range(0, planets.Length);
        return planets[randomIndex];
    }

    public float GetPlanetDistance(string planetName)
    {
        planetName = planetName.ToLower();
        if (planetDistances.TryGetValue(planetName, out float distance))
        {
            return distance / 1000000f;
        }
        else
        {
            return -1f; // Return -1 if the distance is not available
        }
    }
}