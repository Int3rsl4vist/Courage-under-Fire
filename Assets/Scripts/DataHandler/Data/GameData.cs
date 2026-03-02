using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int playerHealth;
    public Vector3 playerPosition;

    public SerializableDictionary<string, int> weaponsAmmo;
    public GameData()
    {
        this.playerHealth = 100;
        this.playerPosition = Vector3.zero;
        this.weaponsAmmo = new();
    }
}
