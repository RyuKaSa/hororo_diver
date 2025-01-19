using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionManage : MonoBehaviour
{

    [SerializeField]
    private ParticleSystem explosionParticle;

    private float explosionStartTime;

    private float currentTime;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (explosionParticle.isPlaying)
        {
            currentTime += Time.deltaTime;
        }

        if (currentTime - explosionStartTime >= explosionParticle.main.duration)
        {
            Destroy(gameObject);
        }

    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log("In explosion manage collision layer " + other.gameObject.layer);
        if (other.gameObject.layer == LayerMask.NameToLayer("MisterFish") && !explosionParticle.isPlaying)
        {
            Debug.Log("With mister fish");
            explosionParticle.transform.position = transform.position;
            explosionParticle.Play();
            explosionStartTime = Time.deltaTime;
            currentTime = explosionStartTime;
        }

    }

}
