using UnityEngine;
using System.Collections.Generic;

public class TextureManager
{
    public BlockType[] blockTypes;
    
    private List<Texture2D> textures;

    public void initialize() {
        for(int i = 0; i < blockTypes.Length; i++) {
            blockTypes[i].initialize();
            textures.AddRange(blockTypes[i].getBlockFaceTextures());
        }
    }
}
