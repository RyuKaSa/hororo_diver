using UnityEngine;


public sealed class Projectile : MonoBehaviour
{
    float speed;
    float gradient, offset; // Represent gradient and offset in linear equation

    public void Initialize(float speed, float gradient, float offset)
    {
        this.speed = speed;
        this.gradient = gradient;
        this.offset = offset;
    }

    void Update()
    {
        float x = transform.position.x + speed;
        float y = gradient * x + offset;

        transform.position = new Vector3(x, y, 0f);
    }
}