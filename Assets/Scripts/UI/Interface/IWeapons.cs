using System;

public interface IWeapons : IEquipable
{
    public void AttackProcessing();

    public bool WeaponAnimationIsPlaying();
}