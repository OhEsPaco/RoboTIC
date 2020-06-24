using System.Collections.Generic;
using UnityEngine;

public class EditorLogic : MonoBehaviour
{
    [SerializeField] private int mapSizeX = 8;
    [SerializeField] private int mapSizeY = 4;
    [SerializeField] private int mapSizeZ = 8;

    private void Awake()
    {
        EventAggregator.Instance.Subscribe<MsgEditorMapSize>(ServeMapSize);
    }

    private void ServeMapSize(MsgEditorMapSize msg)
    {
        List<int> mapSize = new List<int>();
        mapSize.Add(mapSizeX);
        mapSize.Add(mapSizeY);
        mapSize.Add(mapSizeZ);
        EventAggregator.Instance.Publish(new ResponseWrapper<MsgEditorMapSize, List<int>>(msg, mapSize));
    }

    // Update is called once per frame
    private void Update()
    {
    }
}