using JetBrains.Annotations;
using UnityEngine;

[System.Serializable]
public struct Item
{
    [SerializeField] private int count;
    [SerializeField] private ItemData data;

    public void Merge(ref Item _other)
    {
        if (Full) return;

        if(Empty) data = _other.data;

        if (_other.Data != data) throw new System.Exception("Try to merge differents item types.");

        int _total = _other.count + count;

        if(_total <= data.stackMaxCount)
        {
            count = _total;
            _other.count = 0;
            return;
        }

        count = data.stackMaxCount;
        _other.count = _total - count;
    }

    public bool AvailableFor(Item _other) => Empty || (Data == _other.Data && !Full);

    public ItemData Data => data;
    public bool Full => data && count >= data.stackMaxCount;
    public bool Empty => count == 0 || data == null;
    public int Count => count;
}
