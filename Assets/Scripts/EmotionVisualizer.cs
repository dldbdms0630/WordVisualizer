using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using HuggingFace.API;


public class EmotionVisualizer : MonoBehaviour 
{
    public class EmotionGradientSettings
    {
        public Color Color1 { get; }
        public Color Color2 { get; }
        public Color Color3 { get; }
        public float Radius { get; }
        public Vector2 GradientOrigin { get; }
        public float Spread { get; }

        public EmotionGradientSettings(Color color1, Color color2, Color color3, float radius, Vector2 gradientOrigin, float spread)
        {
            Color1 = color1;
            Color2 = color2;
            Color3 = color3;
            Radius = radius;
            GradientOrigin = gradientOrigin;
            Spread = spread;
        }
    }

    public GameObject shapePrefab; // base obj prefab (currently just a cube)
    public Transform canvas; 
    public int scalingFactor = 10; // adjust for controlling output quantity

    public void ProcessTextTone(string inputText) {
        Debug.Log("PLAYER INPUT  :  " + inputText);
        HuggingFaceAPI.TextClassification(inputText, outputClassification => {
            foreach (var classification in outputClassification.classifications) {
                if (classification.score > 0.1f) { // Filter by threshold
                    Debug.Log(classification.label + classification.score);
                    int objectCount = Mathf.RoundToInt(classification.score * scalingFactor);
                    Debug.Log("Here's the obj count for " + classification.label + ": " + objectCount );
                    GenerateObjects(classification.label, objectCount);
                }
            }
        }, error => {
            Debug.Log("Error: " + error);
        });
    }

    private Dictionary<string, EmotionGradientSettings> emotionGradientSettings = new Dictionary<string, EmotionGradientSettings>
    {
        { "admiration", new EmotionGradientSettings(new Color(1f, .9f, .5f), new Color(1f, .46f, .15f), new Color(1f, .3f, .7f), 0.7f, new Vector2(0.5f, 0.5f), 2.0f) },
        
        // need to tweak
        { "amusement", new EmotionGradientSettings(new Color(1f, .6f, .2f), new Color(1f, .5f, .1f), new Color(1f, .4f, 0f), 0.4f, new Vector2(0.5f, 0.6f), 1.2f) },
        { "anger", new EmotionGradientSettings(new Color(0.8f, 0f, 0f), new Color(0.9f, .1f, .1f), new Color(1f, .2f, .2f), 0.3f, new Vector2(0.5f, 0.4f), 0.8f) },
        { "annoyance", new EmotionGradientSettings(new Color(.7f, .4f, .1f), new Color(.8f, .5f, .2f), new Color(.9f, .6f, .3f), 0.4f, new Vector2(0.6f, 0.5f), 1.1f) },
        { "approval", new EmotionGradientSettings(new Color(.5f, 1f, .5f), new Color(.4f, .6f, .9f), new Color(.3f, .8f, .3f), 0.5f, new Vector2(0.4f, 0.5f), 1.0f) },
        { "caring", new EmotionGradientSettings(new Color(1f, .8f, .8f), new Color(1f, .7f, .7f), new Color(1f, .6f, .6f), 0.6f, new Vector2(0.5f, 0.5f), 0.9f) },
        { "confusion", new EmotionGradientSettings(new Color(.5f, .6f, .8f), new Color(.4f, .5f, .7f), new Color(.3f, .4f, .6f), 0.3f, new Vector2(0.55f, 0.55f), 1.3f) },
        
        { "curiosity", new EmotionGradientSettings(new Color(1f, .9f, 0f), new Color(.1f, .4f, .8f), new Color(.85f, 1f, .3f), 0.6f, new Vector2(0.5f, 0.5f), 1.9f) },
        { "desire", new EmotionGradientSettings(new Color(.3f, .05f, .05f), new Color(.8f, .1f, .4f), new Color(1f, 1f, .3f), 0.6f, new Vector2(0.5f, 0.5f), 1.9f) },
        
        // need to tweak
        { "disappointment", new EmotionGradientSettings(new Color(.3f, .5f, .8f), new Color(.2f, .4f, .7f), new Color(.1f, .3f, .6f), 0.5f, new Vector2(0.6f, 0.4f), 1.1f) },
        { "disapproval", new EmotionGradientSettings(new Color(.3f, .3f, .3f), new Color(.2f, .2f, .2f), new Color(.1f, .1f, .1f), 0.4f, new Vector2(0.4f, 0.6f), 0.8f) },
        { "disgust", new EmotionGradientSettings(new Color(.6f, .8f, .2f), new Color(.5f, .7f, .1f), new Color(.4f, .6f, 0f), 0.3f, new Vector2(0.3f, 0.5f), 1.0f) },
        { "embarrassment", new EmotionGradientSettings(new Color(1f, .7f, .6f), new Color(.9f, .6f, .5f), new Color(.8f, .5f, .4f), 0.4f, new Vector2(0.5f, 0.5f), 0.9f) },
        
        // do this
        { "excitement", new EmotionGradientSettings(new Color(1f, 1f, .4f), new Color(1f, .9f, .3f), new Color(1f, .8f, .2f), 0.6f, new Vector2(0.6f, 0.6f), 1.2f) },
        
        // need to tweak
        { "fear", new EmotionGradientSettings(new Color(0.2f, 0.1f, 0.5f), new Color(0.3f, 0.2f, 0.6f), new Color(0.1f, 0.05f, 0.3f), 0.4f, new Vector2(0.5f, 0.5f), 2.0f) },
        { "gratitude", new EmotionGradientSettings(new Color(0.4f, 0.9f, 0.6f), new Color(0.3f, 0.8f, 0.5f), new Color(0.5f, 1.0f, 0.7f), 0.6f, new Vector2(0.5f, 0.6f), 1.5f) },
        { "grief", new EmotionGradientSettings(new Color(0.2f, 0.1f, 0.3f), new Color(0.1f, 0.05f, 0.2f), new Color(0.3f, 0.2f, 0.4f), 0.5f, new Vector2(0.6f, 0.4f), 2.2f) },
        { "joy", new EmotionGradientSettings(new Color(1f, 0.9f, 0.2f), new Color(1f, 1f, 0.4f), new Color(1f, 0.8f, 0.0f), 0.3f, new Vector2(0.4f, 0.6f), 1.8f) },
        
        { "love", new EmotionGradientSettings(new Color(1f, 0.35f, 0.48f), new Color(1f, 0.18f, 0.4f), new Color(1f, 0.78f, 0.77f), 0.38f, new Vector2(0.5f, 0.5f), 1.9f) },

        // do this 
        { "nervousness", new EmotionGradientSettings(new Color(0.6f, 0.5f, 0.8f), new Color(0.7f, 0.6f, 0.9f), new Color(0.5f, 0.4f, 0.7f), 0.5f, new Vector2(0.5f, 0.5f), 2.0f) },
        
        // need to tweak
        { "optimism", new EmotionGradientSettings(new Color(0.6f, 1f, 0.3f), new Color(0.5f, 0.9f, 0.2f), new Color(0.7f, 1f, 0.4f), 0.4f, new Vector2(0.6f, 0.5f), 1.5f) },
        { "pride", new EmotionGradientSettings(new Color(0.8f, 0.7f, 0.1f), new Color(0.9f, 0.8f, 0.2f), new Color(0.7f, 0.6f, 0.0f), 0.6f, new Vector2(0.5f, 0.4f), 1.8f) },
        { "realization", new EmotionGradientSettings(new Color(0.7f, 0.9f, 1f), new Color(0.6f, 0.8f, 0.9f), new Color(0.8f, 1f, 1.0f), 0.5f, new Vector2(0.5f, 0.6f), 1.2f) },
        { "relief", new EmotionGradientSettings(new Color(0.5f, 0.8f, 1f), new Color(0.4f, 0.7f, 0.9f), new Color(0.6f, 0.9f, 1.0f), 0.6f, new Vector2(0.6f, 0.5f), 1.4f) },
        
        { "remorse", new EmotionGradientSettings(new Color(0.5f, 0.4f, 0.6f), new Color(0.5f, 0.55f, 0.7f), new Color(0.1f, 0.1f, 0.2f), 0.455f, new Vector2(0.35f, 0.3f), 1.77f) },
        { "sadness", new EmotionGradientSettings(new Color(0.2f, 0.3f, 0.5f), new Color(0.5f, 0.65f, 0.95f), new Color(0.15f, 0.2f, 0.65f), 0.7f, new Vector2(0.5f, 0.65f), 1.8f) },
        
        // need to tweak
        { "surprise", new EmotionGradientSettings(new Color(1f, 0.5f, 1f), new Color(0.9f, 0.4f, 0.9f), new Color(1.0f, 0.6f, 1.0f), 0.3f, new Vector2(0.6f, 0.4f), 2.2f) },
        
        { "neutral", new EmotionGradientSettings(new Color(0.9f, 0.9f, 0.8f), new Color(0.8f, 0.8f, 0.7f), new Color(0.1f, 0.1f, 0.2f), 0.4f, new Vector2(0.5f, 0.5f), 1.77f) }
    

        
        
        // Add other emotions here...
    };






    private void GenerateObjects(string category, int count) {
        // Color emotionColor = GetEmotionColor(category);

        if (!emotionGradientSettings.TryGetValue(category, out var settings))
        {
            Debug.LogWarning($"No gradient settings found for category: {category}");
            return;
        }


        // below values just kinda hardcoded TWEAK EM AS NEEDED
        float minX = -90;
        float maxX = 90;
        float minY = -40;
        float maxY = 40;
        float fixedZ = 80;


        for (int i = 0; i < count; i++) {
            // random position using bounds
            Vector3 randomPosition = new Vector3(
                Random.Range(minX, maxX), 
                Random.Range(minY, maxY), 
                // Random.Range(fixedZ - 5, fixedZ + 5)
                fixedZ
            );

            // instantiate objects 
            GameObject shape = Instantiate(shapePrefab);
            shape.transform.position = randomPosition;
            shape.transform.localScale = Vector3.one * Random.Range(.7f, 1.8f); // Randomize size
            // shape.transform.localScale = Vector3.one * Random.Range(count / 3f, count / .6f); // Randomize size
            
            var spriteRenderer= shape.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                var material= spriteRenderer.material;
                material.SetColor("_Color1", settings.Color1);
                material.SetColor("_Color2", settings.Color2);
                material.SetColor("_Color3", settings.Color3);
                material.SetFloat("_Radius", settings.Radius);
                material.SetVector("_GradientOrigin", settings.GradientOrigin);
                material.SetFloat("_Spread", settings.Spread);
            }


            shape.AddComponent<MovingObject>();
        }
    }

    // private Color GetEmotionColor(string category) {
    //     var emotionColors = new Dictionary<string, Color> {
    //         { "admiration", new Color(1f, 0.9f, 0.5f) }, // gold/light yellow
    //         { "amusement", new Color(1f, 0.6f, 0.2f) },  // bright orange
    //         { "anger", new Color(0.8f, 0f, 0f) },        // deep red
    //         { "annoyance", new Color(0.7f, 0.4f, 0.1f) },// dull orange or brown
    //         { "approval", new Color(0.5f, 1f, 0.5f) },   // light green
    //         { "caring", new Color(1f, 0.8f, 0.8f) },     // soft pink
    //         { "confusion", new Color(0.5f, 0.6f, 0.8f) },// grayish blue
    //         { "curiosity", new Color(0.3f, 0.9f, 1f) },  // bright cyan
    //         { "desire", new Color(0.8f, 0.1f, 0.4f) },   // deep pink or crimson
    //         { "disappointment", new Color(0.3f, 0.5f, 0.8f) }, // muted blue
    //         { "disapproval", new Color(0.3f, 0.3f, 0.3f) },    // dark gray
    //         { "disgust", new Color(0.6f, 0.8f, 0.2f) },  // sickly green
    //         { "embarrassment", new Color(1f, 0.7f, 0.6f) },    // light peach 
    //         { "excitement", new Color(1f, 1f, 0.4f) },   // bright yellow
    //         { "fear", new Color(0.2f, 0.1f, 0.5f) },     // dark purple
    //         { "gratitude", new Color(0.4f, 0.9f, 0.6f) },// soft green
    //         { "grief", new Color(0.2f, 0.1f, 0.3f) },    // deep purple
    //         { "joy", new Color(1f, 0.9f, 0.2f) },        // bright yellow
    //         { "love", new Color(1f, 0.5f, 0.6f) },       // warm red
    //         { "nervousness", new Color(0.6f, 0.5f, 0.8f) }, // light gray/purple
    //         { "optimism", new Color(0.6f, 1f, 0.3f) },   // bright green/lime
    //         { "pride", new Color(0.8f, 0.7f, 0.1f) },    // some gold color
    //         { "realization", new Color(0.7f, 0.9f, 1f) },// light blue
    //         { "relief", new Color(0.5f, 0.8f, 1f) },     // cool blue 
    //         { "remorse", new Color(0.5f, 0.4f, 0.6f) },  // muted purple
    //         { "sadness", new Color(0.2f, 0.3f, 0.5f) },  // navy blue
    //         { "surprise", new Color(1f, 0.5f, 1f) },     // bright magenta 
    //         { "neutral", new Color(0.9f, 0.9f, 0.8f) }   // soft beige
    //     };

    //     return emotionColors.ContainsKey(category) ? emotionColors[category] : Color.white;
    // }
}
