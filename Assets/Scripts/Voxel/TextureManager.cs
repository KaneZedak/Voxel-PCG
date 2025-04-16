using UnityEngine;
using System.Collections.Generic;

public class TextureManager : MonoBehaviour
{
    public BlockType[] blockTypes;
    public int faceSize = 16;
    public int atlasSize = 1024;
    public float updateInterval = 0.3f;

    private List<Texture2D> textures = new List<Texture2D>();
    private Texture2D textureAtlas;
    private Rect[] textureRects;
    private int[] blockTypeIndex;

    private float timer = 0f;
    private Dictionary<Texture2D, int[,]> grids = new Dictionary<Texture2D, int[,]>();
    private Dictionary<int, int> blockIdToFaceCount = new Dictionary<int, int>();

    private Dictionary<Texture2D, (Color alive, Color dead)> textureColors = new();
    private Dictionary<Texture2D, int> textureRules = new();
    void Start()
    {
        Initialize();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= updateInterval)
        {
            timer = 0f;
            UpdateAllTextures();
        }
    }

    public void Initialize()
    {
        blockTypeIndex = new int[blockTypes.Length];
        textures = new List<Texture2D>();
        grids = new Dictionary<Texture2D, int[,]>();
        textureColors = new Dictionary<Texture2D, (Color alive, Color dead)>();
        textureRules = new Dictionary<Texture2D, int>();

        int index = 0;

        for (int i = 0; i < blockTypes.Length; i++)
        {
            if (blockTypes[i] == null) continue;

            blockTypes[i].Initialize();
            var faceTex = blockTypes[i].getBlockFaceTextures(); 

            for (int faceIndex = 0; faceIndex < faceTex.Length; faceIndex++)
            {
                Texture2D dynamicTex = new Texture2D(faceSize, faceSize, TextureFormat.RGBA32, false);
                dynamicTex.filterMode = FilterMode.Point;
                dynamicTex.wrapMode = TextureWrapMode.Clamp;

                int[,] grid = new int[faceSize, faceSize];
                Color alive = blockTypes[i].aliveColors[faceIndex];
                Color dead = blockTypes[i].deadColors[faceIndex];
                int rule = blockTypes[i].caRules[faceIndex];

                textureColors[dynamicTex] = (alive, dead);
                textureRules[dynamicTex] = rule;
                grids[dynamicTex] = grid;
                

                for (int x = 0; x < faceSize; x++)
                    for (int y = 0; y < faceSize; y++)
                    {
                        grid[x, y] = Random.value > 0.5f ? 1 : 0;
                        dynamicTex.SetPixel(x, y, grid[x, y] == 1 ? Color.green : Color.black);
                    }

                dynamicTex.Apply();
                textures.Add(dynamicTex);
                
            }

            blockTypeIndex[blockTypes[i].id] = index;
            index += 6; 
        }

        textureAtlas = new Texture2D(atlasSize, atlasSize, TextureFormat.RGBA32, false, false);
        textureAtlas.filterMode = FilterMode.Point;
        textureAtlas.wrapMode = TextureWrapMode.Clamp;

      
        textureRects = new Rect[textures.Count];
        int cols = atlasSize / faceSize;

        for (int i = 0; i < textures.Count; i++)
        {
            Texture2D tex = textures[i];
            int x = (i % cols) * faceSize;
            int y = (i / cols) * faceSize;

            textureAtlas.SetPixels(x, y, faceSize, faceSize, tex.GetPixels());

            textureRects[i] = new Rect(
                (float)x / atlasSize,
                (float)y / atlasSize,
                (float)faceSize / atlasSize,
                (float)faceSize / atlasSize
            );
        }

        textureAtlas.Apply(false, false);
    }

    public Texture2D GetTexture()
    {
        if (textureAtlas == null)
            Initialize();

        return textureAtlas;
    }

    public Rect GetTextureRectById(int blockId, int faceIndex)
    {
        return textureRects[blockTypeIndex[blockId] + faceIndex];
    }

    void UpdateAllTextures()
    {
        if (grids == null || textures == null || textureRects == null)
        {
            Debug.LogWarning("UpdateAllTextures: Not initialized properly.");
            return;
        }

        int rectCount = textureRects.Length;
        int texCount = textures.Count;

        for (int i = 0; i < texCount; i++)
        {
            Texture2D tex = textures[i];
            if (tex == null) continue;
            if (!grids.ContainsKey(tex)) continue;

            int[,] grid = grids[tex];
            StepCA(grid, tex, GetRuleForTexture(i));
            tex.Apply();
        }

       
        for (int i = 0; i < texCount && i < rectCount; i++)
        {
            Texture2D tex = textures[i];
            if (tex == null) continue;

            Rect rect = textureRects[i];
            int x = Mathf.RoundToInt(rect.x * atlasSize);
            int y = Mathf.RoundToInt(rect.y * atlasSize);

            Color[] pixels = tex.GetPixels();
            textureAtlas.SetPixels(x, y, faceSize, faceSize, pixels);
        }

        textureAtlas.Apply(false, false); 
    }

    void StepCA(int[,] grid, Texture2D tex, int _)
    {
        int size = grid.GetLength(0);
        int[,] next = new int[size, size];

        if (!textureRules.TryGetValue(tex, out int rule)) rule = 0;
        float t = Time.time;

        for (int x = 0; x < size; x++)
            for (int y = 0; y < size; y++)
            {
                int alive = CountAlive(grid, x, y);
                int current = grid[x, y];

                switch (rule)
                {
                    case 0: 
                        int topZone = size * 1 / 5;
                        int bottomZone = size * 4 / 5;

                        if (y >= topZone)
                        {
                            if (current == 1 && Random.value > 0.60f) next[x, y] = 0;
                            else if (current == 0 && Random.value > 0.90f) next[x, y] = 1;
                            else next[x, y] = current;
                        }
                        else if (y < bottomZone)
                        {
                            if (current == 1 && Random.value > 0.90f) next[x, y] = 0;
                            else if (current == 0 && Random.value > 0.60f) next[x, y] = 1;
                            else next[x, y] = current;
                        }
                        else
                        {
                            next[x, y] = (Random.value > 0.5f) ? 1 : 0;
                        }
                        break;

                    case 1: 
                        next[x, y] = (alive + Random.Range(0, 2)) % 2;
                        break;

                    case 2: // Venus 
                        if (current == 0 && Random.value > 0.995f)
                            next[x, y] = 1;
                        else if (current == 0 && alive >= 3 && Random.value > 0.9f)
                            next[x, y] = 1;
                        else if (current == 1 && Random.value < 0.1f)
                            next[x, y] = 0;
                        else
                            next[x, y] = current;
                        break;

                    case 3: // Mercury 
                        float pulse = Mathf.Sin(t + (x * 0.15f) + (y * 0.1f)); 

                        bool basePattern = ((x + y) % 6 == 0); 
                        bool offsetWave = Mathf.PerlinNoise(x * 0.1f, y * 0.1f + t * 0.2f) > 0.5f;

                        if (pulse > 0.6f && (basePattern || offsetWave))
                            next[x, y] = 1;
                        else if (Random.value > 0.985f) 
                            next[x, y] = 1;
                        else if (grid[x, y] == 1 && Random.value > 0.1f) 
                            next[x, y] = 1;
                        else
                            next[x, y] = 0;
                        break;

                    case 4: // Jupiter 
                        float centerX = size / 2f;
                        float centerY = size / 2f;
                        float dx = x - centerX;
                        float dy = y - centerY;
                        float dist = Mathf.Sqrt(dx * dx + dy * dy);
                        float vortex = Mathf.Sin(dist * 0.5f - t * 1.5f);
                        float band = Mathf.Sin(y * 0.3f + t * 2f);
                        bool noiseSpike = Mathf.PerlinNoise(x * 0.1f, y * 0.1f + t) > 0.6f;

                        if ((vortex > 0.5f && band > 0.2f) || noiseSpike)
                            next[x, y] = 1;
                        else if (current == 1 && Random.value > 0.1f)
                            next[x, y] = 1;
                        else if (current == 0 && alive >= 3 && Random.value > 0.95f)
                            next[x, y] = 1;
                        else
                            next[x, y] = 0;
                        break;

                    case 5: // Saturn 
                        int bandIndex = (y / 4) % 2;
                        next[x, y] = Mathf.Sin(t * 2f + bandIndex * Mathf.PI) > 0 ? 1 : 0;
                        break;

                    case 6: // Neptune 
                        if (current == 1 && alive < 1) next[x, y] = 0;
                        else if (current == 0 && alive > 3) next[x, y] = 1;
                        else if (current == 0 && Random.value > 0.999f) next[x, y] = 1;
                        else next[x, y] = current;
                        break;

                    case 7: // Uranus 
                        if (current == 0 && alive >= 2 && Random.value > 0.97f)
                            next[x, y] = 1;
                        else if (current == 1 && alive < 1 && Random.value > 0.9f)
                            next[x, y] = 0;
                        else if (current == 0 && Random.value > 0.999f)
                            next[x, y] = 1;
                        else
                            next[x, y] = current;
                        break;

                    default:
                        next[x, y] = current;
                        break;
                }
            }

        var colors = textureColors[tex];

        for (int x = 0; x < size; x++)
            for (int y = 0; y < size; y++)
            {
                grid[x, y] = next[x, y];
                tex.SetPixel(x, y, grid[x, y] == 1 ? colors.alive : colors.dead);
            }
    }


    int CountAlive(int[,] grid, int x, int y)
    {
        int count = 0;
        int size = grid.GetLength(0);
        for (int dx = -1; dx <= 1; dx++)
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;
                int nx = (x + dx + size) % size;
                int ny = (y + dy + size) % size;
                count += grid[nx, ny];
            }
        return count;
    }

    int GetRuleForTexture(int index)
    {
        return 4; //
    }
}