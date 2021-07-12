using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditorInternal;
using UnityEditor;
using System.IO;
using UnityEngine.UI;


[System.Serializable]
public class SpriteProcessor : AssetPostprocessor
{
    public TileGen tilGen;

    void OnPreprocessTexture()
    {
        Debug.Log("Importing asset to " + assetPath);
        if(assetPath.Contains("itspkr"))
        {
            TextureImporter textureImporter = (TextureImporter)assetImporter;
            textureImporter.textureType = TextureImporterType.Sprite;
            textureImporter.spriteImportMode = SpriteImportMode.Multiple;
            textureImporter.mipmapEnabled = false;
            textureImporter.filterMode = FilterMode.Point;
            textureImporter.spritePixelsPerUnit = TileGen.tileSize;
        }
    }

    public void OnPostprocessTexture(Texture2D texture)
    {
        TileGen tileGen = new TileGen();

        int spriteSize = TileGen.tileSize;
        int count = 8;

        List<SpriteMetaData> metas = new List<SpriteMetaData>();
        List<SpriteMetaData> metasExtras = new List<SpriteMetaData>();
        List<SpriteMetaData> finalMetas = new List<SpriteMetaData>();
        int i = -8;
        for (int y = count; y > 0; --y)
        {
            for (int x = 0; x < count; ++x)
            {
                SpriteMetaData meta = new SpriteMetaData();
                meta.rect = new Rect(x * spriteSize, (y * spriteSize) - spriteSize, spriteSize, spriteSize);
                meta.name = "a";
                i++;

                metas.Add(meta);
            }
        }
        for (int z = metas.Count; z > 0; z--)
        {
                if (z == 63 || z == 62 || z == 61 || z == 60 || z == 59 || z == 58 || z == 57 || z == 56 || z == 55 || z == 54 || z == 53 || z == 47 || z == 39 || z == 31 || z == 23 || z == 15 || z == 7)
                {
                SpriteMetaData tempData = metas[z];
                tempData.name = "other";
                metas[z] = tempData;
                metasExtras.Add(tempData);
                metas.RemoveAt(z);
                }
        }
        for (int u = 0; u < metas.Count; u++)
        {
            SpriteMetaData tempData = metas[u];
            tempData.name = u.ToString();
            finalMetas.Add(tempData);
        }
    
        for (int r = 0; r < metasExtras.Count; r++)
        {
            finalMetas.Add(metasExtras[r]);
        }

        TextureImporter textureImporter = (TextureImporter)assetImporter;
        textureImporter.spritesheet = finalMetas.ToArray();
        tileGen.Refresh();
    }

    public void OnPostprocessSprites(Texture2D texture, Sprite[] sprites)
    {
    }
}//Class SpriteProcessor

public class TileGen : MonoBehaviour
{
    SpriteProcessor sProcessor = new SpriteProcessor();
    public List<Sprite> spriteList;

    public RawImage inImg;
    public RawImage outImg;

    public Texture2D inputTex;
    Texture2D inputTexFixed;
    public static int tileSize;
    int halfTile;
    public Texture2D outputTex;


    Texture2D refNWCorner;
    Texture2D refNECorner;
    Texture2D refSWCorner;
    Texture2D refSECorner;

    Texture2D refNWMiniCorner;
    Texture2D refNEMiniCorner;
    Texture2D refSWMiniCorner;
    Texture2D refSEMiniCorner;

    Texture2D refNWInvCorner;
    Texture2D refNEInvCorner;
    Texture2D refSWInvCorner;
    Texture2D refSEInvCorner;

    Texture2D refNWMiniInvCorner;
    Texture2D refNEMiniInvCorner;
    Texture2D refSWMiniInvCorner;
    Texture2D refSEMiniInvCorner;

    Texture2D refNShore;
    Texture2D refEShore;
    Texture2D refSShore;
    Texture2D refWShore;

    Texture2D refNSBridge;
    Texture2D refWEBridge;

    Texture2D refIntersection;
    Texture2D refIsland;

    public TileGenGuide tileGuide;

    public Texture2D finalTexture;

    string txtrPath;
    string ruleTilePath;

    string playerSetName;
    string fileName;
    int stage;

    void Start()
    {
        sProcessor.tilGen = this.GetComponent<TileGen>();
    }

    #region bigRegion

    public void CreateTilesetFromBaseTexture(Texture2D input)
    {

        if (tileGuide != null)
        {
            outImg = tileGuide.optImg;
        }

        inputTex = input;


        stage = 0;
        tileSize = inputTex.width / 4;
        halfTile = tileSize / 2;
        outputTex = new Texture2D(tileSize * 8, tileSize * 8);
        if (tileGuide != null)
        {
            tileGuide.optImg.texture = outputTex;
        }
        outputTex.filterMode = FilterMode.Point;
        CreateGrid(outputTex, tileSize);
        outImg.texture = outputTex;
        Debug.Log("Process started.");
        Debug.Log("Tile size: " + tileSize + " px");
        Debug.Log("Output image size: " + outputTex.width + " px.");
        Debug.Log("Starting first pass.");

        inputTexFixed = new Texture2D(inputTex.width, inputTex.height);
        inputTex.filterMode = FilterMode.Point;
        SimpleCopyPaste(inputTexFixed, inputTex);

        EraseSquareSectionOfTexture(inputTexFixed, new Vector2(tileSize * 3, 0), tileSize, tileSize);

        //Save this for future upgrades
        //if(biggerCorners)
        //{
        //    int biggerHalfTile = halfTile + (halfTile / 2);
        //    //Top Left Mini Corner
        //    refNWMiniCorner = CopySectionFromTexture(inputTex, new Vector2(0, (tileSize * 3) + (halfTile)), biggerHalfTile, biggerHalfTile);
        //    //Top Right Mini Corner
        //    refNEMiniCorner = CopySectionFromTexture(inputTex, new Vector2((tileSize * 2) + (halfTile), (tileSize * 3) + (halfTile)), biggerHalfTile, biggerHalfTile);
        //    //Bottom Left Mini Corner
        //    refSWMiniCorner = CopySectionFromTexture(inputTex, new Vector2(0, tileSize), biggerHalfTile, biggerHalfTile);
        //    //Bottom Right Mini Corner
        //    refSEMiniCorner = CopySectionFromTexture(inputTex, new Vector2((tileSize * 2) + (halfTile), tileSize), biggerHalfTile, biggerHalfTile);
        //}


        //MINI CORNERS

        //Top Left Mini Corner
        refNWMiniCorner = CopySectionFromTexture(inputTex, new Vector2(0, (tileSize * 3) + (halfTile)), halfTile, halfTile);
        //Top Right Mini Corner
        refNEMiniCorner = CopySectionFromTexture(inputTex, new Vector2((tileSize * 2) + (halfTile), (tileSize * 3) + (halfTile)), halfTile, halfTile);
        //Bottom Left Mini Corner
        refSWMiniCorner = CopySectionFromTexture(inputTex, new Vector2(0, tileSize), halfTile, halfTile);
        //Bottom Right Mini Corner
        refSEMiniCorner = CopySectionFromTexture(inputTex, new Vector2((tileSize * 2) + (halfTile), tileSize), halfTile, halfTile);



        //MINI INV CORNERS
        //Top Left Mini Inv Corner
        refNWMiniInvCorner = CopySectionFromTexture(inputTex, new Vector2(tileSize * 3, halfTile), halfTile, halfTile);
        //Top Right Mini Inv Corner
        refNEMiniInvCorner = CopySectionFromTexture(inputTex, new Vector2((tileSize * 3) + (halfTile), (tileSize * 2) + (halfTile)), halfTile, halfTile);
        //Bottom Left Mini Inv Corner
        refSWMiniInvCorner = CopySectionFromTexture(inputTex, new Vector2((tileSize * 3), tileSize), halfTile, halfTile);
        //Bottom Right Mini Inv Corner
        refSEMiniInvCorner = CopySectionFromTexture(inputTex, new Vector2((tileSize * 3) + (halfTile), tileSize * 3), halfTile, halfTile);

        //CORNERS
        refNWCorner = CopySectionFromTexture(inputTex, new Vector2(0, inputTex.height - tileSize), tileSize, tileSize);
        refNECorner = CopySectionFromTexture(inputTex, new Vector2(inputTex.width - tileSize * 2, inputTex.height - tileSize), tileSize, tileSize);
        refSWCorner = CopySectionFromTexture(inputTex, new Vector2(0, tileSize), tileSize, tileSize);
        refSECorner = CopySectionFromTexture(inputTex, new Vector2(tileSize * 2, tileSize), tileSize, tileSize);

        //INVCORNERS
        refNWInvCorner = CopySectionFromTexture(inputTex, new Vector2(inputTex.width - tileSize, inputTex.height - tileSize), tileSize, tileSize);
        refNEInvCorner = CopySectionFromTexture(inputTex, new Vector2(inputTex.width - tileSize, inputTex.height - tileSize * 2), tileSize, tileSize);
        refSWInvCorner = CopySectionFromTexture(inputTex, new Vector2(inputTex.width - tileSize, inputTex.height - tileSize * 3), tileSize, tileSize);
        refSEInvCorner = CopySectionFromTexture(inputTex, new Vector2(inputTex.width - tileSize, inputTex.height - tileSize * 4), tileSize, tileSize);

        //SHORES
        refNShore = CopySectionFromTexture(inputTex, new Vector2(tileSize, tileSize * 3), tileSize, tileSize);
        refEShore = CopySectionFromTexture(inputTex, new Vector2(tileSize * 2, tileSize * 2), tileSize, tileSize);
        refSShore = CopySectionFromTexture(inputTex, new Vector2(tileSize, tileSize), tileSize, tileSize);
        refWShore = CopySectionFromTexture(inputTex, new Vector2(0, tileSize * 2), tileSize, tileSize);

        //ISLAND
        refIsland = new Texture2D(tileSize, tileSize);
        refIsland.filterMode = FilterMode.Point;
        //TopLeft
        PasteSectionToTexture(refIsland, refNWMiniCorner, new Vector2(0, halfTile));
        //TopRight
        PasteSectionToTexture(refIsland, refNEMiniCorner, new Vector2(halfTile, halfTile));
        //BottomLeft
        PasteSectionToTexture(refIsland, refSWMiniCorner, new Vector2(0, 0));
        //BottomRight
        PasteSectionToTexture(refIsland, refSEMiniCorner, new Vector2(halfTile, 0));



        //INTERSECTION
        refIntersection = new Texture2D(tileSize, tileSize);
        refIntersection.filterMode = FilterMode.Point;
        //TopLeft
        PasteSectionToTexture(refIntersection, refNWMiniInvCorner, new Vector2(0, halfTile));
        //TopRight
        PasteSectionToTexture(refIntersection, refNEMiniInvCorner, new Vector2(halfTile, halfTile));
        //BottomLeft
        PasteSectionToTexture(refIntersection, refSWMiniInvCorner, new Vector2(0, 0));
        //BottomRight
        PasteSectionToTexture(refIntersection, refSEMiniInvCorner, new Vector2(halfTile, 0));




        //BRIDGES
        refNSBridge = new Texture2D(tileSize, tileSize);
        refIntersection.filterMode = FilterMode.Point;
        PasteSectionToTexture(refNSBridge, CopySectionFromTexture(refWShore, new Vector2(0, 0), halfTile, halfTile), new Vector2(0, 0));
        PasteSectionToTexture(refNSBridge, CopySectionFromTexture(refEShore, new Vector2(halfTile, 0), halfTile, halfTile), new Vector2(halfTile, 0));
        PasteSectionToTexture(refNSBridge, CopySectionFromTexture(refWShore, new Vector2(0, halfTile), halfTile, halfTile), new Vector2(0, halfTile));
        PasteSectionToTexture(refNSBridge, CopySectionFromTexture(refEShore, new Vector2(halfTile, halfTile), halfTile, halfTile), new Vector2(halfTile, halfTile));

        refWEBridge = new Texture2D(tileSize, tileSize);
        refIntersection.filterMode = FilterMode.Point;
        PasteSectionToTexture(refWEBridge, CopySectionFromTexture(refNShore, new Vector2(halfTile, halfTile), halfTile, halfTile), new Vector2(halfTile, halfTile));
        PasteSectionToTexture(refWEBridge, CopySectionFromTexture(refSShore, new Vector2(halfTile, 0), halfTile, halfTile), new Vector2(halfTile, 0));
        PasteSectionToTexture(refWEBridge, CopySectionFromTexture(refNShore, new Vector2(0, halfTile), halfTile, halfTile), new Vector2(0, halfTile));
        PasteSectionToTexture(refWEBridge, CopySectionFromTexture(refSShore, new Vector2(0, 0), halfTile, halfTile), new Vector2(0, 0));



        Vector2 testVec = new Vector2(outputTex.width / 2, outputTex.height / 2);

        //Pastes the fixed base tilemap (without the straggler NW invcorner at the bottom)
        PasteSectionToTexture(outputTex, inputTexFixed, new Vector2(0, tileSize * 4));

        //Pastes the remaining inverse corners on the new image
        StartCoroutine(CopySectionFromATetureAndPasteIntoAnotherTextureIE(inputTex, new Vector2(tileSize * 3, 0), tileSize, tileSize * 2, outputTex, new Vector2(outputTex.width + tileSize, outputTex.height + tileSize * 6)));

        //Paste Island
        StartCoroutine(PasteSectionToTextureIE(true, outputTex, refIsland, new Vector2(tileSize * 3, tileSize * 5)));

        //Paste all the NW Corners
        StartCoroutine(PasteSectionToTextureIE(false, outputTex, refNWCorner, new Vector2(tileSize * 5, tileSize * 7)));

        //Paste all the NE Corners
        StartCoroutine(PasteSectionToTextureIE(false, outputTex, refNECorner, new Vector2(tileSize * 6, tileSize * 7)));

        //Paste all the SW Corners
        StartCoroutine(PasteSectionToTextureIE(false, outputTex, refSWCorner, new Vector2(tileSize * 5, tileSize * 6)));

        //Paste all the SE Corners
        StartCoroutine(PasteSectionToTextureIE(false, outputTex, refSECorner, new Vector2(tileSize * 6, tileSize * 6)));

        //Paste all N Shores
        StartCoroutine(PasteSectionToTextureIE(false, outputTex, refNShore, new Vector2(tileSize, tileSize * 2)));
        StartCoroutine(PasteSectionToTextureIE(false, outputTex, refNShore, new Vector2(0, tileSize)));

        //Paste all E Shores
        StartCoroutine(PasteSectionToTextureIE(false, outputTex, refEShore, new Vector2(tileSize * 2, tileSize * 2)));
        StartCoroutine(PasteSectionToTextureIE(false, outputTex, refEShore, new Vector2(tileSize * 4, tileSize * 2)));

        //Paste all S Shores
        StartCoroutine(PasteSectionToTextureIE(false, outputTex, refSShore, new Vector2(tileSize * 3, tileSize * 2)));
        StartCoroutine(PasteSectionToTextureIE(false, outputTex, refSShore, new Vector2(tileSize * 5, tileSize * 2)));

        //Paste all W Shores
        StartCoroutine(PasteSectionToTextureIE(false, outputTex, refWShore, new Vector2(0, tileSize * 2)));
        StartCoroutine(PasteSectionToTextureIE(false, outputTex, refWShore, new Vector2(tileSize * 6, tileSize * 2)));

        //Paste all the NS Bridges
        StartCoroutine(PasteSectionToTextureIE(false, outputTex, refNSBridge, new Vector2(tileSize * 5, tileSize * 5)));
        StartCoroutine(PasteSectionToTextureIE(false, outputTex, refNSBridge, new Vector2(tileSize, tileSize * 4)));
        StartCoroutine(PasteSectionToTextureIE(false, outputTex, refNSBridge, new Vector2(tileSize * 3, tileSize * 4)));

        //Paste all the WE Bridges
        StartCoroutine(PasteSectionToTextureIE(false, outputTex, refWEBridge, new Vector2(tileSize * 6, tileSize * 5)));
        StartCoroutine(PasteSectionToTextureIE(false, outputTex, refWEBridge, new Vector2(0, tileSize * 4)));
        StartCoroutine(PasteSectionToTextureIE(false, outputTex, refWEBridge, new Vector2(tileSize * 2, tileSize * 4)));

        //Paste all the Intersections
        StartCoroutine(PasteSectionToTextureIE(false, outputTex, refIntersection, new Vector2(tileSize * 4, tileSize * 5)));
        //Ends row
        StartCoroutine(PasteSectionToTextureIE(false, outputTex, refIntersection, new Vector2(tileSize * 4, tileSize * 4)));
        StartCoroutine(PasteSectionToTextureIE(false, outputTex, refIntersection, new Vector2(tileSize * 5, tileSize * 4)));
        StartCoroutine(PasteSectionToTextureIE(false, outputTex, refIntersection, new Vector2(tileSize * 6, tileSize * 4)));
        //Ts row
        StartCoroutine(PasteSectionToTextureIE(false, outputTex, refIntersection, new Vector2(0, tileSize * 3)));
        StartCoroutine(PasteSectionToTextureIE(false, outputTex, refIntersection, new Vector2(tileSize, tileSize * 3)));
        StartCoroutine(PasteSectionToTextureIE(false, outputTex, refIntersection, new Vector2(tileSize * 2, tileSize * 3)));
        StartCoroutine(PasteSectionToTextureIE(false, outputTex, refIntersection, new Vector2(tileSize * 3, tileSize * 3)));
        StartCoroutine(PasteSectionToTextureIE(false, outputTex, refIntersection, new Vector2(tileSize * 4, tileSize * 3)));
        StartCoroutine(PasteSectionToTextureIE(false, outputTex, refIntersection, new Vector2(tileSize * 5, tileSize * 3)));
        StartCoroutine(PasteSectionToTextureIE(false, outputTex, refIntersection, new Vector2(tileSize * 6, tileSize * 3)));
        //Fat Ts row
        StartCoroutine(PasteSectionToTextureIE(false, outputTex, refIntersection, new Vector2(tileSize, tileSize)));
        StartCoroutine(PasteSectionToTextureIE(false, outputTex, refIntersection, new Vector2(tileSize * 2, tileSize)));
        StartCoroutine(PasteSectionToTextureIE(false, outputTex, refIntersection, new Vector2(tileSize * 3, tileSize)));
        StartCoroutine(PasteSectionToTextureIE(false, outputTex, refIntersection, new Vector2(tileSize * 4, tileSize)));




        //StartCoroutine(CopySectionFromATetureAndPasteIntoAnotherTextureIE(inputTex, new Vector2(0, tileSize), halfTile, halfTile, outputTex, new Vector2(outputTex.width / 2, outputTex.height / 2)));


        //StartCoroutine(CopySectionFromATetureAndPasteIntoAnotherTextureIE(inputTex, new Vector2(0, tileSize), tileSize/2, tileSize/2, outputTex, new Vector2(outputTex.width / 2, outputTex.height / 2)));
        //topLeftCorner = CopySectionFromTexture(inputTex, new Vector2(0, 0), tileSize, tileSize);

        // StartCoroutine(StartProcess());
    }

    public void SecondPass()
    {
        Debug.Log("Second pass initiated.");
        //ELBOWS
        StartCoroutine(PasteSectionToTextureIE(false, outputTex, refSEMiniInvCorner, new Vector2((tileSize * 5) + halfTile, tileSize * 7)));
        StartCoroutine(PasteSectionToTextureIE(false, outputTex, refSWMiniInvCorner, new Vector2((tileSize * 6), tileSize * 7)));
        StartCoroutine(PasteSectionToTextureIE(false, outputTex, refNEMiniInvCorner, new Vector2((tileSize * 5) + halfTile, tileSize * 6 + halfTile)));
        StartCoroutine(PasteSectionToTextureIE(false, outputTex, refNWMiniInvCorner, new Vector2((tileSize * 6), tileSize * 6 + halfTile)));

        //ENDS
        //W End
        StartCoroutine(PasteSectionToTextureIE(false, outputTex, refNWMiniCorner, new Vector2(0, tileSize * 4 + halfTile)));
        StartCoroutine(PasteSectionToTextureIE(false, outputTex, refSWMiniCorner, new Vector2(0, tileSize * 4)));
        //N End
        StartCoroutine(PasteSectionToTextureIE(false, outputTex, refNWMiniCorner, new Vector2(tileSize, tileSize * 4 + halfTile)));
        StartCoroutine(PasteSectionToTextureIE(false, outputTex, refNEMiniCorner, new Vector2(tileSize + halfTile, tileSize * 4 + halfTile)));
        //E End
        StartCoroutine(PasteSectionToTextureIE(false, outputTex, refNEMiniCorner, new Vector2(tileSize * 2 + halfTile, tileSize * 4 + halfTile)));
        StartCoroutine(PasteSectionToTextureIE(false, outputTex, refSEMiniCorner, new Vector2(tileSize * 2 + halfTile, tileSize * 4)));
        //S End
        StartCoroutine(PasteSectionToTextureIE(false, outputTex, refSWMiniCorner, new Vector2(tileSize * 3, tileSize * 4)));
        StartCoroutine(PasteSectionToTextureIE(false, outputTex, refSEMiniCorner, new Vector2(tileSize * 3 + halfTile, tileSize * 4)));

        //NWFullCorner CopySectionFromTexture(inputTex, new Vector2(tileSize, tileSize * 2 + halfTile), halfTile, halfTile)
        //NEFullCorner CopySectionFromTexture(inputTex, new Vector2(tileSize + halfTile, tileSize * 2 + halfTile), halfTile, halfTile)
        //SWFullCorner CopySectionFromTexture(inputTex, new Vector2(tileSize, tileSize * 2), halfTile, halfTile)
        //SEFullCorner CopySectionFromTexture(inputTex, new Vector2(tileSize + halfTile, tileSize * 2), halfTile, halfTile)

        //DIAGONAL BRIDGES
        //SW NE Bridge (bottom left - top right full)
        //topright
        StartCoroutine(PasteSectionToTextureIE(false, outputTex, CopySectionFromTexture(inputTex, new Vector2(tileSize + halfTile, tileSize * 2 + halfTile), halfTile, halfTile), new Vector2(tileSize * 4 + halfTile, tileSize * 4 + halfTile)));
        //bottomleft
        StartCoroutine(PasteSectionToTextureIE(false, outputTex, CopySectionFromTexture(inputTex, new Vector2(tileSize, tileSize * 2), halfTile, halfTile), new Vector2(tileSize * 4, tileSize * 4)));

        //SW NE Bridge (top left - bottom right full
        //topleft
        StartCoroutine(PasteSectionToTextureIE(false, outputTex, CopySectionFromTexture(inputTex, new Vector2(tileSize, tileSize * 2 + halfTile), halfTile, halfTile), new Vector2(tileSize * 5, tileSize * 4 + halfTile)));
        //bottomright
        StartCoroutine(PasteSectionToTextureIE(false, outputTex, CopySectionFromTexture(inputTex, new Vector2(tileSize + halfTile, tileSize * 2), halfTile, halfTile), new Vector2(tileSize * 5 + halfTile, tileSize * 4)));

        //3INVERSE CORNERS
        StartCoroutine(PasteSectionToTextureIE(false, outputTex, CopySectionFromTexture(inputTex, new Vector2(tileSize, tileSize * 2), halfTile, halfTile), new Vector2(tileSize * 6, tileSize * 4)));
        StartCoroutine(PasteSectionToTextureIE(false, outputTex, CopySectionFromTexture(inputTex, new Vector2(tileSize + halfTile, tileSize * 2 + halfTile), halfTile, halfTile), new Vector2(tileSize * 4 + halfTile, tileSize * 3 + halfTile)));
        StartCoroutine(PasteSectionToTextureIE(false, outputTex, CopySectionFromTexture(inputTex, new Vector2(tileSize + halfTile, tileSize * 2), halfTile, halfTile), new Vector2(tileSize * 5 + halfTile, tileSize * 3)));
        StartCoroutine(PasteSectionToTextureIE(false, outputTex, CopySectionFromTexture(inputTex, new Vector2(tileSize, tileSize * 2 + halfTile), halfTile, halfTile), new Vector2(tileSize * 6, tileSize * 3 + halfTile)));

        //fulltopright CopySectionFromTexture(inputTex, new Vector2(tileSize + halfTile, tileSize * 2 + halfTile), halfTile, halfTile)
        //fullbottomleft CopySectionFromTexture(inputTex, new Vector2(tileSize, tileSize * 2), halfTile, halfTile)
        //fulltopleft CopySectionFromTexture(inputTex, new Vector2(tileSize, tileSize * 2 + halfTile), halfTile, halfTile)
        //fullbottomright CopySectionFromTexture(inputTex, new Vector2(tileSize + halfTile, tileSize * 2), halfTile, halfTile)

        //Ts
        //NT
        StartCoroutine(PasteSectionToTextureIE(false, outputTex, CopySectionFromTexture(inputTex, new Vector2(tileSize, tileSize), halfTile, halfTile), new Vector2(0, tileSize * 3)));
        StartCoroutine(PasteSectionToTextureIE(false, outputTex, CopySectionFromTexture(inputTex, new Vector2(tileSize + halfTile, tileSize), halfTile, halfTile), new Vector2(halfTile, tileSize * 3)));

        //ET
        StartCoroutine(PasteSectionToTextureIE(false, outputTex, CopySectionFromTexture(inputTex, new Vector2(0, tileSize * 2 + halfTile), halfTile, halfTile), new Vector2(tileSize, tileSize * 3 + halfTile)));
        StartCoroutine(PasteSectionToTextureIE(false, outputTex, CopySectionFromTexture(inputTex, new Vector2(0, tileSize * 2), halfTile, halfTile), new Vector2(tileSize, tileSize * 3)));

        //ST
        StartCoroutine(PasteSectionToTextureIE(false, outputTex, CopySectionFromTexture(inputTex, new Vector2(tileSize, tileSize * 3 + halfTile), halfTile, halfTile), new Vector2(tileSize * 2, tileSize * 3 + halfTile)));
        StartCoroutine(PasteSectionToTextureIE(false, outputTex, CopySectionFromTexture(inputTex, new Vector2(tileSize + halfTile, tileSize * 3 + halfTile), halfTile, halfTile), new Vector2(tileSize * 2 + halfTile, tileSize * 3 + halfTile)));

        //ET
        StartCoroutine(PasteSectionToTextureIE(false, outputTex, CopySectionFromTexture(inputTex, new Vector2(tileSize * 2 + halfTile, tileSize * 2 + halfTile), halfTile, halfTile), new Vector2(tileSize * 3 + halfTile, tileSize * 3 + halfTile)));
        StartCoroutine(PasteSectionToTextureIE(false, outputTex, CopySectionFromTexture(inputTex, new Vector2(tileSize * 2 + halfTile, tileSize * 2), halfTile, halfTile), new Vector2(tileSize * 3 + halfTile, tileSize * 3)));

        //N
        //topLeftNShore CopySectionFromTexture(inputTex, new Vector2(tileSize, tileSize * 3 + halfTile), halfTile, halfTile)
        //topRightNShore CopySectionFromTexture(inputTex, new Vector2(tileSize + halfTile, tileSize * 3 + halfTile), halfTile, halfTile)

        //S
        //bottomLeftSShore CopySectionFromTexture(inputTex, new Vector2(tileSize, tileSize), halfTile, halfTile)
        //bottomRightSShore CopySectionFromTexture(inputTex, new Vector2(tileSize + halfTile, tileSize), halfTile, halfTile)

        //W
        //topLeftWShore CopySectionFromTexture(inputTex, new Vector2(0, tileSize * 2 + halfTile), halfTile, halfTile)
        //bottomLeftWShore CopySectionFromTexture(inputTex, new Vector2(0, tileSize * 2), halfTile, halfTile)

        //E
        //topRightEShore CopySectionFromTexture(inputTex, new Vector2(tileSize * 2 + halfTile, tileSize * 2 + halfTile), halfTile, halfTile)
        //bottomRightEShore CopySectionFromTexture(inputTex, new Vector2(tileSize * 2 + halfTile, tileSize * 2), halfTile, halfTile)

        //GUN
        //35 N Gun
        StartCoroutine(PasteSectionToTextureIE(false, outputTex, refNEMiniInvCorner, new Vector2(halfTile, tileSize * 2 + halfTile)));
        //36 E Gun
        StartCoroutine(PasteSectionToTextureIE(false, outputTex, refSEMiniInvCorner, new Vector2(tileSize + halfTile, tileSize * 2)));
        //37 S Gun
        StartCoroutine(PasteSectionToTextureIE(false, outputTex, refSWMiniInvCorner, new Vector2(tileSize * 2, tileSize * 2)));
        //38 W Gun
        StartCoroutine(PasteSectionToTextureIE(false, outputTex, refNWMiniInvCorner, new Vector2(tileSize * 3, tileSize * 2 + halfTile)));

        //INVGUN
        //39 N InvGun
        StartCoroutine(PasteSectionToTextureIE(false, outputTex, refNWMiniInvCorner, new Vector2(tileSize * 4, tileSize * 2 + halfTile)));
        //40 E InvGun
        StartCoroutine(PasteSectionToTextureIE(false, outputTex, refNEMiniInvCorner, new Vector2(tileSize * 5 + halfTile, tileSize * 2 + halfTile)));
        //41 S InvGun
        StartCoroutine(PasteSectionToTextureIE(false, outputTex, refSEMiniInvCorner, new Vector2(tileSize * 6 + halfTile, tileSize * 2)));
        //42 W InvGun
        StartCoroutine(PasteSectionToTextureIE(false, outputTex, refSWMiniInvCorner, new Vector2(0, tileSize)));

        //43 N FatT
        StartCoroutine(PasteSectionToTextureIE(false, outputTex, CopySectionFromTexture(inputTex, new Vector2(tileSize, tileSize * 2), halfTile, halfTile), new Vector2(tileSize, tileSize)));
        StartCoroutine(PasteSectionToTextureIE(false, outputTex, CopySectionFromTexture(inputTex, new Vector2(tileSize + halfTile, tileSize * 2), halfTile, halfTile), new Vector2(tileSize + halfTile, tileSize)));

        //44 E FatT
        StartCoroutine(PasteSectionToTextureIE(false, outputTex, CopySectionFromTexture(inputTex, new Vector2(tileSize, tileSize * 2 + halfTile), halfTile, halfTile), new Vector2(tileSize * 2, tileSize + halfTile)));
        StartCoroutine(PasteSectionToTextureIE(false, outputTex, CopySectionFromTexture(inputTex, new Vector2(tileSize, tileSize * 2), halfTile, halfTile), new Vector2(tileSize * 2, tileSize)));

        //45 S FatT
        StartCoroutine(PasteSectionToTextureIE(false, outputTex, CopySectionFromTexture(inputTex, new Vector2(tileSize, tileSize * 2 + halfTile), halfTile, halfTile), new Vector2(tileSize * 3, tileSize + halfTile)));
        StartCoroutine(PasteSectionToTextureIE(false, outputTex, CopySectionFromTexture(inputTex, new Vector2(tileSize + halfTile, tileSize * 2 + halfTile), halfTile, halfTile), new Vector2(tileSize * 3 + halfTile, tileSize + halfTile)));

        //46 W FatT
        StartCoroutine(PasteSectionToTextureIE(false, outputTex, CopySectionFromTexture(inputTex, new Vector2(tileSize + halfTile, tileSize * 2 + halfTile), halfTile, halfTile), new Vector2(tileSize * 4 + halfTile, tileSize + halfTile)));
        StartCoroutine(PasteSectionToTextureIE(true, outputTex, CopySectionFromTexture(inputTex, new Vector2(tileSize + halfTile, tileSize * 2), halfTile, halfTile), new Vector2(tileSize * 4 + halfTile, tileSize)));

    }

    #endregion

    public void FinishUp()
    {
        int localT = (int)Time.time;
        int localI = Random.Range(100, 999);
        string t = localT.ToString();
        string i = localI.ToString();


        string name = "test";
        if (tileGuide != null)
        {
            if (tileGuide.iField.text != null && tileGuide.iField.text != "")
            {
                string s = tileGuide.iField.text;
                List<string> forbiddenChars = new List<string>();
                forbiddenChars.Add(" ");
                forbiddenChars.Add("!");
                forbiddenChars.Add("\"");
                forbiddenChars.Add("#");
                forbiddenChars.Add("$");
                forbiddenChars.Add("%");
                forbiddenChars.Add("&");
                forbiddenChars.Add("'");
                forbiddenChars.Add("(");
                forbiddenChars.Add(")");
                forbiddenChars.Add("*");
                forbiddenChars.Add("+");
                forbiddenChars.Add(",");
                forbiddenChars.Add("-");
                forbiddenChars.Add(".");
                forbiddenChars.Add("/");
                forbiddenChars.Add(":");
                forbiddenChars.Add(";");
                forbiddenChars.Add("<");
                forbiddenChars.Add("=");
                forbiddenChars.Add(">");
                forbiddenChars.Add("?");
                forbiddenChars.Add("@");
                forbiddenChars.Add("[");
                forbiddenChars.Add("\\");
                forbiddenChars.Add("]");
                forbiddenChars.Add("^");
                forbiddenChars.Add("`");
                forbiddenChars.Add("{");
                forbiddenChars.Add("|");
                forbiddenChars.Add("}");
                forbiddenChars.Add("~");


                for (int z = 0; z < forbiddenChars.Count; z++)
                {
                    name = s + "_";
                    if (s.Contains(forbiddenChars[z]))
                    {
                        name = "Txtr_" + Random.Range(100, 9999).ToString() + "_";
                        break;
                    }
                }
            }
            else
            {
                name = "Txtr_" + Random.Range(100, 9999).ToString() + "_";
            }

        }

        playerSetName = name;

        string fileNamePNG = "itspkr" + t + i + ".png";
        fileName = "itspkr" + t + i;

        string fullPath = "/TileGen/Resources/ExportTilemap/";
        txtrPath = fullPath + playerSetName + fileNamePNG;
        Debug.Log("Success! Exporting image to" + (fullPath + playerSetName + fileNamePNG));
        byte[] bytes = outputTex.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + (fullPath + playerSetName + fileNamePNG), bytes);

        Refresh();
        finalTexture = Resources.Load<Texture2D>("ExportTilemap/" + playerSetName + fileName);
        //finalTexture = Resources.Load<Texture2D>("ExportTilemap/" + playerSetName + fileName);

        SetTexture2DSpriteMeshType(ref finalTexture, SpriteMeshType.FullRect);

        if (tileGuide != null)
        {
            tileGuide.DoneWithTexture();
        }

        Invoke("GetChildTiles", 1);

    }

    public static bool SetTexture2DSpriteMeshType(ref Texture2D texture, SpriteMeshType meshType)
    {
        if (texture == null)
        {
            return false;
        }

        string assetPath = AssetDatabase.GetAssetPath(texture);
        TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;

        if (importer == null)
        {
            return false;
        }

        TextureImporterSettings textureSettings = new TextureImporterSettings();
        importer.ReadTextureSettings(textureSettings);

        if (textureSettings.spriteMeshType != meshType)
        {
            textureSettings.spriteMeshType = meshType;

            importer.SetTextureSettings(textureSettings);
            importer.SaveAndReimport();

            return true;
        }

        return false;
    }

    public void GetChildTiles()
    {
        Debug.Log("Getting child tiles...");
        Sprite[] sprt = Resources.LoadAll<Sprite>("ExportTilemap/" + playerSetName + fileName);
        spriteList.Clear();
        Debug.Log("clearing...");


        for (int i = 0; i < sprt.Length; i++)
        {

            spriteList.Add(sprt[i]);
        }
        Debug.Log("added sprites");

        for (int i = 0; i < spriteList.Count; i++)
        {
            spriteList[i].name = "tile_" + i.ToString();
            Debug.Log("Sprite list " + i + "set: " + spriteList[i].name);
        }
        Debug.Log("renamed sprites");

        if (spriteList == null)
        {
            Debug.LogError("Sprite list null! Not creating rule tile.");
        }
        else
        {
            Debug.Log("[0] = " + spriteList[0].name);
            Debug.Log("[1] = " + spriteList[1].name);
            Debug.Log("[2] = " + spriteList[2].name);

            Debug.Log("Sprite list not null. Proceeding.");

            Invoke("CreateRuleTile", 0.5f);
        }


    }

#if UNITY_EDITOR
    [ContextMenu("Refresh")]
    public void Refresh()
    {
        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
    }

#endif
    [ContextMenu("Save")]
    public void Save()
    {
        AssetDatabase.SaveAssets();
    }

    public void SimpleCopyPaste(Texture2D bottomTex, Texture2D topTex)
    {
        for (int x = 0; x < topTex.width; x++)
        {
            for (int y = 0; y < topTex.height; y++)
            {
                bottomTex.SetPixel(x, y, topTex.GetPixel(x, y));
            }
        }
        bottomTex.Apply();
    }

    public IEnumerator CopySectionFromATetureAndPasteIntoAnotherTextureIE(Texture2D reference, Vector2 refCoord, int width, int height, Texture2D targetTexture, Vector2 targetCoord)
    {
        for (int x = (int)refCoord.x; x < refCoord.x + width; x++)
        {
            for (int y = (int)refCoord.y; y < refCoord.y + height; y++)
            {
                Color col = reference.GetPixel(x, y);
                targetTexture.SetPixel((int)targetCoord.x + x, (int)targetCoord.y + y, col);
                targetTexture.Apply();
            }
            yield return new WaitForEndOfFrame();
        }

    }

    public void CopySectionFromATetureAndPasteIntoAnotherTexture(Texture2D reference, Vector2 refCoord, int width, int height, Texture2D targetTexture, Vector2 targetCoord)
    {
        for (int x = (int)refCoord.x; x < refCoord.x + width; x++)
        {
            for (int y = (int)refCoord.y; y < refCoord.y + height; y++)
            {
                Color col = reference.GetPixel(x, y);
                targetTexture.SetPixel((int)targetCoord.x + x, (int)targetCoord.y + y, col);
                targetTexture.Apply();
            }
        }

    }

    Texture2D CopySectionFromTexture(Texture2D baseTexture,Vector2 coord, int width, int height)
    {
        Texture2D output = new Texture2D(width,height,TextureFormat.ARGB32, false);

        for (int x = (int)coord.x; x < coord.x + width; x++)
        {
            for (int y = (int)coord.y; y < coord.y + height; y++)
            {
                Color col;
                col = baseTexture.GetPixel(x, y);
                output.SetPixel(x, y, col);
            }
        }
        output.Apply();
        return output;

    }

    public void EraseSquareSectionOfTexture(Texture2D bottomTex, Vector2 coord, int width, int height)
    {
        for (int x = (int)coord.x; x < width + (int)coord.x; x++)
        {
            for (int y = (int)coord.y; y < height + (int)coord.y; y++)
            {
                bottomTex.SetPixel(x, y, new Color(0,0,0,0));
            }
        }
        bottomTex.Apply();
    }

    public IEnumerator PasteSectionToTextureIE(bool counting, Texture2D bottomTex, Texture2D topTex, Vector2 coord)
    {
        int pixelAmount = topTex.width * topTex.height;
        int count = 0;

        int topTextureX = 0;
        int topTextureY = 0;
        for (int x = (int)coord.x; x < (int)coord.x + topTex.width; x++)
        {
            for (int y = (int)coord.y; y < (int)coord.y + topTex.width; y++)
            {
                bottomTex.SetPixel(x, y, topTex.GetPixel(topTextureX, topTextureY));
                topTextureY++;
                if (counting)
                {
                    count++;
                }
                if (count >= pixelAmount)
                {
                    if (stage == 0)
                    {
                        stage++;
                        SecondPass();
                    }
                    else
                    {
                       FinishUp();
                    }
                }

            }
            topTextureX++;
            bottomTex.Apply();

            yield return new WaitForEndOfFrame();
        }
    }

    public void PasteSectionToTexture(Texture2D bottomTex, Texture2D topTex, Vector2 coord)
    {
        int topTextureX = 0;
        int topTextureY = 0;
        for (int x = (int)coord.x; x < (int)coord.x + topTex.width; x++)
        {
            for (int y = (int)coord.y; y < (int)coord.y + topTex.width; y++)
            {
                bottomTex.SetPixel(x, y, topTex.GetPixel(topTextureX, topTextureY));
                topTextureY++;
            }
            topTextureX++;
            bottomTex.Apply();
        }
    }

    public void CreateGrid(Texture2D where, int tileSize)
    {
        int currentRow = 0;
        int rowCount = 0;

        int currentColumn = 0;
        int columnCount = 0;

        for (int x = 0; x < where.width; x++)
        {
            for (int y = 0; y < where.height; y++)
            {
                if (y == currentRow * tileSize)
                {
                    where.SetPixel(x, y, Color.grey);
                    rowCount++;
                    if (rowCount > 0)
                    {
                        rowCount = 0;
                        currentRow++;
                        if (currentRow >= 8)
                        {
                            currentRow = 0;
                        }
                    }
                }

                if (x == currentColumn * tileSize)
                {
                    where.SetPixel(x, y, Color.grey);
                    columnCount++;
                    if (columnCount >= where.height)
                    {
                        columnCount = 0;
                        currentColumn++;
                    }
                }


            }
        }
        where.Apply();
    }

     [ContextMenu("Create Rule Tile from Populated Sprite List")]
    public void CreateRuleTile()
    {
        Debug.Log("Creating Rule tile.");
        //Add this and spriteList as arguments to the function later.
        RuleTile rTile = ScriptableObject.CreateInstance("RuleTile") as RuleTile;

        Debug.Log("Test 1");

        AssetDatabase.CreateAsset(rTile, "Assets/TileGen/Resources/ExportTilemap/" + playerSetName + "ruletile.asset");
        Debug.Log("Test a");
        ruleTilePath = "Assets/TileGen/Resources/ExportTilemap/" + playerSetName + "ruletile.asset";
        Debug.Log("Test b");
        rTile.m_DefaultSprite = spriteList[0];

        Debug.Log("Test 2");

        //RULE TILES
        //1 = GREEN
        //2 = RED X
        //0 = EMPTY

        rTile.m_DefaultSprite = spriteList[17];

        Debug.Log("Test 3");

        //RULE r_33 - SE 3Inv
        RuleTile.TilingRule r_33 = new RuleTile.TilingRule();
        r_33.m_Sprites[0] = spriteList[33];
        List<int> r_33_list = new List<int>()
        {
        2,1,2,
        1,  1,
        2,1,1
        };

        Debug.Log("Test 4");

        r_33.m_Neighbors = new List<int>(r_33_list.ToArray());
        rTile.m_TilingRules.Add(r_33);

        Debug.Log("Debug is " + tileGuide.debug);
        if (tileGuide.debug)
        {
            Debug.Log("= r_33 neighbors =");
            Debug.Log(r_33.m_Neighbors);
            Debug.Log("_____________________");

        }

        //RULE r_27 - SW 3Inv
        RuleTile.TilingRule r_27 = new RuleTile.TilingRule();
        r_27.m_Sprites[0] = spriteList[27];
        List<int> r_27_list = new List<int>()
        {
        2,1,2,
        1,  1,
        1,1,2
        };
        r_27.m_Neighbors = new List<int>(r_27_list.ToArray());
        rTile.m_TilingRules.Add(r_27);

        //RULE r_34 - NW 3Inv
        RuleTile.TilingRule r_34 = new RuleTile.TilingRule();
        r_34.m_Sprites[0] = spriteList[34];
        List<int> r_34_list = new List<int>()
        {
        1,1,2,
        1,  1,
        2,1,2
        };
        r_34.m_Neighbors = new List<int>(r_34_list.ToArray());
        rTile.m_TilingRules.Add(r_34);

        //RULE r_32 - NE 3Inv
        RuleTile.TilingRule r_32 = new RuleTile.TilingRule();
        r_32.m_Sprites[0] = spriteList[32];
        List<int> r_32_list = new List<int>()
        {
        2,1,1,
        1,  1,
        2,1,2
        };
        r_32.m_Neighbors = new List<int>(r_32_list.ToArray());
        rTile.m_TilingRules.Add(r_32);

        //RULE r_0 - NW Corner
        RuleTile.TilingRule r_0 = new RuleTile.TilingRule();
        r_0.m_Sprites[0] = spriteList[0];
        List<int> r_0_list = new List<int>()
        {
        0,2,0,
        2,  1,
        0,1,1
        };
        r_0.m_Neighbors = new List<int>(r_0_list.ToArray());
        rTile.m_TilingRules.Add(r_0);

        //RULE r_02 - NE Corner
        RuleTile.TilingRule r_02 = new RuleTile.TilingRule();
        r_02.m_Sprites[0] = spriteList[2];
        List<int> r_02_list = new List<int>()
        {
        0,2,0,
        1,  2,
        1,1,0
        };
        r_02.m_Neighbors = new List<int>(r_02_list.ToArray());
        rTile.m_TilingRules.Add(r_02);

        //RULE r_16 - SE Corner
        RuleTile.TilingRule r_16 = new RuleTile.TilingRule();
        r_16.m_Sprites[0] = spriteList[16];
        List<int> r_16_list = new List<int>()
        {
        1,1,0,
        1,  2,
        0,2,0
        };
        r_16.m_Neighbors = new List<int>(r_16_list.ToArray());
        rTile.m_TilingRules.Add(r_16);

        //RULE r_14 - SW Corner
        RuleTile.TilingRule r_14 = new RuleTile.TilingRule();
        r_14.m_Sprites[0] = spriteList[14];
        List<int> r_14_list = new List<int>()
        {
        0,1,1,
        2,  1,
        0,2,0
        };
        r_14.m_Neighbors = new List<int>(r_14_list.ToArray());
        rTile.m_TilingRules.Add(r_14);

        //RULE r_01 - N Shore
        RuleTile.TilingRule r_01 = new RuleTile.TilingRule();
        r_01.m_Sprites[0] = spriteList[1];
        List<int> r_01_list = new List<int>()
        {
        0,2,0,
        1,  1,
        1,1,1
        };
        r_01.m_Neighbors = new List<int>(r_01_list.ToArray());
        rTile.m_TilingRules.Add(r_01);


        //RULE r_43 - N Fat T
        RuleTile.TilingRule r_43 = new RuleTile.TilingRule();
        r_43.m_Sprites[0] = spriteList[43];
        List<int> r_43_list = new List<int>()
        {
        2,1,2,
        1,  1,
        1,1,1
        };
        r_43.m_Neighbors = new List<int>(r_43_list.ToArray());
        rTile.m_TilingRules.Add(r_43);

        //RULE r_44 - E Fat T 
        RuleTile.TilingRule r_44 = new RuleTile.TilingRule();
        r_44.m_Sprites[0] = spriteList[44];
        List<int> r_44_list = new List<int>()
        {
        1,1,2,
        1,  1,
        1,1,2
        };
        r_44.m_Neighbors = new List<int>(r_44_list.ToArray());
        rTile.m_TilingRules.Add(r_44);

        //RULE r_45 - S Fat T 
        RuleTile.TilingRule r_45 = new RuleTile.TilingRule();
        r_45.m_Sprites[0] = spriteList[45];
        List<int> r_45_list = new List<int>()
        {
        1,1,1,
        1,  1,
        2,1,2
        };
        r_45.m_Neighbors = new List<int>(r_45_list.ToArray());
        rTile.m_TilingRules.Add(r_45);

        //RULE r_46 - W Fat T
        RuleTile.TilingRule r_46 = new RuleTile.TilingRule();
        r_46.m_Sprites[0] = spriteList[46];
        List<int> r_46_list = new List<int>()
        {
        2,1,1,
        1,  1,
        2,1,1
        };
        r_46.m_Neighbors = new List<int>(r_46_list.ToArray());
        rTile.m_TilingRules.Add(r_46);

        //RULE r_03 - NW Invcorner
        RuleTile.TilingRule r_03 = new RuleTile.TilingRule();
        r_03.m_Sprites[0] = spriteList[3];
        List<int> r_03_list = new List<int>()
        {
        1,1,0,
        1,  1,
        0,1,2
        };
        r_03.m_Neighbors = new List<int>(r_03_list.ToArray());
        rTile.m_TilingRules.Add(r_03);

        //RULE r_04 - NE Invcorner
        RuleTile.TilingRule r_04 = new RuleTile.TilingRule();
        r_04.m_Sprites[0] = spriteList[4];
        List<int> r_04_list = new List<int>()
        {
        0,1,1,
        1,  1,
        2,1,0
        };
        r_04.m_Neighbors = new List<int>(r_04_list.ToArray());
        rTile.m_TilingRules.Add(r_04);

        //RULE r_05 - NW Elbow
        RuleTile.TilingRule r_05 = new RuleTile.TilingRule();
        r_05.m_Sprites[0] = spriteList[5];
        List<int> r_05_list = new List<int>()
        {
        0,2,0,
        2,  1,
        0,1,2
        };
        r_05.m_Neighbors = new List<int>(r_05_list.ToArray());
        rTile.m_TilingRules.Add(r_05);

        //RULE r_06 - NE Elbow
        RuleTile.TilingRule r_06 = new RuleTile.TilingRule();
        r_06.m_Sprites[0] = spriteList[6];
        List<int> r_06_list = new List<int>()
        {
        0,2,0,
        1,  2,
        2,1,0
        };
        r_06.m_Neighbors = new List<int>(r_06_list.ToArray());
        rTile.m_TilingRules.Add(r_06);

        //RULE r_07 - W Shore
        RuleTile.TilingRule r_07 = new RuleTile.TilingRule();
        r_07.m_Sprites[0] = spriteList[7];
        List<int> r_07_list = new List<int>()
        {
        0,1,1,
        2,  1,
        0,1,1
        };
        r_07.m_Neighbors = new List<int>(r_07_list.ToArray());
        rTile.m_TilingRules.Add(r_07);

        //RULE r_08 - Middle
        RuleTile.TilingRule r_08 = new RuleTile.TilingRule();
        r_08.m_Sprites[0] = spriteList[8];
        List<int> r_08_list = new List<int>()
        {
        1,1,1,
        1,  1,
        1,1,1
        };
        r_08.m_Neighbors = new List<int>(r_08_list.ToArray());
        rTile.m_TilingRules.Add(r_08);

        //RULE r_09 - E Shore
        RuleTile.TilingRule r_09 = new RuleTile.TilingRule();
        r_09.m_Sprites[0] = spriteList[9];
        List<int> r_09_list = new List<int>()
        {
        1,1,0,
        1,  2,
        1,1,0
        };
        r_09.m_Neighbors = new List<int>(r_09_list.ToArray());
        rTile.m_TilingRules.Add(r_09);

        //RULE r_10 - SW Inv Corner
        RuleTile.TilingRule r_10 = new RuleTile.TilingRule();
        r_10.m_Sprites[0] = spriteList[10];
        List<int> r_10_list = new List<int>()
        {
        0,1,2,
        1,  1,
        1,1,0
        };
        r_10.m_Neighbors = new List<int>(r_10_list.ToArray());
        rTile.m_TilingRules.Add(r_10);

        //RULE r_11 - SE Inv Corner
        RuleTile.TilingRule r_11 = new RuleTile.TilingRule();
        r_11.m_Sprites[0] = spriteList[11];
        List<int> r_11_list = new List<int>()
        {
        2,1,0,
        1,  1,
        0,1,1
        };
        r_11.m_Neighbors = new List<int>(r_11_list.ToArray());
        rTile.m_TilingRules.Add(r_11);

        //RULE r_12 - SW Elbow
        RuleTile.TilingRule r_12 = new RuleTile.TilingRule();
        r_12.m_Sprites[0] = spriteList[12];
        List<int> r_12_list = new List<int>()
        {
        0,1,2,
        2,  1,
        0,2,0
        };
        r_12.m_Neighbors = new List<int>(r_12_list.ToArray());
        rTile.m_TilingRules.Add(r_12);

        //RULE r_13 - SE Elbow
        RuleTile.TilingRule r_13 = new RuleTile.TilingRule();
        r_13.m_Sprites[0] = spriteList[13];
        List<int> r_13_list = new List<int>()
        {
        2,1,0,
        1,  2,
        0,2,0
        };
        r_13.m_Neighbors = new List<int>(r_13_list.ToArray());
        rTile.m_TilingRules.Add(r_13);

        //RULE r_15 - S Shore
        RuleTile.TilingRule r_15 = new RuleTile.TilingRule();
        r_15.m_Sprites[0] = spriteList[15];
        List<int> r_15_list = new List<int>()
        {
        1,1,1,
        1,  1,
        0,2,0
        };
        r_15.m_Neighbors = new List<int>(r_15_list.ToArray());
        rTile.m_TilingRules.Add(r_15);

        //RULE r_17 - Island
        RuleTile.TilingRule r_17 = new RuleTile.TilingRule();
        r_17.m_Sprites[0] = spriteList[17];
        List<int> r_17_list = new List<int>()
        {
        0,2,0,
        2,  2,
        0,2,0
        };
        r_17.m_Neighbors = new List<int>(r_17_list.ToArray());
        rTile.m_TilingRules.Add(r_17);

        //RULE r_18 - Intersection
        RuleTile.TilingRule r_18 = new RuleTile.TilingRule();
        r_18.m_Sprites[0] = spriteList[18];
        List<int> r_18_list = new List<int>()
        {
        2,1,2,
        1,  1,
        2,1,2
        };
        r_18.m_Neighbors = new List<int>(r_18_list.ToArray());
        rTile.m_TilingRules.Add(r_18);

        //RULE r_19 - NS Bridge
        RuleTile.TilingRule r_19 = new RuleTile.TilingRule();
        r_19.m_Sprites[0] = spriteList[19];
        List<int> r_19_list = new List<int>()
        {
        0,1,0,
        2,  2,
        0,1,0
        };
        r_19.m_Neighbors = new List<int>(r_19_list.ToArray());
        rTile.m_TilingRules.Add(r_19);

        //RULE r_20 - WE Bridge
        RuleTile.TilingRule r_20 = new RuleTile.TilingRule();
        r_20.m_Sprites[0] = spriteList[20];
        List<int> r_20_list = new List<int>()
        {
        0,2,0,
        1,  1,
        0,2,0
        };
        r_20.m_Neighbors = new List<int>(r_20_list.ToArray());
        rTile.m_TilingRules.Add(r_20);

        //RULE r_21 - W End
        RuleTile.TilingRule r_21 = new RuleTile.TilingRule();
        r_21.m_Sprites[0] = spriteList[21];
        List<int> r_21_list = new List<int>()
        {
        0,2,0,
        2,  1,
        0,2,0
        };
        r_21.m_Neighbors = new List<int>(r_21_list.ToArray());
        rTile.m_TilingRules.Add(r_21);

        //RULE r_22 - E End
        RuleTile.TilingRule r_22 = new RuleTile.TilingRule();
        r_22.m_Sprites[0] = spriteList[23];
        List<int> r_22_list = new List<int>()
        {
        0,2,0,
        1,  2,
        0,2,0
        };
        r_22.m_Neighbors = new List<int>(r_22_list.ToArray());
        rTile.m_TilingRules.Add(r_22);

        //RULE r_24 - S End
        RuleTile.TilingRule r_24 = new RuleTile.TilingRule();
        r_24.m_Sprites[0] = spriteList[24];
        List<int> r_24_list = new List<int>()
        {
        0,1,0,
        2,  2,
        0,2,0
        };
        r_24.m_Neighbors = new List<int>(r_24_list.ToArray());
        rTile.m_TilingRules.Add(r_24);

        //RULE r_23 - N End 
        RuleTile.TilingRule r_23 = new RuleTile.TilingRule();
        r_23.m_Sprites[0] = spriteList[22];
        List<int> r_23_list = new List<int>()
        {
        0,2,0,
        2,  2,
        0,1,0
        };
        r_23.m_Neighbors = new List<int>(r_23_list.ToArray());
        rTile.m_TilingRules.Add(r_23);


        //RULE r_25 - SW NE Bridge
        RuleTile.TilingRule r_25 = new RuleTile.TilingRule();
        r_25.m_Sprites[0] = spriteList[25];
        List<int> r_25_list = new List<int>()
        {
        2,1,1,
        1,  1,
        1,1,2
        };
        r_25.m_Neighbors = new List<int>(r_25_list.ToArray());
        rTile.m_TilingRules.Add(r_25);

        //RULE r_26 - NW SE Bridge
        RuleTile.TilingRule r_26 = new RuleTile.TilingRule();
        r_26.m_Sprites[0] = spriteList[26];
        List<int> r_26_list = new List<int>()
        {
        1,1,2,
        1,  1,
        2,1,1
        };
        r_26.m_Neighbors = new List<int>(r_26_list.ToArray());
        rTile.m_TilingRules.Add(r_26);

        //RULE r_28 - N T
        RuleTile.TilingRule r_28 = new RuleTile.TilingRule();
        r_28.m_Sprites[0] = spriteList[28];
        List<int> r_28_list = new List<int>()
        {
        2,1,2,
        1,  1,
        0,2,0
        };
        r_28.m_Neighbors = new List<int>(r_28_list.ToArray());
        rTile.m_TilingRules.Add(r_28);

        //RULE r_29 - E T
        RuleTile.TilingRule r_29 = new RuleTile.TilingRule();
        r_29.m_Sprites[0] = spriteList[29];
        List<int> r_29_list = new List<int>()
        {
        0,1,2,
        2,  1,
        0,1,2
        };
        r_29.m_Neighbors = new List<int>(r_29_list.ToArray());
        rTile.m_TilingRules.Add(r_29);

        //RULE r_30 - S T
        RuleTile.TilingRule r_30 = new RuleTile.TilingRule();
        r_30.m_Sprites[0] = spriteList[30];
        List<int> r_30_list = new List<int>()
        {
        0,2,0,
        1,  1,
        2,1,2
        };
        r_30.m_Neighbors = new List<int>(r_30_list.ToArray());
        rTile.m_TilingRules.Add(r_30);

        //RULE r_31 - W T
        RuleTile.TilingRule r_31 = new RuleTile.TilingRule();
        r_31.m_Sprites[0] = spriteList[31];
        List<int> r_31_list = new List<int>()
        {
        2,1,0,
        1,  2,
        2,1,0
        };
        r_31.m_Neighbors = new List<int>(r_31_list.ToArray());
        rTile.m_TilingRules.Add(r_31);

        //RULE r_35 - N GUN
        RuleTile.TilingRule r_35 = new RuleTile.TilingRule();
        r_35.m_Sprites[0] = spriteList[35];
        List<int> r_35_list = new List<int>()
        {
        0,1,2,
        2,  1,
        0,1,1
        };
        r_35.m_Neighbors = new List<int>(r_35_list.ToArray());
        rTile.m_TilingRules.Add(r_35);

        //RULE r_36 - E Gun
        RuleTile.TilingRule r_36 = new RuleTile.TilingRule();
        r_36.m_Sprites[0] = spriteList[36];
        List<int> r_36_list = new List<int>()
        {
        0,2,0,
        1,  1,
        1,1,2
        };
        r_36.m_Neighbors = new List<int>(r_36_list.ToArray());
        rTile.m_TilingRules.Add(r_36);

        //RULE r_37 - S Gun
        RuleTile.TilingRule r_37 = new RuleTile.TilingRule();
        r_37.m_Sprites[0] = spriteList[37];
        List<int> r_37_list = new List<int>()
        {
        1,1,0,
        1,  2,
        2,1,0
        };
        r_37.m_Neighbors = new List<int>(r_37_list.ToArray());
        rTile.m_TilingRules.Add(r_37);

        //RULE r_38 - W Gun
        RuleTile.TilingRule r_38 = new RuleTile.TilingRule();
        r_38.m_Sprites[0] = spriteList[38];
        List<int> r_38_list = new List<int>()
        {
        2,1,1,
        1,  1,
        0,2,0
        };
        r_38.m_Neighbors = new List<int>(r_38_list.ToArray());
        rTile.m_TilingRules.Add(r_38);

        //RULE r_39 - N InvGun
        RuleTile.TilingRule r_39 = new RuleTile.TilingRule();
        r_39.m_Sprites[0] = spriteList[39];
        List<int> r_39_list = new List<int>()
        {
        2,1,0,
        1,  2,
        1,1,0
        };
        r_39.m_Neighbors = new List<int>(r_39_list.ToArray());
        rTile.m_TilingRules.Add(r_39);

        //RULE r_40 - E InvGun
        RuleTile.TilingRule r_40 = new RuleTile.TilingRule();
        r_40.m_Sprites[0] = spriteList[40];
        List<int> r_40_list = new List<int>()
        {
        1,1,2,
        1,  1,
        0,2,0
        };
        r_40.m_Neighbors = new List<int>(r_40_list.ToArray());
        rTile.m_TilingRules.Add(r_40);

        //RULE r_41 - S InvGun
        RuleTile.TilingRule r_41 = new RuleTile.TilingRule();
        r_41.m_Sprites[0] = spriteList[41];
        List<int> r_41_list = new List<int>()
        {
        0,1,1,
        2,  1,
        0,1,2
        };
        r_41.m_Neighbors = new List<int>(r_41_list.ToArray());
        rTile.m_TilingRules.Add(r_41);


        //RULE r_42 - W InvGun
        RuleTile.TilingRule r_42 = new RuleTile.TilingRule();
        r_42.m_Sprites[0] = spriteList[42];
        List<int> r_42_list = new List<int>()
        {
        0,2,0,
        1,  1,
        2,1,1
        };
        r_42.m_Neighbors = new List<int>(r_42_list.ToArray());
        rTile.m_TilingRules.Add(r_42);


        if (tileGuide != null)
        {
            tileGuide.panel3.SetActive(true);
            tileGuide.rawImageDone.texture = outputTex; 
            tileGuide.doneDescription.text = "Done! \n Your tilemap can be found under " + txtrPath + ".\nYour rule tile can be found under " + ruleTilePath + ".\n \n You can safely exit playmode now.";
        }

        //Makes it so that when you leave playmode, the rule tile stays with its settings
        EditorUtility.SetDirty(rTile);
    }
    
}
