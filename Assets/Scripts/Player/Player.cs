using UnityEngine;


public sealed class Player : MonoBehaviour
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

    public void TakeDamage(IMob mob)
    {
        Debug.Log("Inflige damage");
        health = mob.InflictDamage(health);
    }

}