//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using System.Linq;

//public class DataPersistanceManager : MonoBehaviour
//{
//    private GameData gameData;
//    private List<IDataPersistance> dataPersistanceList;
//    public static DataPersistanceManager Instance { get; private set; }

//    private void Awake()
//    {
//        if(Instance != null)
//        {
//            Debug.LogError("Data Persistance Manager already exists!");
//        }
//        Instance = this;
//    }

//    private void Start()
//    {
//        this.dataPersistanceList = FindAllDataPersistances();
//        LoadGame();
//    }

//    public void NewGame()
//    {
//        this.gameData = new GameData();
//    }
//    public void LoadGame()
//    {
//        if(this.gameData == null)
//        {
//            Debug.Log("No data found. Initialising defults");
//            NewGame();
//        }

//        foreach (IDataPersistance dataPersistance in dataPersistanceList)
//            dataPersistance.LoadData(gameData);

//        Debug.Log($"Loaded ammo: {gameData.ammo}");
//    }
//    public void SaveGame()
//    {
//        foreach(IDataPersistance dataPersistance in dataPersistanceList)
//            dataPersistance.SaveData(ref gameData);

//        Debug.Log($"Saved ammo: {gameData.ammo}");
//    }

//    private void OnApplicationQuit()
//    {
//        SaveGame();
//    }

//    private List<IDataPersistance> FindAllDataPersistances()
//    {
//        IEnumerable<IDataPersistance> dataPersistances = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistance>();
//        return new List<IDataPersistance>(dataPersistances);
//    }
//}
