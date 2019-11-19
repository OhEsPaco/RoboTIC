using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{

    public string levelPath = "Assets/StoryLevels/test.json";
    private LevelData levelData;
    // Start is called before the first frame update
    void Start()
    {
        levelData = new LevelData();
        string readedString= ReadString(levelPath);
        //Debug.Log(readedString);
        //LevelData = JsonUtility.FromJson<LevelData>(readedString);

        JsonUtility.FromJsonOverwrite(readedString, levelData);
        Debug.Log(levelData.availableInstructions.jump);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    string ReadString(string path)
    {
        //Read the text from directly from the test.txt file
        System.IO.StreamReader reader = new System.IO.StreamReader(levelPath);
        string output = reader.ReadToEnd();
        reader.Close();
     //   Debug.Log(output);
        return output;
    }

}
