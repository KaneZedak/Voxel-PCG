using UnityEngine;

public class VoxelFaceTextureDynamic : MonoBehaviour
{
    public Renderer topRenderer, bottomRenderer, frontRenderer, backRenderer, leftRenderer, rightRenderer;
    public int size = 16;
    public float updateInterval = 0.3f;

    Texture2D topTex, bottomTex, frontTex, backTex, leftTex, rightTex;
    float timer = 0f;

    void Start()
    {
        topTex = CreateTex(Color.green, Color.black);
        bottomTex = CreateTex(new Color(0.4f, 0.2f, 0.1f), Color.black);
        frontTex = CreateTex(Color.Lerp(Color.green, Color.gray, 0.5f), Color.black);
        backTex = CreateTex(Color.Lerp(Color.green, Color.gray, 0.5f), Color.black);
        leftTex = CreateTex(Color.Lerp(Color.green, Color.gray, 0.5f), Color.black);
        rightTex = CreateTex(Color.Lerp(Color.green, Color.gray, 0.5f), Color.black);

        topRenderer.material.SetTexture("_BaseMap", topTex);
        bottomRenderer.material.SetTexture("_BaseMap", bottomTex);
        frontRenderer.material.SetTexture("_BaseMap", frontTex);
        backRenderer.material.SetTexture("_BaseMap", backTex);
        leftRenderer.material.SetTexture("_BaseMap", leftTex);
        rightRenderer.material.SetTexture("_BaseMap", rightTex);
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= updateInterval)
        {
            timer = 0f;
            RandomFill(topTex);
            RandomFill(bottomTex);
            RandomFill(frontTex);
            RandomFill(backTex);
            RandomFill(leftTex);
            RandomFill(rightTex);
        }
    }

    Texture2D CreateTex(Color color1, Color color2)
    {
        Texture2D tex = new Texture2D(size, size);
        tex.filterMode = FilterMode.Point;
        tex.wrapMode = TextureWrapMode.Clamp;
        RandomFill(tex, color1, color2);
        return tex;
    }

    void RandomFill(Texture2D tex, Color? on = null, Color? off = null)
    {
        Color c1 = on ?? Color.white;
        Color c0 = off ?? Color.black;

        for (int x = 0; x < size; x++)
            for (int y = 0; y < size; y++)
            {
                tex.SetPixel(x, y, Random.value > 0.5f ? c1 : c0);
            }

        tex.Apply();
    }
}
