using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomScale : MonoBehaviour
{
    public float minScale = 8.0f;
    public float maxScale = 12.0f;

    public Transform target;

    private float time;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        float scale = minScale + ((Mathf.Sin(time*37.0f)*0.5f+0.5f) * (maxScale-minScale));
        target.localScale = new Vector3(scale, scale, scale);
    }
}
