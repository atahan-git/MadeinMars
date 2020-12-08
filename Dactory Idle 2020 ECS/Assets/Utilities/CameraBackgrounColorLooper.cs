using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Does what it says, for the main screen.
/// Mostly a placeholder.
/// </summary>
public class CameraBackgrounColorLooper : MonoBehaviour {

    public Camera target;

    public float h = 0;
    public float hdelta = 20f;
    [Range(0, 1)] public float s = 0.1f;
    [Range(0, 1)] public float v = 0.8f;

    void Update() {
        HsvToRgb(h, s, v, out var r, out var g, out var b);
        target.backgroundColor = new Color(r / 255f, g / 255f, b / 255f);

        h += hdelta * Time.deltaTime;
    }


    void HsvToRgb(float h, float S, float V, out int r, out int g, out int b) {
        float H = h;
        while (H < 0) {
            H += 360;
        }

        ;
        while (H >= 360) {
            H -= 360;
        }

        ;
        float R, G, B;
        if (V <= 0) {
            R = G = B = 0;
        } else if (S <= 0) {
            R = G = B = V;
        } else {
            float hf = H / 60.0f;
            int i = (int) Mathf.Floor(hf);
            float f = hf - i;
            float pv = V * (1 - S);
            float qv = V * (1 - S * f);
            float tv = V * (1 - S * (1 - f));
            switch (i) {

                // Red is the dominant color

                case 0:
                    R = V;
                    G = tv;
                    B = pv;
                    break;

                // Green is the dominant color

                case 1:
                    R = qv;
                    G = V;
                    B = pv;
                    break;
                case 2:
                    R = pv;
                    G = V;
                    B = tv;
                    break;

                // Blue is the dominant color

                case 3:
                    R = pv;
                    G = qv;
                    B = V;
                    break;
                case 4:
                    R = tv;
                    G = pv;
                    B = V;
                    break;

                // Red is the dominant color

                case 5:
                    R = V;
                    G = pv;
                    B = qv;
                    break;

                // Just in case we overshoot on our math by a little, we put these here. Since its a switch it won't slow us down at all to put these here.

                case 6:
                    R = V;
                    G = tv;
                    B = pv;
                    break;
                case -1:
                    R = V;
                    G = pv;
                    B = qv;
                    break;

                // The color is not defined, we should throw an error.

                default:
                    //LFATAL("i Value error in Pixel conversion, Value is %d", i);
                    R = G = B = V; // Just pretend its black/white
                    break;
            }
        }

        r = Clamp((int) (R * 255.0));
        g = Clamp((int) (G * 255.0));
        b = Clamp((int) (B * 255.0));
    }

    /// <summary>
    /// Clamp a value to 0-255
    /// </summary>
    int Clamp(int i) {
        if (i < 0) return 0;
        if (i > 255) return 255;
        return i;
    }
}
