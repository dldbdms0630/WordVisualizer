using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextNoise : MonoBehaviour
{
    TMP_Text textMesh;

    void Start()
    {
        textMesh = GetComponent<TMP_Text>();
    }

    void Update()
    {
        // Force mesh update for TextMeshPro UGUI
        textMesh.ForceMeshUpdate(true, true);
        var textInfo = textMesh.textInfo;

        // Apply noise to each word
        for (int i = 0; i < textInfo.wordCount; i++)
        {
            var wordInfo = textInfo.wordInfo[i];
            Vector3 offset = Wobble(Time.time + i);
            // Debug.Log($"Offset for word {i}: {offset}");


            for (int j = 0; j < wordInfo.characterCount; j++)
            {
                int charIndex = wordInfo.firstCharacterIndex + j;
                var charInfo = textInfo.characterInfo[charIndex];

                if (!charInfo.isVisible)
                    continue;

                int vertexIndex = charInfo.vertexIndex;

                for (int k = 0; k < 4; k++)
                {
                    textInfo.meshInfo[charInfo.materialReferenceIndex].vertices[vertexIndex + k] += offset;
                }
            }
        }

        // Update the mesh with the modified vertices
        textMesh.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
    }

    Vector3 Wobble(float time)
    {
        float amplitude = 5f; // Adjust for visible motion
        float frequency = 0.3f; // Adjust for speed of motion
        return amplitude * new Vector3(
            Mathf.PerlinNoise(time * frequency, 0.0f),
            Mathf.PerlinNoise(0.0f, time * frequency),
            0.0f // No z-axis movement
        );
    }
}



// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using TMPro;

// public class TextNoise : MonoBehaviour
// {
//     TMP_Text textMesh;
//     Mesh mesh;
//     Vector3[] vertices;

//     void Start()
//     {
//         textMesh = GetComponent<TMP_Text>();
//         // StartCoroutine(name);
//     }

//     void Update() {

//         textMesh.ForceMeshUpdate(true, true);
//         var textInfo = textMesh.textInfo;
//         // var vertices = textInfo.meshInfo[0].vertices; INDIVIDUAL

//         /* start of word-based noise */
//         for (int i = 0; i < textInfo.wordCount; i++)
//         {
//             var wordInfo = textInfo.wordInfo[i];
//             Vector3 offset = Wobble(Time.time + i);

//             for (int j = 0; j < wordInfo.characterCount; j++)
//             {
//                 int charIndex = wordInfo.firstCharacterIndex + j;
//                 var charInfo = textInfo.characterInfo[charIndex];

//                 if (!charInfo.isVisible)
//                     continue;

//                 int vertexIndex = charInfo.vertexIndex;

//                 for (int k = 0; k < 4; k++)
//                 {
//                     textInfo.meshInfo[charInfo.materialReferenceIndex].vertices[vertexIndex + k] += offset;
//                 }
//             }

//             // Check for punctuation marks following the word
//         if (wordInfo.lastCharacterIndex + 1 < textInfo.characterCount)
//         {
//             var nextCharInfo = textInfo.characterInfo[wordInfo.lastCharacterIndex + 1];
//             if (nextCharInfo.character == '.' || nextCharInfo.character == ',' || nextCharInfo.character == '!' 
//                 || nextCharInfo.character == '?' || nextCharInfo.character == '—' || nextCharInfo.character == ':'
//                 || nextCharInfo.character == '\'')
//             {
//                 int vertexIndex = nextCharInfo.vertexIndex;

//                 for (int k = 0; k < 4; k++)
//                     textInfo.meshInfo[nextCharInfo.materialReferenceIndex].vertices[vertexIndex + k] += offset;
//             }
//             else if (nextCharInfo.character == '\'')
//             {
//                 int nextWordIndex = wordInfo.lastCharacterIndex + 1;
//                 while (nextWordIndex < textInfo.characterCount && textInfo.characterInfo[nextWordIndex].character != ' ' && textInfo.characterInfo[nextWordIndex].character != '\n')
//                 {
//                     var followingCharInfo = textInfo.characterInfo[nextWordIndex];
//                     int vertexIndex = followingCharInfo.vertexIndex;

//                     for (int k = 0; k < 4; k++)
//                         {
//                             textInfo.meshInfo[followingCharInfo.materialReferenceIndex].vertices[vertexIndex + k] += offset;
//                         }

//                         nextWordIndex++;
//                     }
//                 }
//             }
//         }

//         // INDIVIDUAL
//         // for (int i = 0; i < vertices.Length; i++)
//         // {
//         //     Vector3 offset = Wobble(Time.time + i);
//         //     vertices[i] = vertices[i] + offset;
//         // }

//         // Update the mesh with the new vertex positions
//         textMesh.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
//     }

//     Vector2 Wobble(float time) {
//         float amplitude = 0.02f; // Adjust this value to change the height of the wave
//         float frequency = 0.15f; // Adjust this value to change the speed of the wave
//         return amplitude * new Vector2(Mathf.PerlinNoise(time * frequency, 0.0f), Mathf.PerlinNoise(0.0f, time * frequency));
//     }
// }






// // Vector2 Wobble(float time) {
//     //     float amplitude = 0.005f; // Adjust this value to change the height of the wave
//     //     float frequency = 0.1f; // Adjust this value to change the speed of the wave
//     //     return amplitude * new Vector2(Mathf.Sin(time * frequency), Mathf.Cos(time * frequency));
//     // }





//     // public TMP_Text textComponent;
    

//     // void Update()
//     // {
//     //     textComponent.ForceMeshUpdate();
//     //     var textInfo = textComponent.textInfo;

//     //     // can use i to apply diff effects per character
//     //     for (int i = 0; i < textInfo.characterCount; ++i) {
//     //         var charInfo = textInfo.characterInfo[i];

//     //         // skip if char is invisible
//     //         if (!charInfo.isVisible) {
//     //             continue;
//     //         }

//     //         var verts = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;


//     //         // one loop for each of the 4 vertices of character
//     //         for (int j = 0; i < 4; ++j) {
//     //             var orig = verts[charInfo.vertexIndex + j];
                
//     //             // override this vertex with modified version! HAHA manipulation !!
//     //             verts[charInfo.vertexIndex + j] = orig + new Vector3(0, Mathf.Sin(Time.time*2f + orig.x*0.01f) * 10f, 0);
//     //         }
//     //     }
        
//     //     for (int i = 0; i < textInfo.meshInfo.Length; ++i) {
//     //         var meshInfo = textInfo.meshInfo[i];
//     //         meshInfo.mesh.vertices = meshInfo.vertices;
//     //         textComponent.UpdateGeometry(meshInfo.mesh, i); 
//     //     }
//     // }

