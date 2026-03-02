using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    // Unity neumí uložit Dictionary, ale umí uložit Listy.
    // Takže to při ukládání rozbijeme na dva seznamy.
    [SerializeField] private List<TKey> keys = new List<TKey>();
    [SerializeField] private List<TValue> values = new List<TValue>();

    // Zavolá se těsně PŘED uložením (Save)
    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();

        foreach (KeyValuePair<TKey, TValue> pair in this)
        {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
    }

    // Zavolá se hned PO načtení (Load)
    public void OnAfterDeserialize()
    {
        this.Clear();

        if (keys.Count != values.Count)
        {
            Debug.LogError("Chyba v SerializableDictionary: Počet klíčů a hodnot nesedí!");
            return;
        }

        for (int i = 0; i < keys.Count; i++)
        {
            // Ošetření duplicit, kdyby se data poškodila
            if (!this.ContainsKey(keys[i]))
            {
                this.Add(keys[i], values[i]);
            }
        }
    }
}