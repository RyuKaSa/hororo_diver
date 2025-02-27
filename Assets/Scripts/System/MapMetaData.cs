using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMetaData : MonoBehaviour
{
    [SerializeField]
    private List<Transform> respawnPointList;

    void Start()
    {

    }

    void Update()
    {

    }

    public Vector3 PositionToRespawnPoint(Vector3 position)
    {
        Vector3 closestRespawnPoint = respawnPointList[0].position;
        float closestYDifference = Mathf.Infinity;
        foreach (Transform respawnPoint in respawnPointList)
        {
            Debug.Log("respawnPoint " + respawnPoint.position);
            // Checks is respawnPoint is above position in parameter
            if (respawnPoint.position.y > position.y)
            {
                float deltaY = respawnPoint.position.y - position.y;
                if (deltaY < closestYDifference)
                {
                    closestYDifference = deltaY;
                    closestRespawnPoint = respawnPoint.position;
                }
            }
        }

        return closestRespawnPoint;
    }
}
