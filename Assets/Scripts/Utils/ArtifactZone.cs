using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class ArtifactZone : MonoBehaviour
{

    [SerializeField]
    UnityEvent onTriggerEnter;

    [SerializeField]
    UnityEvent onTriggerExit;

    void Start()
    {

    }

    void Update()
    {

    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            onTriggerEnter.Invoke();

        }

    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            onTriggerExit.Invoke();

        }
    }

}
