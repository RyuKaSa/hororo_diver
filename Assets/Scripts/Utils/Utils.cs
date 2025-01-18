using UnityEngine;


public sealed class Utils
{

    public static T GetComponentFromGameObjectTag<T>(string tag)
    {
        if (tag == null)
        {
            Debug.Log("Error: tag is null");
        }

        var go = GameObject.FindGameObjectWithTag(tag);
        if (go == null)
        {
            Debug.Log("Error: GameObject with " + tag + " tag name not found");
        }

        T goComponent = go.GetComponent<T>();
        if (goComponent == null)
        {
            Debug.Log("Error: could not find component");
        }
        return goComponent;

    }
}