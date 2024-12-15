using System.Collections;
using System.Collections.Generic;
using System.IO; // for memorystream and binarywriter
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using HuggingFace.API;


public class SpeechRecognition : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button startOverButton;

    // [SerializeField] private Button stopButton;
    [SerializeField] private TextMeshProUGUI text;

    [SerializeField] public EmotionVisualizer emotionVisualizer; 
    [SerializeField] public TextMeshProUGUI introductionText; 


    private AudioClip clip;
    private byte[] bytes;
    private bool recording;

    /* for UI stuff */
    private void Start() 
    {
        startButton.onClick.AddListener(StartRecording);
        startOverButton.onClick.AddListener(StartOver);
        startOverButton.interactable= false;

        // stopButton.onClick.AddListener(StopRecording);
        // stopButton.interactable = false;
    }

    /* if recording reaches 10 sec (max) then stop recording automatically */
    private void Update() 
    {
        if (recording && Microphone.GetPosition(null) >= clip.samples) {
            StopRecording();
        }
    }

    /* will record up to 10 seconds of audio at 44100 Hz */
    private void StartRecording() 
    {
        StartCoroutine(FadeOutAndDisable());
        text.gameObject.SetActive(true);
        text.color = Color.white;
        text.fontSize= 20;
        StartCoroutine(FadeInAndEnable(2));
        text.text = "LISTENING...";
        startButton.interactable = false;
        // stopButton.interactable = true;
        clip = Microphone.Start(null, false, 10, 44100);
        recording = true;
    }

    private IEnumerator FadeOutAndDisable()
    {
        // Check if introductionText is assigned
        if (introductionText == null)
        {
            Debug.LogError("IntroductionText is not assigned in the Inspector.");
            yield break;
        }

        // Check if introductionText GameObject is active
        if (!introductionText.gameObject.activeInHierarchy)
        {
            Debug.LogError("IntroductionText GameObject is not active.");
            yield break;
        }

        Color originalColor = introductionText.color;
        float elapsedTime = 0f;

        while (elapsedTime < 0.8f)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / 0.8f);
            introductionText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        introductionText.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        introductionText.gameObject.SetActive(false);
    }
    

    private IEnumerator FadeInAndEnable(int seconds)
    {
        Debug.Log("fade in");
        
        yield return new WaitForSeconds(seconds);
        Color originalColor = text.color;
        // color.a= 0f;
        float elapsedTime = 0f;

        // Set initial alpha to 0
        text.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);

        while (elapsedTime < 0.8f)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / 0.8f);
            text.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        text.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f);
    }




    /* truncate recording and encode in WAV format */
    private void StopRecording() 
    {
        var position = Microphone.GetPosition(null);
        Microphone.End(null);
        var samples = new float[position * clip.channels];
        clip.GetData(samples, 0);
        bytes = EncodeAsWAV(samples, clip.frequency, clip.channels);
        recording = false;
        SendRecording();
    }

    /* prepare audio data for Hugging Face API
     * takes samples and converts into WAV format byte array
     */
    private byte[] EncodeAsWAV(float[] samples, int frequency, int channels) 
    {
        using (var memoryStream = new MemoryStream(44 + samples.Length * 2)) 
        {
            using (var writer = new BinaryWriter(memoryStream)) 
            {
                // write wav header
                writer.Write("RIFF".ToCharArray());
                writer.Write(36 + samples.Length * 2); // file size
                writer.Write("WAVE".ToCharArray());
                writer.Write("fmt ".ToCharArray());
                writer.Write(16);
                writer.Write((ushort)1);
                writer.Write((ushort)channels);
                writer.Write(frequency);
                writer.Write(frequency * channels * 2);
                writer.Write((ushort)(channels * 2));
                writer.Write((ushort)16); // bits per sample 
                writer.Write("data".ToCharArray());
                writer.Write(samples.Length * 2); // data chunk size 

                // write samples
                foreach (var sample in samples) {
                    writer.Write((short)(sample * short.MaxValue));
                }
            }
            return memoryStream.ToArray(); // return wav data as byte array
        }
    }

    /* use Hugging Face Unity API to run speech recognition on encoded audio
     * displays response in white if successful; else, error message in red
     */
    private void SendRecording()
    {
        text.color = Color.yellow;
        text.text = "SENDING...";
        // stopButton.interactable = false;
        HuggingFaceAPI.AutomaticSpeechRecognition(bytes, response => {
            text.color = Color.black;
            text.fontSize= 30;
            // tempText= text.color
            // tempText.a= 0;


            Color originalColor= text.color;
            text.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
            text.text = response.ToUpper();
            

            StartCoroutine(FadeInAndEnable(1));
            text.gameObject.AddComponent<TextNoise>();
            startOverButton.interactable = true;
            emotionVisualizer.ProcessTextTone(response); // call new func w recognized text
            startButton.interactable = true;
        }, error => {
            text.color = Color.red;
            text.text = "ERROR!";
            startButton.interactable = true;
        });
    }

    private void StartOver()
    {
        GameObject[] particles = GameObject.FindGameObjectsWithTag("Particle");
        foreach (GameObject particle in particles)
        {
            Destroy(particle);
            Debug.Log("particle destroyed");
        }

        // StartCoroutine(FadeOutAndDisable)
        text.text= " ";
        introductionText.gameObject.SetActive(true);
        Debug.Log("introduction text is active");
        Color textColor= introductionText.color;
        introductionText.color = new Color(textColor.r, textColor.g, textColor.b, 1f);


        // StartCoroutine(FadeInAndEnable(2));
        startOverButton.interactable = false;
    }


    // private void ProcessTextTone(string inputText) {
    //     Debug.Log("PLAYER INPUT  :  " + inputText);
    //     HuggingFaceAPI.TextClassification(inputText, outputClassification => {
    //         Debug.Log(outputClassification.classifications[0].label + outputClassification.classifications[0].score);
    //         Debug.Log(outputClassification.classifications[1].label + outputClassification.classifications[1].score);
    //         Debug.Log(outputClassification.classifications[2].label + outputClassification.classifications[2].score);
    //     }, error => {
    //         // On Error
    //         Debug.Log("Error");
    //     });
    // }


}




