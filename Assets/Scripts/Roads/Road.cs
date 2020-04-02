using System.Collections.Generic;
using UnityEngine;
using static PathContainer;
using static RoadIO;

public abstract class Road : MonoBehaviour
{
    private Dictionary<IODirection, List<RoadIO>> ioByDirection = new Dictionary<IODirection, List<RoadIO>>();
    private Dictionary<string, RoadIO> ioByID = new Dictionary<string, RoadIO>();
    private Dictionary<string, Path> pathsByName = new Dictionary<string, Path>();

    private RoadIO[] allIO;
    private PathContainer pathContainer;

    public string RoadIdentifier
    {
        get
        {
            return gameObject.name;
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
        pathContainer = GetComponentInChildren<PathContainer>();

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

    public abstract void ExecuteAction(in string[] args);

    public abstract bool GetPathAndOutput(in RoadInput input, out Path path, out RoadOutput output);

    protected bool GetPathByName(in string name, out Path path)
    {
        if (pathsByName.ContainsKey(name))
        {
            path = pathsByName[name];
            return true;
        }
        else
        {
            if (pathContainer != null)
            {
                if (pathContainer.GetPathByName(name, out path))
                {
                    pathsByName.Add(name, path);

                    return true;
                }
            }
            else
            {
                Debug.LogWarning("No PathContainer found");
            }
        }
        path = new Path();
        return false;
    }

    protected bool DoesThisRoadHasThisIO(string id)
    {
        return ioByID.ContainsKey(id);
    }
}