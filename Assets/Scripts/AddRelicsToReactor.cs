using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddRelicsToReactor : MonoBehaviour
{
    public ReactorAnimation reactor;
    public RandomScale scaler;
    public SmoothScroll fail;
    public SmoothScroll little;
    public SmoothScroll medium;
    public SmoothScroll full;

    public int relicsCount = 0;
    // Start is called before the first frame update
    void Start()
    {
        var itemDB = Utils.GetComponentFromGameObjectTag<ItemDatabase>("ItemDatabase");
        if (itemDB == null)
        {
            Debug.Log("Error: Could not find ItemDatabase component");
        }
        else {
            relicsCount = itemDB.artifacts.Length;
            var dontDestroyComponent = itemDB.GetComponent<DontDestroy>();
            dontDestroyComponent.remove(transform);
            Destroy(itemDB);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check collision with Player Layer
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            setSpriteFromRelicsAmount(relicsCount);
            // switch (reactor.level)
            // {
            //     case 0:
            //         reactor.level = 1;
            //         scaler.maxScale = 9.5f;
            //         break;
            //     case 1:
            //         reactor.level = 2;
            //         scaler.maxScale = 8.5f;
            //         break;
            //     case 2:
            //         reactor.level = 3;
            //         scaler.maxScale = 8;
            //         break;
            //     case 3:
            //         // show text
            //     default:
            //         break;
            // }
        }
    }

    private void setSpriteFromRelicsAmount(int count) {
        if(count == 0) {
            reactor.level = 0;
            scaler.maxScale = 12.0f;
            fail.isActif = true;
            return;
        }
        if(count <= 3) {
            reactor.level = 1;
            scaler.maxScale = 9.5f;
            little.isActif = true;
            return;
        }
        if(count <= 6) {
            reactor.level = 2;
            scaler.maxScale = 8.5f;
            medium.isActif = true;
            return;
        }
        if(count <= 10) {
            reactor.level = 3;
            scaler.maxScale = 8f;
            full.isActif = true;
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
