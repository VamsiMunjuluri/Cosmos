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
        
        Dictionary<string, float> spectralSizes = new Dictionary<string, float>()
    {
        { "O", 6.6f }, 
        { "B", 1.8f }, 
        { "A", 1.4f }, 
        { "F", 1.1f }, 
        { "G", 1.0f }, 
        { "K", 0.8f }, 
        { "M", 0.7f }  
    };

        if (spectralSizes.TryGetValue(spectralClass, out float relativeSize))
        {
            
            float baseSize = 1.0f;

           
            return baseSize * Mathf.Log(1 + relativeSize) / 0.1f;
        }
        else
        {
          
            return 1.0f; 
        }
    }

    void DrawConstellation()
    {
        for (int i = 0; i < starPositions.Length; i += 2)
        {
            
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

        
    }


Color GetColorFromSpectralType(string spectralType)
    {
        switch (spectralType.Trim())
        {
            case "O": return new Color(0.5f, 0.5f, 1.0f);
            case "B": return new Color(0.7f, 0.7f, 1.0f); 
            case "A": return new Color(1.0f, 1.0f, 1.0f);
            case "F": return new Color(1.0f, 1.0f, 0.5f); 
            case "G": return new Color(1.0f, 1.0f, 0.2f);
            case "K": return new Color(1.0f, 0.8f, 0.4f); 
            case "M": return new Color(1.0f, 0.5f, 0.0f);
            default: return Color.white;
        }
    }

    float GetBrightnessFromAbsMag(float absMag)
    {
        
        float brightestMag = -10f; 
        float dimmestMag = 16f; 

        
        absMag = Mathf.Clamp(absMag, brightestMag, dimmestMag);

       
        float brightness = 1f - ((absMag - brightestMag) / (dimmestMag - brightestMag));

        
        brightness = Mathf.Pow(brightness, 2.5f);

        return brightness;
    }
    void Start()
    {
        string[] lines = starCSV.text.Split('\n');
        int numParticlesAlive = ps.GetParticles(particleStars);
        Debug.Log("Particles before initialization: " + numParticlesAlive); 

        for (int i = 1; i < maxParticles; i++)
        {
            string[] components = lines[i].Split(',');
            if (components.Length >= 5)
            {
                particleStars[i].position = new Vector3(float.Parse(components[2]) ,
                                                        float.Parse(components[4]) ,
                                                        float.Parse(components[3]) );   
                string spectralType = components[10];
                float starSize = GetSizeFromSpectralClass(spectralType);
                float brightness = GetBrightnessFromAbsMag(float.Parse(components[5]));
                Color starColor = GetColorFromSpectralType(spectralType);
               
                particleStars[i].startLifetime = Mathf.Infinity;
                particleStars[i].startColor = starColor; 
                particleStars[i].startSize = starSize;
            }
            else
            {
                Debug.LogError("Invalid line in CSV: " + lines[i]);
            }
        }

        ps.SetParticles(particleStars, maxParticles);
        numParticlesAlive = ps.GetParticles(particleStars);
        Debug.Log("Particles after initialization: " + numParticlesAlive); 
        DrawConstellation();
    }

}
