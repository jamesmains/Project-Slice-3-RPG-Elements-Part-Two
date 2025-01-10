using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemDictionary : UnitySerializedDictionary<Item, int>{}

public abstract class UnitySerializedDictionary<TKey, TValue>:
    Dictionary<TKey, TValue>, ISerializationCallbackReceiver {
    [SerializeField] 
    private List<KeyValueData> keyValueData = new();
    public void OnAfterDeserialize() {
        this.Clear();
        foreach (var item in this.keyValueData) {
            this[item.key] = item.value;
        }
    }

    public void OnBeforeSerialize() {
        this.keyValueData.Clear();
        foreach (var kvp in this) {
            this.keyValueData.Add(new KeyValueData() {
                key = kvp.Key,
                value = kvp.Value
            });
        }
    }

    [Serializable]
    private struct KeyValueData {
        public TKey key;
        public TValue value;
    }
}
