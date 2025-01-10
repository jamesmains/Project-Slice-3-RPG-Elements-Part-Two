using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

[Serializable]
public class ObservableArray<T> {
    [ItemCanBeNull] private T[] Items;
    
    public event Action<T[]> AnyValueChanged = delegate { };
    public int Count => Items.Count(i => i != null);
    public T this[int index] => Items[index];

    public ObservableArray(int size = 20, IList<T> initialValues = null) {
        Items = new T[size];
        if (initialValues != null) {
            initialValues.Take(size).ToArray().CopyTo(Items,0);
            Invoke();
        }
    }
    
    public List<T> Get() => Items.ToList();
    
    public void Invoke() => AnyValueChanged.Invoke(Items);

    public void Swap(int index1, int index2) {
        (Items[index1], Items[index2]) = (Items[index2], Items[index1]);
        Invoke();
    }

    public void Clear() {
        Items = new T[Items.Length];
        Invoke();
    }
    
    public bool TryAdd(T item) {
        for (int i = 0; i < Items.Length; i++) {
            if (Items[i] != null) continue;
            Items[i] = item;
            Invoke();
            return true;
        }
        return false;
    }

    public T TryAdd(T item, int index) {
        if (Items[index] != null) {
            var itemToRemove = Items[index];
            Items[index] = item;
            Invoke();
            return itemToRemove;
        }
        else {
            Items[index] = item;
            Invoke();
            return default;
        }
    }

    public T TryRemove(T item) {
        for (int i = 0; i < Items.Length; i++) {
            if(!EqualityComparer<T>.Default.Equals(Items[i], item)) continue;
            var itemToRemove = Items[i];
            Items[i] = default;
            Invoke();
            return itemToRemove;
        }
        return default;
    }
}
