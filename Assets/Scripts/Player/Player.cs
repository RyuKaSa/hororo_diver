using UnityEngine;


public sealed class Player : MonoBehaviour, IDamageable
{


    [SerializeField]
    private Player_Input playerInput;

    [SerializeField]
    private float health = 15f;

    public void Start()
    {

    }

    public void Update()
    {
        playerInput.Update();
        if (health <= 0)
        {
            Debug.Log("Player is dead");
        }
    }

    public void Damage(float damage)
    {
        Debug.Log(transform.name + " takes " + damage + " damage");
        health -= damage;
    }


}