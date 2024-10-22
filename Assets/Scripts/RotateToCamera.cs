using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToCamera : MonoBehaviour
{
    private  Camera cam;
    public float rotationSpeed;

    void Start() 
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        float depthFromCamera = cam.WorldToScreenPoint(transform.position).z;
        mousePos.z = depthFromCamera; // Utiliser la profondeur de l'objet
        Vector3 worldMousePos = cam.ScreenToWorldPoint(mousePos);
        Vector3 direction = worldMousePos - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
        Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

    }
}
