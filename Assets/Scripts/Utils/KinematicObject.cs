using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KinematicObject : MonoBehaviour
{

    [SerializeField]
    private Transform source, destination;

    [SerializeField]
    private float speed;

    private Vector3 velocity;

    private bool isTriggered = false;

    void Start()
    {

    }

    void Update()
    {
        if (isTriggered)
        {
            MoveTo();
        }
    }


    public void MoveTo()
    {
        if (!isTriggered)
        {
            Camera mainCamera = Camera.main;
            float camWidth = mainCamera.orthographicSize * mainCamera.aspect;
            float camHeight = mainCamera.orthographicSize;

            transform.position = new Vector3(mainCamera.transform.position.x + camWidth, mainCamera.transform.position.y, mainCamera.nearClipPlane - 5f);
            destination.position = new Vector3(mainCamera.transform.position.x - camWidth, mainCamera.transform.position.y, mainCamera.nearClipPlane - 5f);

            Debug.Log("source.position = " + source.position + " destination.position = " + destination.position + " camWidth = " + camWidth);

        }

        isTriggered = true;
        var step = speed * Time.time;

        // transform.position = Vector3.MoveTowards(source.position, destination.position, step);
        transform.position = Vector3.SmoothDamp(transform.position, destination.position, ref velocity, 0.5f);
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

}
