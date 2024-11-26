using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour {
    private Vector3 initialPosition;
    public float noiseSpeed = 1f;
    public float noiseIntensity = 0.5f;
    public Vector3 velocity; // for bounce
    private Vector3 screenBounds;
    private float objectWidth, objectHeight;

    void Start() {
        initialPosition = transform.localPosition;

        //  random initial velocity for bouncing
        velocity = new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f), 0);

        // screen bounds for constraints
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        objectWidth = transform.localScale.x / 2;
        objectHeight = transform.localScale.y / 2;
    }

    void Update() {
        // Perlin Noise for mushed motion????
        float time = Time.time * noiseSpeed;
        Vector3 noiseOffset = new Vector3(
            Mathf.PerlinNoise(time, 0) * noiseIntensity - (noiseIntensity / 2),
            Mathf.PerlinNoise(0, time) * noiseIntensity - (noiseIntensity / 2),
            0
        );

        transform.localPosition = initialPosition + noiseOffset;

        transform.position += velocity * Time.deltaTime;

        if (transform.position.x > screenBounds.x - objectWidth || transform.position.x < -screenBounds.x + objectWidth) {
            velocity.x *= -1; // reverse direction
        }
        if (transform.position.y > screenBounds.y - objectHeight || transform.position.y < -screenBounds.y + objectHeight) {
            velocity.y *= -1; // reverse direction
        }
    }
}
