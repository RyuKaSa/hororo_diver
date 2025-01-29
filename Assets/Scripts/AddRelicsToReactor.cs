using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddRelicsToReactor : MonoBehaviour
{
    public ReactorAnimation reactor;
    public RandomScale scaler;
    // Start is called before the first frame update
    void Start()
    {
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check collision with Player Layer
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            switch (reactor.level)
            {
                case 0:
                    reactor.level = 1;
                    scaler.maxScale = 10.5f;
                    break;
                case 1:
                    reactor.level = 2;
                    scaler.maxScale = 8.5f;
                    break;
                case 2:
                    reactor.level = 3;
                    scaler.maxScale = 8;
                    break;
                default:
                    break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
