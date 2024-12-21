using System;

/// <summary>
/// Class which represents an attribute in the game
/// (speed or damage for exemple). This attribute contains
/// list of stat modifier which modify the base value.
/// Calculates the final value as needed 
/// i.e. when calling the FinalValue() function
/// </summary>
public sealed class Attribute
{
    private readonly float baseValue;

    private readonly List<StatModifier> statModifierList = new List<StatModifier>();

    private float finalValue;

    public Attribute(float baseValue)
    {
        this.baseValue = baseValue;
        finalValue = baseValue;
    }


    /// <summary>
    /// Add stat modifier to the List
    /// </summary>
    /// <param name="statModifier"></param>
    public void AddStatModifier(StatModifier statModifier)
    {
        if (statModifier == null)
        {
            Debug.Warning("StatModifier parameter is null");
            return;
        }
        statModifierList.Add(statModifier);
    }

    private float ComputeStatModifier(StatModifier statModifier, float acc)
    {
        if (statModifier.Type() == StatModifier.StatModifierType.ADDITIONAL)
        {
            return acc + statModifier.Value();
        }
        else
        {
            return acc * (1 + statModifier.Value());
        }
    }


    /// <summary>
    /// Method which calculates the final value of this attribute
    /// base on the stat modifier list
    /// </summary>
    /// <returns></returns>
    public float FinalValue()
    {
        return statModifierList.Aggregate(baseValue, (sm, acc) => { acc += ComputeStatModifier(sm, acc); });
    }

}