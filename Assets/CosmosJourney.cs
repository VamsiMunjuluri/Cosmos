using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CosmosJourney : MonoBehaviour


{
    public ParticleSystem ps;
    ParticleSystem.Particle[] particleStars;
    public TextAsset starCSV;
    public int maxParticles = 100;
    public Material lineMaterial;
    private Vector3[] starPositions = new Vector3[]
    {
        new Vector3(-25.878f, -14.868f, -42.158f),
        new Vector3(-28.132f, -12.6f, -38.552f),
        new Vector3(-28.132f, -12.6f, -38.552f),
        new Vector3(-36.549f, -9.253f, -42.42f),
        new Vector3(-36.549f, -9.253f, -42.42f),
        new Vector3(-16.409f, -6.422f, -15.152f),
        new Vector3(-16.409f, -6.422f, -15.152f),
        new Vector3(-50.079f, -34.061f, -51.89f),
        new Vector3(-50.079f, -34.061f, -51.89f),
        new Vector3(-28.132f, -12.6f, -38.552f)
    };

    // Start is called before the first frame update
    void Awake ()
    {
        ps = GetComponent<ParticleSystem>();
        particleStars = new ParticleSystem.Particle[maxParticles];

        var main = ps.main;
        main.maxParticles = maxParticles;
        



        var em = ps.emission;
        em.enabled = true;
        em.rateOverTime = 0;
        

        em.SetBursts(
            new ParticleSystem.Burst[]
            {
                new ParticleSystem.Burst(0.0f, (short)maxParticles, (short)maxParticles)
               
            });


    }

    float GetSizeFromSpectralClass(string spectralClass)
    {
        // Define relative size for each spectral class based on the image provided.
        // These are example values and should be adjusted based on your visual needs and scene scale.
        Dictionary<string, float> spectralSizes = new Dictionary<string, float>()
    {
        { "O", 6.6f }, // Larger than 6.6 solar radii
        { "B", 1.8f }, // Between 1.8 and 6.6 solar radii
        { "A", 1.4f }, // Between 1.4 and 1.8 solar radii
        { "F", 1.1f }, // Between 1.1 and 1.4 solar radii
        { "G", 1.0f }, // Between 0.9 and 1.1 solar radii, let's take 1 as an average
        { "K", 0.8f }, // Between 0.7 and 0.9 solar radii
        { "M", 0.7f }  // Less than 0.7 solar radii
    };

        if (spectralSizes.TryGetValue(spectralClass, out float relativeSize))
        {
            // Assuming 1 unit in the scene represents the size of the sun
            // Adjust this base size to fit the scale of your scene appropriately.
            float baseSize = 1.0f;

            // Use a logarithmic scale to reduce the disparity in sizes.
            // The constant 0.1f is arbitrary and can be adjusted to suit your needs.
            return baseSize * Mathf.Log(1 + relativeSize) / 0.1f;
        }
        else
        {
           // Debug.LogWarning("Spectral class '" + spectralClass + "' not recognized. Defaulting to base size.");
            return 1.0f; // Default size if spectral class is unknown
        }
    }

    void DrawConstellation()
    {
        for (int i = 0; i < starPositions.Length; i += 2)
        {
            // Make sure we do not exceed the array bounds
            if (i + 1 < starPositions.Length)
            {
                DrawLineBetweenStars(starPositions[i], starPositions[i + 1]);
            }
        }
    }

    void DrawLineBetweenStars(Vector3 start, Vector3 end)
    {
        GameObject lineObj = new GameObject("Constellation Line");
        LineRenderer lineRenderer = lineObj.AddComponent<LineRenderer>();
        lineRenderer.material = lineMaterial;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
        lineRenderer.startWidth = 0.02f;
        lineRenderer.endWidth = 0.02f;

        // Optional: If you want the line to always face the camera (like a billboard),
        // you can add the following line:
        // lineRenderer.alignment = LineAlignment.View;
    }


Color GetColorFromSpectralType(string spectralType)
    {
        switch (spectralType.Trim())
        {
            case "O": return new Color(0.5f, 0.5f, 1.0f);
            case "B": return new Color(0.7f, 0.7f, 1.0f); // Blue-white
            case "A": return new Color(1.0f, 1.0f, 1.0f);
            case "F": return new Color(1.0f, 1.0f, 0.5f); // Yellow-white
            case "G": return new Color(1.0f, 1.0f, 0.2f);
            case "K": return new Color(1.0f, 0.8f, 0.4f); // Orange
            case "M": return new Color(1.0f, 0.5f, 0.0f);
            default: return Color.white; // Default color if spectral type is unknown
        }
    }

    float GetBrightnessFromAbsMag(float absMag)
    {
        // Define the range of absolute magnitudes visible in your scene.
        // These are example values and should be adjusted based on your visual needs.
        float brightestMag = -10f; // Brightest star (Sirius)
        float dimmestMag = 16f; // Adjust this based on the dimmest star you want to be visible

        // Clamp the absolute magnitude to the range we're working with
        absMag = Mathf.Clamp(absMag, brightestMag, dimmestMag);

        // Convert the magnitude to a linear scale between 0 and 1, where 0 is the dimmest and 1 is the brightest.
        // This formula can be adjusted based on how you want the scale to work visually.
        float brightness = 1f - ((absMag - brightestMag) / (dimmestMag - brightestMag));

        // Since the scale is logarithmic, we might want to apply a power to simulate this effect
        // You can adjust the power to control how the brightness scales.
        brightness = Mathf.Pow(brightness, 2.5f);

        return brightness;
    }
    void Start()
    {
        string[] lines = starCSV.text.Split('\n');
        int numParticlesAlive = ps.GetParticles(particleStars);
        Debug.Log("Particles before initialization: " + numParticlesAlive); // Should be 0

        for (int i = 1; i < maxParticles; i++)
        {
            string[] components = lines[i].Split(',');
            if (components.Length >= 5)
            {
                particleStars[i].position = new Vector3(float.Parse(components[2]) ,
                                                        float.Parse(components[4]) ,
                                                        float.Parse(components[3]) );    // tried dividing by / (float)3.086)but it was very blurry
                string spectralType = components[10];
                float starSize = GetSizeFromSpectralClass(spectralType);
                float brightness = GetBrightnessFromAbsMag(float.Parse(components[5]));
                Color starColor = GetColorFromSpectralType(spectralType);
               // starColor *= brightness;
               // particleStars[i].startSize = 1.0f; // Make sure the size is large enough to be visible.
                particleStars[i].startLifetime = Mathf.Infinity;
                particleStars[i].startColor = starColor; // Use a color that will be visible.
                particleStars[i].startSize = starSize;
            }
            else
            {
                Debug.LogError("Invalid line in CSV: " + lines[i]);
            }
        }

        ps.SetParticles(particleStars, maxParticles);
        numParticlesAlive = ps.GetParticles(particleStars);
        Debug.Log("Particles after initialization: " + numParticlesAlive); // Should be maxParticles
        DrawConstellation();
    }

}
