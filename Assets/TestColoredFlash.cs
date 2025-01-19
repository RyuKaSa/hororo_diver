using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestColoredFlash : MonoBehaviour
{

    [SerializeField]
    private ColoredFlash coloredFlash;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("Active flash light");
            coloredFlash.Flash(Color.red);
        }

    }
}
