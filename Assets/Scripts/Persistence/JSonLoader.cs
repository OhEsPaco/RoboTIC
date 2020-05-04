using UnityEngine;

/// <summary>
/// Defines the <see cref="JSonLoader" />
/// </summary>
public class JSonLoader : MonoBehaviour
{
    [SerializeField] private EventAggregator eventAggregator;
    public void Awake()
    {
        eventAggregator.Subscribe<MsgLoadLevelData>(LoadLevelData);
    }
    private void Start()
    {
        /*eventAggregator.Subscribe<ResponseWrapper<MsgLoadLevelData, LevelData>>(this.ReceiveLevelData);
        eventAggregator.Publish<MsgLoadLevelData>(new MsgLoadLevelData(GetLevelPath()));*/
       
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
    private void LoadLevelData(MsgLoadLevelData msg)
    {
        LevelData levelData = new LevelData();
        string readedString = ReadFileAsString(msg.Path);
        JsonUtility.FromJsonOverwrite(readedString, levelData);

        eventAggregator.Publish(new ResponseWrapper<MsgLoadLevelData, LevelData>(msg, levelData));
    }
}