using System;
using UnityEngine;

public interface IWeapons
{
    string ItemName();
    Sprite Icon();
    ItemData.ItemType ItemType();
    int StackMaxCount();
    void AttackProcessing();
    void ApplyUpgrade(string attribute, float percentage);
}