using UnityEngine;
using System.Collections.Generic;

public class TextureManager
{
    public BlockType[] blockTypes;
    private List<Texture2D> textures = new List<Texture2D>();
    private Texture2D textureAtlas;

    private int totalTextureSize;
    private Rect[] textureRects;
    private int[] blockTypeIndex;
    private int atlasTextureSize;
    private int atlasSize = 1000;
    
    public void initialize() {
        blockTypeIndex = new int[blockTypes.Length];

        totalTextureSize = 0;

        int index = 0;

        for(int i = 0; i < blockTypes.Length; i++) {
            blockTypes[i].initialize();
            textures.AddRange(new List<Texture2D>(blockTypes[i].getBlockFaceTextures()));
            totalTextureSize += blockTypes[i].textureTotalSize;
            blockTypeIndex[blockTypes[i].id] = index;
            index += blockTypes[i].getBlockFaceTextures().Length;
        }
        
        textureAtlas = new Texture2D(atlasSize, atlasSize);
        textureRects = textureAtlas.PackTextures(textures.ToArray(), 0, atlasSize);
    }

    public Texture2D getTexture() {
        if(textureAtlas == null) initialize();

        return textureAtlas;
    }

    //return the UV rectangle of the block with blockId of the face faceIndex
    public Rect getTextureRectById(int blockId, int faceIndex) {
        return textureRects[blockTypeIndex[blockId] + faceIndex];
    }
}
