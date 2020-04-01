using System.Collections.Generic;
using UnityEngine;
using static RoadIO;

public class Road : MonoBehaviour
{
    private Dictionary<IODirection, List<RoadIO>> ioByDirection = new Dictionary<IODirection, List<RoadIO>>();
    private Dictionary<string, RoadIO> ioByID = new Dictionary<string, RoadIO>();

    private RoadIO[] allIO;

    [UniqueIdentifier, SerializeField] private string id;

    public string RoadIdentifier
    {
        get
        {
            return id;
        }
    }

    public List<RoadIO> GetRoadIOByDirection(IODirection direction)
    {
        return ioByDirection[direction];
    }

    public RoadIO GetRoadIOByID(string ioID)
    {
        if (!ioByID.ContainsKey(ioID))
        {
            return null;
        }
        return ioByID[ioID];
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
            ioByDirection.Add(direction, listOfIO);
        }

        foreach (RoadIO thisIO in allIO)
        {
            List<RoadIO> listOfIO = ioByDirection[thisIO.Direction];
            listOfIO.Add(thisIO);
            if (!ioByID.ContainsKey(thisIO.IOIdentifier))
            {
                ioByID.Add(thisIO.IOIdentifier, thisIO);
            }

        }
    }

    public RoadOutput GetNextOutput(RoadInput roadInput)
    {
        return null;
    }
}