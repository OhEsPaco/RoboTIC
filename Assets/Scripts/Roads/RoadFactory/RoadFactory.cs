using System.Collections.Generic;
using UnityEngine;
using static RoadIO;

public class RoadFactory : MonoBehaviour
{
    private Dictionary<string, Road> roadsByID = new Dictionary<string, Road>();
    private Road[] allRoads;
    private Road[] functionRoads;
    private Road[] connectorRoads;
    [SerializeField] private float maxGapBetweenRoads = 0.3f;

    public float MaxGapBetweenRoads { get => maxGapBetweenRoads; set => maxGapBetweenRoads = value; }

    // Start is called before the first frame update
    private void Start()
    {
        //Tomamos todas las carreteras
        allRoads = Resources.LoadAll<Road>("Prefabs/Roads");
        if (allRoads.Length == 0)
        {
            Debug.LogError("No roads found");
        }

        List<Road> tmp_functionRoads = new List<Road>();
        List<Road> tmp_connectorRoads = new List<Road>();

        foreach (Road r in allRoads)
        {
            Debug.Log("Added road: " + r.RoadIdentifier + " Connector: " + r.Connector);
            if (!roadsByID.ContainsKey(r.RoadIdentifier))
            {
                roadsByID.Add(r.RoadIdentifier, r);
                if (r.Connector)
                {
                    tmp_connectorRoads.Add(r);
                }
                else
                {
                    tmp_functionRoads.Add(r);
                }
            }
            else
            {
                Debug.LogError("A road with this name is already added: " + r.RoadIdentifier);
            }
          
        }

        functionRoads = tmp_functionRoads.ToArray();
        connectorRoads = tmp_connectorRoads.ToArray();
    }

    public bool GetRoadByID(in string id, out Road road)
    {
        if (roadsByID.ContainsKey(id))
        {
            road = roadsByID[id];
            return true;
        }
        road = null;
        return false;
    }

    public bool SpawnRoadByID(in string id, out Road road)
    {
        Road roadToSpawn;
        if (GetRoadByID(id, out roadToSpawn))
        {
            //road = Instantiate(roadToSpawn);
            road = roadToSpawn;
            return true;
        }

        road = null;
        return false;
    }

    public bool SpawnRoadByID(in string id, in List<RoadIO> ioToMatch, out Road road, out Dictionary<string, string> connectionsR_C)
    {
        Road roadToSpawn;
        connectionsR_C = null;
        road = null;

        if (GetRoadByID(id, out roadToSpawn))
        {
            if (SpawnRoad(roadToSpawn, ioToMatch, maxGapBetweenRoads, out road, out connectionsR_C))
            {
                return true;
            }
        }

        return false;
    }

    public bool SpawnRoadByID(in string id, in List<RoadIO> ioToMatch, in List<RoadIO> ioToMatch2, out Road road, out Dictionary<string, string> connectionsR1_Connector, out Dictionary<string, string> connectionsR2_Connector)
    {
        connectionsR1_Connector = null;
        connectionsR2_Connector = null;
        road = null;

        Road roadToSpawn;
        if (GetRoadByID(id, out roadToSpawn))
        {
            Road[] connectors = { roadToSpawn };
            road = ConnectRoads(connectors, ioToMatch, ioToMatch2, maxGapBetweenRoads, out connectionsR1_Connector, out connectionsR2_Connector);
            if (road != null)
            {
                return true;
            }
        }

        return false;
    }

    public bool FillGapWithConnector(in List<RoadIO> ioToMatch, in List<RoadIO> ioToMatch2, out Road road, out Dictionary<string, string> connectionsR1_Connector, out Dictionary<string, string> connectionsR2_Connector)
    {
        road = ConnectRoads(connectorRoads, ioToMatch, ioToMatch2, maxGapBetweenRoads, out connectionsR1_Connector, out connectionsR2_Connector);
        if (road != null)
        {
            return true;
        }

        road = null;
        return false;
    }

    //Dadas una lista de carreteras, una lista de io de la carretera a un lado de un hueco y una lista de io de la carretera al otro lado
    //intenta conectarlas mediante una carretera de la lista anterior
    //public bool SpawnAndConnectRoad(in Road roadToSpawn, in List<RoadIO> ioToMatch, in float errorMargin, out Road spawnedRoad)
    public Road ConnectRoads(in Road[] connectors, in List<RoadIO> ioRoad1, in List<RoadIO> ioRoad2, float errorMargin, out Dictionary<string, string> connectionsR1_Connector, out Dictionary<string, string> connectionsR2_Connector)
    {
        connectionsR1_Connector = null;
        connectionsR2_Connector = null;

        //Si alguna de las dos listas no tiene io no se pueden conectar
        if (ioRoad1.Count > 0 && ioRoad2.Count > 0)
        {
            //Comprobamos que todo io de una lista tenga la misma carretera padre y direccion
            Road parentRoad = ioRoad1[0].GetParentRoad();
            IODirection dir = ioRoad1[0].Direction;

            foreach (RoadIO rIO in ioRoad1)
            {
                if (parentRoad != rIO.GetParentRoad() || dir != rIO.Direction)
                {
                    return null;
                }
            }

            parentRoad = ioRoad2[0].GetParentRoad();
            dir = ioRoad2[0].Direction;
            foreach (RoadIO rIO in ioRoad2)
            {
                if (parentRoad != rIO.GetParentRoad() || dir != rIO.Direction)
                {
                    return null;
                }
            }

            //Comprobamos que la dirección del io de una parte del hueco sea opuesta a la del otro
            if (ioRoad1[0].Direction != RoadIO.GetOppositeDirection(ioRoad2[0].Direction))
            {
                return null;
            }
        }

        //Cada key es el id de un io de la carretera de inicio y el value, de un io del conector
        List<Dictionary<string, string>> connectionsRoad1;
        //Buscamos una lista de las carreteras que encajan con la carretera 1
        List<Road> connectorsRoad1;

        if (FindSuitableRoads(connectors, ioRoad1, errorMargin, out connectorsRoad1, out connectionsRoad1))
        {
            //Buscamos una lista de las carreteras que encajan con la carretera 1
            List<Dictionary<string, string>> connectionsRoad2;
            List<Road> connectorsRoad2;

            if (FindSuitableRoads(connectors, ioRoad2, errorMargin, out connectorsRoad2, out connectionsRoad2))
            {
                //Buscamos una carretera comun en ambas listas
                Road commonRoad = null;

                foreach (Road r1 in connectorsRoad1)
                {
                    foreach (Road r2 in connectorsRoad2)
                    {
                        if (r1.RoadIdentifier.Equals(r2.RoadIdentifier))
                        {
                            commonRoad = r1;
                            break;
                        }
                    }

                    if (commonRoad != null)
                    {
                        break;
                    }
                }

                if (commonRoad != null)
                {
                    //Si hay carretera comun, la instanciamos
                    //Road connector = Instantiate(commonRoad);
                    Road connector = commonRoad;

                    Road road1 = ioRoad1[0].GetParentRoad();
                    Road road2 = ioRoad2[0].GetParentRoad();

                    //Hacemos que el conector sea del mismo padre que road1
                    //connector.transform.parent = road1.transform.parent;

                    //Buscamos el diccionario con las conexiones que corresponden a esta carretera en las de inicio
                    connectionsR1_Connector = connectionsRoad1[connectorsRoad1.FindIndex(a => a.RoadIdentifier.Equals(commonRoad.RoadIdentifier))];
                    connectionsR2_Connector = connectionsRoad2[connectorsRoad2.FindIndex(a => a.RoadIdentifier.Equals(commonRoad.RoadIdentifier))];

                    //Devolvemos el conector
                    return connector;
                }
            }
        }

        return null;
    }

    public bool SpawnAndConnectRoad(in string roadToSpawn, in List<RoadIO> ioToMatch, in float errorMargin, out Road spawnedRoad, out Dictionary<string, string> connections)
    {
        Road r;
        connections = null;
        if (GetRoadByID(roadToSpawn, out r))
        {
            return SpawnRoad(r, ioToMatch, errorMargin, out spawnedRoad, out connections);
        }

        spawnedRoad = null;
        return false;
    }

    //Intenta spawnear la carretera que se le pide en el conjunto de io que se le pasa
    public bool SpawnRoad(in Road roadToSpawn, in List<RoadIO> ioToMatch, in float errorMargin, out Road spawnedRoad, out Dictionary<string, string> connections)
    {
        //Si alguna de las dos listas no tiene io no se pueden conectar
        if (ioToMatch.Count > 0)
        {
            //Comprobamos que todo io de una lista tenga la misma carretera padre y direccion
            Road parentRoad = ioToMatch[0].GetParentRoad();
            IODirection dir = ioToMatch[0].Direction;

            foreach (RoadIO rIO in ioToMatch)
            {
                if (parentRoad != rIO.GetParentRoad() || dir != rIO.Direction)
                {
                    spawnedRoad = null;
                    connections = null;
                    return false;
                }
            }
        }

        //Añadimos la carretera a spawnear a una lista
        Road[] connectors = { roadToSpawn };

        //Cada key es el id de un io de la carretera de inicio y el value, de un io del conector
        List<Dictionary<string, string>> connectionsRoad1;
        //Buscamos una lista de las carreteras que encajan con la carretera 1
        List<Road> connectorsRoad1;

        //Devuelve las carreteras validas si hay, en este caso solo una
        if (FindSuitableRoads(connectors, ioToMatch, errorMargin, out connectorsRoad1, out connectionsRoad1))
        {
            //Comprobamos que sean la misma por si acaso
            if (connectorsRoad1[0] != roadToSpawn)
            {
                spawnedRoad = null;
                connections = null;
                return false;
            }

           // Road connector = Instantiate(roadToSpawn);

            Road road1 = ioToMatch[0].GetParentRoad();

            //Buscamos el diccionario con las conexiones que corresponden a esta carretera en las de inicio
            connections = connectionsRoad1[connectorsRoad1.FindIndex(a => a.RoadIdentifier.Equals(connectorsRoad1[0].RoadIdentifier))];

            spawnedRoad = roadToSpawn;
            return true;
        }

        spawnedRoad = null;
        connections = null;
        return false;
    }

    //Dada una lista de carreteras y una serie de io, devuelve una lista de carreteras que encajan ahi y un dictionario con las conexiones (id de ioToMatch, id de conector)
    public bool FindSuitableRoads(in Road[] roadList, in List<RoadIO> ioToMatch, in float errorMargin, out List<Road> validRoads, out List<Dictionary<string, string>> connectionsDictionary)
    {
        if (ioToMatch.Count > 0)
        {
            IODirection oppositeDirection = RoadIO.GetOppositeDirection(ioToMatch[0].Direction);
            validRoads = new List<Road>();
            connectionsDictionary = new List<Dictionary<string, string>>();

            if (oppositeDirection != IODirection.Undefined && ioToMatch.Count > 0)
            {
                foreach (Road road in roadList)
                {
                    //Tomamos los io de la direccion opuesta
                    List<RoadIO> candidateIO = road.GetRoadIOByDirection(oppositeDirection);

                    Dictionary<string, string> connections;
                    if (CheckIfValid(ioToMatch, candidateIO, errorMargin, out connections))
                    {
                        validRoads.Add(road);
                        connectionsDictionary.Add(connections);
                    }
                }
            }

            if (validRoads.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        connectionsDictionary = null;
        validRoads = null;
        return false;
    }

    private bool CheckIfValid(in List<RoadIO> ioToMatch, in List<RoadIO> candidateIO, in float errorMargin, out Dictionary<string, string> connections)
    {
        connections = new Dictionary<string, string>();

        // if (ioToMatch.Count == 0 || candidateIO.Count == 0 || ioToMatch.Count != candidateIO.Count)
        if (ioToMatch.Count == 0 || candidateIO.Count == 0 || ioToMatch.Count != candidateIO.Count)
        {
            return false;
        }

        //El pivote puede ser cualquier IO de la carretera inicial, así que simplemente cojo el cero
        RoadIO pivot = ioToMatch[0];

        foreach (RoadIO candidatePivot in candidateIO)
        {
            //Lo que haré será "transportar" cada io de la otra carretera al pivote y ver si las condiciones quedan satisfechas
            //para todos los puntos
            Vector3 difference = pivot.transform.position - candidatePivot.transform.position;

            connections = new Dictionary<string, string>();
            foreach (RoadIO candidateNewRoad in candidateIO)
            {
                foreach (RoadIO candidateOriginalRoad in ioToMatch)
                {
                    //Si el tipo es diferente podemos continuar
                    if (candidateNewRoad.GetType() != candidateOriginalRoad.GetType())
                    {
                        //Si el id del punto en la carretera original no esta en el diccionario podemos continuar
                        if (!connections.ContainsKey(candidateOriginalRoad.IOIdentifier))
                        {
                            //Si la distancia ajustada entre los puntos es menor que el margen de error
                            //queda satisfecho
                            Vector3 pointA = candidateOriginalRoad.transform.position;
                            Vector3 pointB = candidateNewRoad.transform.position + difference;

                            if (Vector3.Distance(pointA, pointB) <= errorMargin)
                            {
                                connections.Add(candidateOriginalRoad.IOIdentifier, candidateNewRoad.IOIdentifier);
                                break;
                            }
                        }
                    }
                }
            }
            if (connections.Count == ioToMatch.Count)
            {
                return true;
            }
        }

        return false;
    }
}