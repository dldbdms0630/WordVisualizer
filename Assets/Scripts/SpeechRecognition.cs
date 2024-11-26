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
    // [SerializeField] private Button stopButton;
    [SerializeField] private TextMeshProUGUI text;

    [SerializeField] public EmotionVisualizer emotionVisualizer; 

    private AudioClip clip;
    private byte[] bytes;
    private bool recording;

    /* for UI stuff */
    private void Start() 
    {
        startButton.onClick.AddListener(StartRecording);
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
        text.color = Color.white;
        text.text = "Recording...";
        startButton.interactable = false;
        // stopButton.interactable = true;
        clip = Microphone.Start(null, false, 10, 44100);
        recording = true;
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
        text.text = "Sending...";
        // stopButton.interactable = false;
        HuggingFaceAPI.AutomaticSpeechRecognition(bytes, response => {
            text.color = Color.white;
            text.text = response;
            emotionVisualizer.ProcessTextTone(response); // call new func w recognized text
            startButton.interactable = true;
        }, error => {
            text.color = Color.red;
            text.text = error;
            startButton.interactable = true;
        });
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




