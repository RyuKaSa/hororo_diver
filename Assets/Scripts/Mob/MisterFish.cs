using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MisterFish : MonoBehaviour
{

    [SerializeField]
    private GameObject player;

    [SerializeField]
    private float initialRadius = 5f;

    [SerializeField]
    private float minRadius = 2f;

    [SerializeField]
    private float maxRadius = 10f;

    [SerializeField]
    private float speed = 0.5f;

    [SerializeField]
    private float radiusChangeInterval = 3f; // Intervalle de temps entre les changements de rayon

    private float radius; // Rayon actuel

    private float angle = 0f;

    private float radiusChangeTimer = 0f;

    void Start()
    {
        radius = initialRadius;
    }

    private void erraticMovement()
    {
        angle += speed * Time.deltaTime;

        float x = Mathf.Cos(angle) * radius;
        float y = Mathf.Sin(angle) * radius;

        transform.position = player.transform.position + new Vector3(x, y, 0);
    }

    void Update()
    {
        radiusChangeTimer += Time.deltaTime;

        if (radiusChangeTimer >= radiusChangeInterval)
        {
            radiusChangeTimer = 0f; // Réinitialiser le timer

            radius = Random.Range(minRadius, maxRadius);
        }

        erraticMovement();
    }
}
