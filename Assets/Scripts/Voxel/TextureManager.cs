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

        int index = 0;

        for (int i = 0; i < blockTypes.Length; i++)
        {
            blockTypes[i].Initialize();
            var faceTex = blockTypes[i].getBlockFaceTextures(); // 

            foreach (var _ in faceTex)
            {
                Texture2D dynamicTex = new Texture2D(faceSize, faceSize, TextureFormat.RGBA32, false);
                dynamicTex.filterMode = FilterMode.Point;
                dynamicTex.wrapMode = TextureWrapMode.Clamp;

                int[,] grid = new int[faceSize, faceSize];

                for (int x = 0; x < faceSize; x++)
                    for (int y = 0; y < faceSize; y++)
                    {
                        grid[x, y] = Random.value > 0.5f ? 1 : 0;
                        dynamicTex.SetPixel(x, y, grid[x, y] == 1 ? Color.green : Color.black);
                    }

                dynamicTex.Apply();
                textures.Add(dynamicTex);
                grids[dynamicTex] = grid;
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

    void StepCA(int[,] grid, Texture2D tex, int rule)
    {
        int size = grid.GetLength(0);
        int[,] next = new int[size, size];

        for (int x = 0; x < size; x++)
            for (int y = 0; y < size; y++)
            {
                int alive = CountAlive(grid, x, y);
                int current = grid[x, y];

                switch (rule)
                {
                    case 0: // Game of Life
                        next[x, y] = (current == 1 && (alive == 2 || alive == 3)) || (current == 0 && alive == 3) ? 1 : 0;
                        break;

                    case 1: // Isolated survive
                        next[x, y] = alive == 0 ? 1 : 0;
                        break;

                    case 2: // Spread if touching
                        next[x, y] = alive >= 1 ? 1 : current;
                        break;

                    case 3: // Random noise
                        next[x, y] = Random.value > 0.5f ? 1 : 0;
                        break;

                    case 4: // Chaotic spread 
                        next[x, y] = (alive + Random.Range(0, 2)) % 2;
                        break;

                    default:
                        next[x, y] = current;
                        break;
                }
            }

        for (int x = 0; x < size; x++)
            for (int y = 0; y < size; y++)
            {
                grid[x, y] = next[x, y];
                tex.SetPixel(x, y, grid[x, y] == 1 ? Color.green : Color.black);
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