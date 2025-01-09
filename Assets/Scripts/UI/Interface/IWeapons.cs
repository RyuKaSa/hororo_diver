using System;

public interface IWeapons : IItemData
{
    public void AttackProcessing();

    public bool WeaponAnimationIsPlaying();
}