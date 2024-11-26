using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using HuggingFace.API;


public class EmotionVisualizer : MonoBehaviour 
{
    public GameObject shapePrefab; // base obj prefab (currently just a cube)
    public Transform canvas; 
    public int scalingFactor = 70; // adjust for controlling output quantity

    public void ProcessTextTone(string inputText) {
        Debug.Log("PLAYER INPUT  :  " + inputText);
        HuggingFaceAPI.TextClassification(inputText, outputClassification => {
            foreach (var classification in outputClassification.classifications) {
                if (classification.score > 0.1f) { // Filter by threshold
                    Debug.Log(classification.label + classification.score);
                    int objectCount = Mathf.RoundToInt(classification.score * scalingFactor);
                    GenerateObjects(classification.label, objectCount);
                }
            }
        }, error => {
            Debug.Log("Error: " + error);
        });
    }

    private void GenerateObjects(string category, int count) {
        Color emotionColor = GetEmotionColor(category);

        float minX = 70;
        float maxX = 730;
        float minY = 120;
        float maxY = 400;


        for (int i = 0; i < count; i++) {
            // random position using bounds
            Vector3 randomPosition = new Vector3(
                Random.Range(minX, maxX), 
                Random.Range(minY, maxY), 
                0
            );

            // instantiate objects 
            GameObject shape = Instantiate(shapePrefab);
            shape.transform.position = randomPosition;
            shape.transform.localScale = Vector3.one * Random.Range(5f, 30f); // Randomize size
            // shape.transform.localScale = Vector3.one * Random.Range(count / 3f, count / .6f); // Randomize size
            
            var renderer = shape.GetComponent<Renderer>(); // For 3D objects
            if (renderer != null) {
                renderer.material.color = emotionColor; // Set the material's color dynamically
                // Debug.Log("went thru");
            }
            MovingObject(shape); // Add noise-based motion
        }
    }


    private Color GetEmotionColor(string category) {
        var emotionColors = new Dictionary<string, Color> {
            { "admiration", new Color(1f, 0.9f, 0.5f) }, // gold/light yellow
            { "amusement", new Color(1f, 0.6f, 0.2f) },  // bright orange
            { "anger", new Color(0.8f, 0f, 0f) },        // deep red
            { "annoyance", new Color(0.7f, 0.4f, 0.1f) },// dull orange or brown
            { "approval", new Color(0.5f, 1f, 0.5f) },   // light green
            { "caring", new Color(1f, 0.8f, 0.8f) },     // soft pink
            { "confusion", new Color(0.5f, 0.6f, 0.8f) },// grayish blue
            { "curiosity", new Color(0.3f, 0.9f, 1f) },  // bright cyan
            { "desire", new Color(0.8f, 0.1f, 0.4f) },   // deep pink or crimson
            { "disappointment", new Color(0.3f, 0.5f, 0.8f) }, // muted blue
            { "disapproval", new Color(0.3f, 0.3f, 0.3f) },    // dark gray
            { "disgust", new Color(0.6f, 0.8f, 0.2f) },  // sickly green
            { "embarrassment", new Color(1f, 0.7f, 0.6f) },    // light peach 
            { "excitement", new Color(1f, 1f, 0.4f) },   // bright yellow
            { "fear", new Color(0.2f, 0.1f, 0.5f) },     // dark purple
            { "gratitude", new Color(0.4f, 0.9f, 0.6f) },// soft green
            { "grief", new Color(0.2f, 0.1f, 0.3f) },    // deep purple
            { "joy", new Color(1f, 0.9f, 0.2f) },        // bright yellow
            { "love", new Color(1f, 0.5f, 0.6f) },       // warm red
            { "nervousness", new Color(0.6f, 0.5f, 0.8f) }, // light gray/purple
            { "optimism", new Color(0.6f, 1f, 0.3f) },   // bright green/lime
            { "pride", new Color(0.8f, 0.7f, 0.1f) },    // some gold color
            { "realization", new Color(0.7f, 0.9f, 1f) },// light blue
            { "relief", new Color(0.5f, 0.8f, 1f) },     // cool blue 
            { "remorse", new Color(0.5f, 0.4f, 0.6f) },  // muted purple
            { "sadness", new Color(0.2f, 0.3f, 0.5f) },  // navy blue
            { "surprise", new Color(1f, 0.5f, 1f) },     // bright magenta 
            { "neutral", new Color(0.9f, 0.9f, 0.8f) }   // soft beige
        };

        return emotionColors.ContainsKey(category) ? emotionColors[category] : Color.white;
    }

    private void MovingObject(GameObject shape) {
        // perlin noise script to the shape for abstract motion
        // shape.AddComponent<MovingObject>().SetParameters(1f, 0.2f);
    }
}
