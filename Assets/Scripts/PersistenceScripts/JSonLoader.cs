using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JSonLoader : MonoBehaviour
{
    private LevelManager manager;
    // Start is called before the first frame update
    void Awake()
    {
        manager = LevelManager.instance;
        
       
    }

    private string ReadString(string path)
    {
        System.IO.StreamReader reader = new System.IO.StreamReader(path);
        string output = reader.ReadToEnd();
        reader.Close();
        return output;
    }

    public LevelData LoadData(string jsonPath)
    {
        LevelData levelData = new LevelData();
        string readedString = ReadString(jsonPath);
        JsonUtility.FromJsonOverwrite(readedString, levelData);
        return levelData;

    }
}
