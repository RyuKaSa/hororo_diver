using System;
using UnityEngine;

public interface IWeapons : IEquipable
{
    Sprite Icon();
    ItemData.ItemType ItemType();
    int StackMaxCount();

    public void AttackProcessing();

    public bool WeaponAnimationIsPlaying();

    public string WeaponName();

    void ApplyUpgrade(string attribute, float percentage);

    public float TimeBetween2Attack();

}