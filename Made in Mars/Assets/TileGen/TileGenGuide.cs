using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.UI;
public class TileGenGuide : MonoBehaviour
{
    public bool debug = false;

    public int step = 0;

    public RawImage iptImg1;
    public RawImage iptImg2;
    public RawImage optImg;
    public Texture2D inputTexture;
    public GameObject panel0;
    public GameObject panel1;
    public GameObject panel2;
    public GameObject panel3;
    public GameObject errorPopUp;
    public Text errorDescriptionText;
    public GameObject largeTexturePopup;
    public Text largeTextureDescriptionText;

    public InputField iField;

    public RawImage rawImageDone;
    public Text doneDescription;

    public bool safeMode = true;


    int width;
    int height;

    public TileGen tilePacker;

    void Start()
    {
        ResetAll();
    }

    // Update is called once per frame
    void Update()
    {
        if (step == 0)
        {
            if (iptImg1.texture != null)
            {
                inputTexture = (Texture2D)iptImg1.mainTexture;
                errorDescriptionText.text = "";
                width = iptImg1.texture.width;
                height = iptImg1.texture.height;

                if (CheckIfSquare(inputTexture))
                {
                    if (inputTexture.width > 511 || inputTexture.height > 511 && safeMode)
                    {
                        largeTexturePopup.SetActive(true);
                        largeTextureDescriptionText.text = ("Your image's dimensions (" + inputTexture.width.ToString() + "," + inputTexture.height.ToString() + ") are larger than the recommended maximum resolution (512x512). This tool was made with pixel art tilesets as its main focus. Remember, the final tileset will be four times larger than the input base tileset, and a large processing area might make Unity freeze and crash eventually. If you think your computer can handle it and want to proceed at your own risk, disable 'Safe Mode' on the TileGuide script.");
                    }
                    else
                    {
                        iptImg2.texture = inputTexture;
                        panel1.SetActive(false);
                        panel2.SetActive(true);
                        step = 1;
                    }

                }
                else
                {
                    errorPopUp.SetActive(true);
                    errorDescriptionText.text = "Your image is not a square. The width should be the same as the height. Current width is " + width + " and current height is " + height;
                    iptImg1.texture = null;
                    panel1.SetActive(true);
                    panel2.SetActive(false);
                }

            }
        }

    }//Update

    
    [ContextMenu("Generate")]
    public void Generate()
    {
        tilePacker.CreateTilesetFromBaseTexture(inputTexture);
    }

    public void DoneWithTexture()
    {
        //loadingText.text = "Done!";
        //loadingText.color = Color.green;
    }

    bool CheckIfSquare(Texture2D t2d)
    {
        bool b = false;
        if(t2d.width == t2d.height)
        {
            b = true;
        }
        else
        {
            b = false;
        }

        return b;
    }

    public void ResetAll()
    {
        Debug.Log("Resetting...");
        iptImg1.texture = null;
        largeTexturePopup.SetActive(false);
        largeTextureDescriptionText.text = "";
        panel0.SetActive(false);
        panel1.SetActive(true);
        panel2.SetActive(false);
        panel3.SetActive(false);
        errorDescriptionText.text = "";
        errorPopUp.SetActive(false);
        rawImageDone.texture = null;
        doneDescription.text = "";
    }

   

 

   
}
