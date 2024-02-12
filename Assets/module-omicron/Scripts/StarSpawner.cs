using UnityEngine;
using System.Globalization;

public class StarSpawner : MonoBehaviour
{
    public TextAsset csvFile; // Assign your CSV file in the Unity Inspector
    public GameObject spherePrefab; // Assign your sphere prefab in the Unity Inspector

    // Start is called before the first frame update
    void Start()
    {
        // Check if the prefab and file are assigned
        if (spherePrefab == null || csvFile == null)
        {
            Debug.LogError("Prefab or CSV File is not assigned!");
            return;
        }

        SpawnStars();
    }

    private void SpawnStars()
    {
        string[] lines = csvFile.text.Split('\n');
        NumberFormatInfo format = new NumberFormatInfo();
        format.NegativeSign = "-";
        format.NumberDecimalSeparator = ".";

        // Skip the header line by starting at 1
        for (int i = 1; i < 100000; i++)
        {
            string[] values = lines[i].Split(',');
            if (values.Length < 5) continue; // Skip lines that don't have enough data

            // Using TryParse for safe parsing
            if (float.TryParse(values[2], NumberStyles.Any, format, out float x) &&
                float.TryParse(values[3], NumberStyles.Any, format, out float y) &&
                float.TryParse(values[4], NumberStyles.Any, format, out float z))
            {
                // Instantiate the sphere at the position from the CSV
                Instantiate(spherePrefab, new Vector3(x, y, z), Quaternion.identity);
            }
            else
            {
                Debug.LogWarning("Invalid data at line: " + i);
            }
        }
    }
}