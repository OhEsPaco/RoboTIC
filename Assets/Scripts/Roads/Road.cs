using System.Collections.Generic;
using UnityEngine;
using static RoadIO;

public class Road : MonoBehaviour
{
    private Dictionary<IODirection, List<RoadIO>> roadsByDirection = new Dictionary<IODirection, List<RoadIO>>();

    public List<RoadIO> GetRoadIOByDirection(IODirection direction)
    {
        return roadsByDirection[direction];
    }

    private void Start()
    {
        RoadIO[] AllIO = GetComponentsInChildren<RoadIO>();

        foreach (IODirection direction in System.Enum.GetValues(typeof(IODirection)))
        {
            List<RoadIO> listOfIO = new List<RoadIO>();
            roadsByDirection.Add(direction, listOfIO);
        }

        foreach (RoadIO thisIO in AllIO)
        {
            List<RoadIO> listOfIO = roadsByDirection[thisIO.Direction];
            listOfIO.Add(thisIO);
            Debug.Log(thisIO.Direction);
        }
    }

    public RoadOutput GetNextOutput(RoadInput roadInput)
    {
        return null;
    }
}