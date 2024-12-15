using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour {

    private float zSpeed; // Speed of movement on the z-axis
    private float zRange; // Range of z-axis movement
    private float zOffset; // Random offset for uniqueness
    private Vector3 originalPosition;

    void Start()
    {
        // Save the initial position to ensure x and y stay constant
        originalPosition = transform.position;

        // Assign unique motion parameters for each instance
        zSpeed = Random.Range(0.2f, 1f); // Speed of movement
        zRange = Random.Range(2f, 10f);  // Range of back-and-forth motion
        zOffset = Random.Range(0f, Mathf.PI * 2f); // Phase offset for varied motion
    }

    void Update()
    {
        // Calculate new z-position using a sine wave for smooth back-and-forth motion
        float newZ = originalPosition.z + Mathf.Sin(Time.time * zSpeed + zOffset) * zRange;

        // Apply the new z-position while keeping x and y constant
        transform.position = new Vector3(originalPosition.x, originalPosition.y, newZ);
    }

    // private Vector3 initialPosition;
    // public float minNoiseSpeed = 1;
    // public float maxNoiseSpeed = 13; 
    // public float minIntensity = 1;
    // public float maxIntensity = 5; 
    // public Vector3 velocity; // for bounce
    // private Vector3 screenBounds;
    // private float objectWidth, objectHeight;
    
    // private float intensity;


    // void Start() {
    //     initialPosition = transform.localPosition;

    //     //  random initial velocity for bouncing
    //     // velocity = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);

    //     // screen bounds for constraints
    //     // screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
    //     // objectWidth = transform.localScale.x / 2;
    //     // objectHeight = transform.localScale.y / 2;
    // }

    // void Update() {
    //     // Perlin Noise for mushed motion????
    //     intensity= Random.Range(minIntensity, maxIntensity); 
        
    //     // float time = Time.time * Random.Range(minNoiseSpeed, maxNoiseSpeed);
    //     // Vector3 noiseOffset = new Vector3(
    //     //     Mathf.PerlinNoise(time, 0) * intensity - (intensity / 2),
    //     //     Mathf.PerlinNoise(0, time) * intensity - (intensity / 2),
    //     //     0
    //     // );

    //     // transform.localPosition = initialPosition + noiseOffset;

    //     // transform.position += velocity * Time.deltaTime;

    //     if (transform.position.x > screenBounds.x - objectWidth || transform.position.x < -screenBounds.x + objectWidth) {
    //         velocity.x *= -1; // reverse direction
    //     }
    //     if (transform.position.y > screenBounds.y - objectHeight || transform.position.y < -screenBounds.y + objectHeight) {
    //         velocity.y *= -1; // reverse direction
    //     }
    // }
}
