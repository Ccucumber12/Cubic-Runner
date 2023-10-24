using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Parallex : MonoBehaviour
{
    public GameObject mainCamera;
    public float parallaxEffect;

    private float startPosition;
    private float backgroundLength;

    void Start()
    {
        startPosition = transform.position.x;
        backgroundLength = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        float distance = (mainCamera.transform.position.x * parallaxEffect);
        transform.position = new Vector3(startPosition + distance, transform.position.y, transform.position.z);

        float temp = (mainCamera.transform.position.x * (1 - parallaxEffect));
        if (temp > startPosition + backgroundLength)
            startPosition += backgroundLength;
        else if (temp < startPosition - backgroundLength)
            startPosition -= backgroundLength;
    }
}
