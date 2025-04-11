using UnityEngine;

public class VoxelPackedTexture : MonoBehaviour
{
    public int faceSize = 16;
    private int atlasCols = 2;
    private int atlasRows = 3;
    private int atlasSize => faceSize * atlasCols;

    private Texture2D atlasTexture;
    private Material mat;
    private float timer = 0f;
    public float updateInterval = 0.3f;

    void Start()
    {
        mat = GetComponent<MeshRenderer>().material;

        atlasTexture = new Texture2D(faceSize * atlasCols, faceSize * atlasRows, TextureFormat.RGBA32, false);
        atlasTexture.filterMode = FilterMode.Point;
        atlasTexture.wrapMode = TextureWrapMode.Clamp;

        mat.SetTexture("_BaseMap", atlasTexture);

        FillAllFaces();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= updateInterval)
        {
            timer = 0f;
            FillAllFaces();
        }
    }

    void FillAllFaces()
    {
        FillFace(0, 2, Color.gray, Color.black); // Top
        FillFace(1, 2, new Color(0.4f, 0.2f, 0.1f), Color.black); // Bottom
        FillFace(0, 1, Color.gray, Color.black); // Front
        FillFace(1, 1, Color.gray, Color.black); // Back
        FillFace(0, 0, Color.gray, Color.black); // Left
        FillFace(1, 0, Color.gray, Color.black); // Right

        atlasTexture.Apply();
    }

    void FillFace(int col, int row, Color alive, Color dead)
    {
        int x0 = col * faceSize;
        int y0 = row * faceSize;

        for (int x = 0; x < faceSize; x++)
            for (int y = 0; y < faceSize; y++)
            {
                Color c = Random.value > 0.5f ? alive : dead;
                atlasTexture.SetPixel(x0 + x, y0 + y, c);
            }
    }
}
