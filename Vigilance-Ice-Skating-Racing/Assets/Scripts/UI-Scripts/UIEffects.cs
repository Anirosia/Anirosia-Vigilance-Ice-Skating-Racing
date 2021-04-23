using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEffects : MonoBehaviour
{
    [Header("Settings")] 
    public bool hasBounce = false;
    public float speed = 1;
    public float amplitude = 1;
    private Vector3 initialPosition;
    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x, initialPosition.y + Mathf.Sin(Time.time * speed) * amplitude, transform.position.y);
    }
}
