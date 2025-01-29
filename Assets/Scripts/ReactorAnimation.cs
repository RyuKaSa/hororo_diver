using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactorAnimation : MonoBehaviour
{
    SpriteRenderer sr;
    public Sprite[] naked;
    public Sprite[] small;
    public Sprite[] medium;

    public Sprite[] full;

    public int level;

    public int frameID;
    public float animTime;

    public bool looped;
    // Start is called before the first frame update
    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }
    void Start()
    {
        InvokeRepeating("Animate", 0, animTime);
    }

    // Update is called once per frame
    void Animate()
    {
        if(!sr.enabled) {
            return;
        }

        Sprite[] current;

        current = naked;

        switch (level)
        {
            case 0:
                current = naked;
                break;
            case 1:
                current = small;
                break;
            case 2:
                current = medium;
                break;
            case 3:
                current = full;
                break;
            default:
                break;
        }

        if(looped) {
            frameID++;

            if(frameID >= 0 && frameID < current.Length) {
                sr.sprite = current[frameID];
            }
        }
        if(frameID > current.Length) {
                frameID = 0;
        }
        else {

            if(frameID < current.Length) {
                frameID++;
            }

            if(frameID >= 0 && frameID < current.Length) {
                sr.sprite = current[frameID];
            }
        }

    }
}
