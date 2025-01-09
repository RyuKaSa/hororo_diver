using UnityEngine;

[System.Serializable]
public struct Item
{
    [SerializeField] private int count;
    [SerializeField] private ItemData data;


    /// <summary>
    /// Constructor to create an Item with specified ItemData and count.
    /// </summary>
    /// <param name="data">The ItemData associated with the item.</param>
    /// <param name="count">The initial count of the item.</param>
    public Item(ItemData data, int count)
    {
        if (data == null)
            throw new System.ArgumentNullException(nameof(data), "ItemData cannot be null.");

        if (count < 0 || count > data.stackMaxCount)
            throw new System.ArgumentOutOfRangeException(nameof(count), $"Count must be between 0 and {data.stackMaxCount}.");

        this.data = data;
        this.count = count;
    }

    public void Merge(ref Item other)
    {
        if (Full) return;

        if (Empty)
            data = other.data;

        if (other.Data != data)
            throw new System.Exception("Cannot merge different item types.");

        int total = other.Count + count;

        if (total <= data.stackMaxCount)
        {
            count = total;
            other.count = 0;
            return;
        }

        count = data.stackMaxCount;
        other.count = total - count;
    }

    public bool AvailableFor(Item other) =>
        Empty || (Data == other.Data && !Full);

    public ItemData Data => data;
    public bool Full => data != null && count >= data.stackMaxCount;
    public bool Empty => count == 0 || data == null;
    public int Count => count;
}