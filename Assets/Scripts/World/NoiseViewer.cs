using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseViewer : MonoBehaviour
{
    private Color[] noisePix;
    private Renderer render;
    [SerializeField]
    private int pixSize = 500;
    [SerializeField]
    private float xOrg, yOrg, zOrg, noiseScale, noiseMultiplier;
    private Texture2D noiseTexture;

    private FastNoise fast = new FastNoise();
    public int Octave;
    public float Amplitude;
    public float Lacunarity;
    public float Frequency;

    public bool draw;

    private void Awake()
    {
        render = GetComponent<Renderer>();

        noiseTexture = new Texture2D(pixSize, pixSize);
        noiseTexture.name = "World perlin noise";
        noiseTexture.filterMode = FilterMode.Point;
        noiseTexture.wrapMode = TextureWrapMode.Clamp;

        noisePix = new Color[pixSize * pixSize];
        render.sharedMaterial.mainTexture = noiseTexture;

        if(draw) StartCoroutine(DrawPlane());
    }

    private IEnumerator DrawPlane()
    {
        fast.SetFractalOctaves(Octave);
        fast.SetFractalLacunarity(Lacunarity);
        fast.SetFrequency(Frequency);
        fast.SetGradientPerturbAmp(Amplitude);

        for (int y = 0; y < noiseTexture.height; y++)
        {
            for (int x = 0; x < noiseTexture.width; x++)
            {
                float sample = fast.GetSimplexFractal(xOrg + x, 0, yOrg + y);
                noisePix[y * noiseTexture.width + x] = new Color(sample, sample, sample);
            }
        }

        noiseTexture.SetPixels(noisePix);
        noiseTexture.Apply();

        yield return new WaitForSeconds(0.25f);
        StartCoroutine(DrawPlane());
        print("Draw");
    }
}
