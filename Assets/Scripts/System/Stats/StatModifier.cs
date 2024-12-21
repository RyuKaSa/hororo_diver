using System;

/// <summary>
/// This class represents a bonus or malus
/// modifier on Entity attributes. The modifier
/// can be flat or base on percentage.
/// </summary>
public sealed class StatModifier
{
    public enum StatModifierType
    {
        ADDITIONAL,
        PERCENTAGE
    };

    private readonly float value;

    private readonly short order; // defines if this modifier must be consider as bonus or malus (* -1 or * 1)

    private readonly StatModifierType type;

    public StatModifier(int value, short order, StatModifierType type)
    {
        this.value = value;
        this.order = order;
        this.type = type;
    }

    /// <summary>
    /// This method returns the value of the modifier.
    /// </summary>
    /// <returns></returns>
    public float Value()
    {
        return value * order;
    }

    public StatModifierType Type()
    {
        return type;
    }
}