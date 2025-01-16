using System.Collections.Generic;

public class WeaponUpgrade
{
    public string Attribute { get; private set; }
    public float Percentage { get; private set; }
    public Dictionary<string, int> RequiredOres { get; private set; }
    public string WeaponType { get; private set; }

    public WeaponUpgrade(string weaponType, string attribute, float percentage, Dictionary<string, int> requiredOres)
    {
        WeaponType = weaponType;
        Attribute = attribute;
        Percentage = percentage;
        RequiredOres = requiredOres;
    }
}