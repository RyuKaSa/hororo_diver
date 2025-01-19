using System.Collections.Generic;

public class Upgrade
{
    public string Attribute { get; private set; }
    public float Percentage { get; private set; }
    public Dictionary<string, int> RequiredOres { get; private set; }

    public Upgrade(string attribute, float percentage, Dictionary<string, int> requiredOres)
    {
        Attribute = attribute;
        Percentage = percentage;
        RequiredOres = requiredOres;
    }
}
