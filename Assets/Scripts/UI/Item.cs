using UnityEngine;

[System.Serializable]
public struct Item
{
    [SerializeField] private int count;
    [SerializeField] private ItemData data;

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