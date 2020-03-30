using System.Collections.Generic;
using UnityEngine;
using static RoadIO;

public class Road : MonoBehaviour
{
    private Dictionary<IODirection, List<RoadIO>> roadsByDirection = new Dictionary<IODirection, List<RoadIO>>();
    private RoadIO[] allIO;
    public List<RoadIO> GetRoadIOByDirection(IODirection direction)
    {
        
        return roadsByDirection[direction];
    }

    public RoadIO[] GetAllIO()
    {
        return allIO;
    }

    private void Awake()
    {
        allIO = GetComponentsInChildren<RoadIO>();

        foreach (IODirection direction in System.Enum.GetValues(typeof(IODirection)))
        {
            List<RoadIO> listOfIO = new List<RoadIO>();
            roadsByDirection.Add(direction, listOfIO);
        }



        foreach (RoadIO thisIO in allIO)
        {
            List<RoadIO> listOfIO = roadsByDirection[thisIO.Direction];
            listOfIO.Add(thisIO);
            //Debug.Log(thisIO.Direction);
        }

       
    }

    public RoadOutput GetNextOutput(RoadInput roadInput)
    {
        return null;
    }
}