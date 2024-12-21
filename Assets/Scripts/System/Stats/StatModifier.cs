using System;

/// <summary>
/// This class represents a bonus or malus
/// modifier on Entity attributes. The modifier
/// can be flat or base on percentage.
/// </summary>
public sealed class StatModifier
{
    private readonly float value;

    private readonly short order; // defines if this modifier must be consider as bonus or malus (* -1 or * 1)

    public StatModifier(int value, short order)
    {
        this.value = value;
        this.order = order;
    }
}