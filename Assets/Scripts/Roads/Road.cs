using System.Collections.Generic;
using UnityEngine;
using static PathContainer;
using static RoadIO;

public abstract class Road : MonoBehaviour
{
    //Si es un conector o no
    [SerializeField] private bool connector = false;

    //Los objetos RoadIO como un diccionario de listas
    private Dictionary<IODirection, List<RoadIO>> ioByDirection = new Dictionary<IODirection, List<RoadIO>>();

    //Los objetos RoadIO como un diccionario por id
    private Dictionary<string, RoadIO> ioByID = new Dictionary<string, RoadIO>();

    //Los caminos por id
    private Dictionary<string, Path> pathsByName = new Dictionary<string, Path>();

    //Toda la io sin ordenar
    private RoadIO[] allIO;

    //El container de los caminos
    private PathContainer pathContainer;

    private bool awaked = false;

    //En este caso el identificador es el nombre del objeto
    public string RoadIdentifier
    {
        get
        {
            return gameObject.name;
        }
    }

    //Devuelve si es conector o no
    public bool Connector { get => connector; }

    public List<RoadIO> GetRoadIOByDirection(IODirection direction)
    {
        if (!ioByDirection.ContainsKey(direction) && !awaked)
        {
            Awake();
        }

        if (!ioByDirection.ContainsKey(direction))
        {
            return null;
        }
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
        if (!awaked)
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
            awaked = true;
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

    public bool RoadReady()
    {
        return true;
    }
}