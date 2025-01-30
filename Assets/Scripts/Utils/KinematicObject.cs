using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KinematicObject : MonoBehaviour
{

    [SerializeField]
    private Transform source, destination, misterFishSource, misterFishDest;

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
            if (Vector3.Distance(transform.position, destination.position) <= 1.5f)
            {
                // Set Kinematic point of Mister Fish
                SetMisterFishKinematicPoint();
                Destroy(gameObject);
            }
        }
    }


    public void MoveTo()
    {
        if (!isTriggered)
        {
            Camera mainCamera = Camera.main;
            float camWidth = mainCamera.orthographicSize * mainCamera.aspect;
            float camHeight = mainCamera.orthographicSize;

            transform.position = new Vector3(mainCamera.transform.position.x + camWidth, mainCamera.transform.position.y, mainCamera.nearClipPlane - 50.5f);
            destination.position = new Vector3(mainCamera.transform.position.x - camWidth, mainCamera.transform.position.y, mainCamera.nearClipPlane - 50.5f);

        }

        isTriggered = true;
        var step = speed * Time.time;

        // transform.position = Vector3.MoveTowards(source.position, destination.position, step);
        transform.position = Vector3.SmoothDamp(transform.position, destination.position, ref velocity, 0.5f);
    }

    private void SetMisterFishKinematicPoint()
    {
        var misterFish = GameObject.Find("MisterFish").GetComponent<MisterFish>();

        // If it comes from the right, it appears from the left and vice versa.
        misterFish.kinematicData.begin = source.position.x > destination.position.x ? misterFishDest : misterFishSource;
        misterFish.kinematicData.end = misterFish.kinematicData.begin.position == destination.position ? misterFishSource : misterFishDest;
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

}
