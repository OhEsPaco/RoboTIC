using UnityEngine;

/// <summary>
/// Defines the <see cref="JSonLoader" />
/// </summary>
public class JSonLoader : MonoBehaviour
{
    /// <summary>
    /// Defines the manager
    /// </summary>
    private LevelManager manager;

    // Start is called before the first frame update
    /// <summary>
    /// The Awake
    /// </summary>
    private void Awake()
    {
        manager = LevelManager.instance;
    }

    /// <summary>
    /// The ReadString
    /// </summary>
    /// <param name="path">The path<see cref="string"/></param>
    /// <returns>The <see cref="string"/></returns>
    private string ReadFileAsString(in string path)
    {
        System.IO.StreamReader reader = new System.IO.StreamReader(path);
        string output = reader.ReadToEnd();
        reader.Close();
        return output;
    }

    /// <summary>
    /// The LoadData
    /// </summary>
    /// <param name="jsonPath">The jsonPath<see cref="string"/></param>
    /// <returns>The <see cref="LevelData"/></returns>
    public LevelData LoadLevelData(in string jsonPath)
    {
        LevelData levelData = new LevelData();
        string readedString = ReadFileAsString(jsonPath);
        JsonUtility.FromJsonOverwrite(readedString, levelData);
        return levelData;
    }
}