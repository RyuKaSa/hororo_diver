using UnityEngine;

public class DebugTestMob : MonoBehaviour
{
    public float visionRadius = 5f;

    public bool drawVisionCircle = true;  // Activer/désactiver l'affichage du cercle

    void Update()
    {
        if (drawVisionCircle)
        {
            DrawVisionCircle();
        }
    }

    /// <summary>
    /// Trace un cercle représentant le champ de vision autour de TestMob.
    /// </summary>
    void DrawVisionCircle()
    {
        int segments = 360;  // Nombre de segments pour approximations du cercle
        float angleStep = 360f / segments;  // Calcul du pas d'angle pour chaque segment

        for (int i = 0; i < segments; i++)
        {
            // Calculer la position de chaque point sur le cercle
            float angle = i * angleStep * Mathf.Deg2Rad;  // Angle en radians
            float x = Mathf.Cos(angle) * visionRadius;
            float y = Mathf.Sin(angle) * visionRadius;
            Vector3 pointOnCircle = new Vector3(x, y, 0f);

            // Tracer une ligne entre deux points successifs pour créer le cercle
            if (i > 0)
            {
                Debug.DrawLine(previousPoint, pointOnCircle, Color.green);
            }

            previousPoint = pointOnCircle;
        }

        // Relier le dernier point au premier pour fermer le cercle
        Debug.DrawLine(previousPoint, new Vector3(Mathf.Cos(0) * visionRadius, 0f, Mathf.Sin(0) * visionRadius) + transform.position, Color.green);
    }

    // Variable pour suivre le dernier point du cercle
    private Vector3 previousPoint;
}
