using System;

public interface IMob
{
    public float InflictDamage(float health);

    public void ReceiveDamage(float damage);
}