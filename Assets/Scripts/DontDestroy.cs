using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    GameObject target;
    // Start is called before the first frame update
    void Start()
    {
        keep();
    }

    public void keep() {
        DontDestroyOnLoad(gameObject);
    }

    public void remove(Transform parent) {
        transform.SetParent(parent);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
