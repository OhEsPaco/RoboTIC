using System.Collections.Generic;
using UnityEngine;
using static RoadIO;

public class RoadFactory : MonoBehaviour
{
    [SerializeField] private Road[] roadConnectors = new Road[0];
    [SerializeField] private Road[] funcionRoads = new Road[0];
    [SerializeField] private Transform roadStartMarker;

    // Start is called before the first frame update
    private void Start()
    {
    }

    //Dadas una lista de carreteras, una lista de io de la carretera a un lado de un hueco y una lista de io de la carretera al otro lado
    //intenta conectarlas mediante una carretera de la lista anterior
    public Road ConnectRoads(in Road[] connectors, in List<RoadIO> ioRoad1, in List<RoadIO> ioRoad2, float errorMargin)
    {
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
        List<Road> connectorsRoad1 = FindSuitableRoads(connectors, ioRoad1, errorMargin, out connectionsRoad1);

        if (connectorsRoad1.Count > 0)
        {
            //Buscamos una lista de las carreteras que encajan con la carretera 1
            List<Dictionary<string, string>> connectionsRoad2;
            List<Road> connectorsRoad2 = FindSuitableRoads(connectors, ioRoad2, errorMargin, out connectionsRoad2);

            if (connectorsRoad2.Count > 0)
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
                    Road connector = Instantiate(commonRoad);

                    Road road1 = ioRoad1[0].GetParentRoad();
                    Road road2 = ioRoad2[0].GetParentRoad();

                    //Hacemos que el conector sea del mismo padre que road1
                    connector.transform.parent = road1.transform.parent;

                    //Buscamos el diccionario con las conexiones que corresponden a esta carretera en las de inicio
                    Dictionary<string, string> ioIdentifiersDictionary1 = connectionsRoad1[connectorsRoad1.FindIndex(a => a.RoadIdentifier.Equals(commonRoad.RoadIdentifier))];
                    Dictionary<string, string> ioIdentifiersDictionary2 = connectionsRoad2[connectorsRoad2.FindIndex(a => a.RoadIdentifier.Equals(commonRoad.RoadIdentifier))];

                    //Conectamos una carreteraa al conector
                    foreach (KeyValuePair<string, string> entry in ioIdentifiersDictionary1)
                    {
                        RoadIO originalIO = road1.GetRoadIOByID(entry.Key);
                        RoadIO connectorIO = connector.GetRoadIOByID(entry.Value);

                        originalIO.ConnectedTo = connectorIO;
                        connectorIO.MoveRoadTo(originalIO.transform.position);
                    }

                    //Conectamos la otra carretera al conector
                    foreach (KeyValuePair<string, string> entry in ioIdentifiersDictionary2)
                    {
                        RoadIO originalIO = road2.GetRoadIOByID(entry.Key);
                        RoadIO connectorIO = connector.GetRoadIOByID(entry.Value);

                        originalIO.ConnectedTo = connectorIO;
                        originalIO.MoveRoadTo(connectorIO.transform.position);
                    }

                    //Devolvemos el conector
                    return connector;
                }
            }
        }
        return null;
    }

    //Intenta spawnear la carretera que se le pide en el conjunto de io que se le pasa
    public bool SpawnAndConnectRoad(in Road roadToSpawn, in List<RoadIO> ioToMatch, in float errorMargin)
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
                    return false;
                }
            }
        }

        //Añadimos la carretera a spawnear a una lista
        Road[] connectors = { roadToSpawn };

        //Cada key es el id de un io de la carretera de inicio y el value, de un io del conector
        List<Dictionary<string, string>> connectionsRoad1;
        //Buscamos una lista de las carreteras que encajan con la carretera 1
        List<Road> connectorsRoad1 = FindSuitableRoads(connectors, ioToMatch, errorMargin, out connectionsRoad1);

        //Devuelve las carreteras validas si hay, en este caso solo una
        if (connectorsRoad1.Count == 1)
        {
            //Comprobamos que sean la misma por si acaso
            if (connectorsRoad1[0] != roadToSpawn)
            {
                return false;
            }

            Road connector = Instantiate(roadToSpawn);

            Road road1 = ioToMatch[0].GetParentRoad();

            //Hacemos que el conector sea del mismo padre que road1
            connector.transform.parent = road1.transform.parent;

            //Buscamos el diccionario con las conexiones que corresponden a esta carretera en las de inicio
            Dictionary<string, string> ioIdentifiersDictionary1 = connectionsRoad1[connectorsRoad1.FindIndex(a => a.RoadIdentifier.Equals(connectorsRoad1[0].RoadIdentifier))];

            //Conectamos una carreteraa al conector
            foreach (KeyValuePair<string, string> entry in ioIdentifiersDictionary1)
            {
                RoadIO originalIO = road1.GetRoadIOByID(entry.Key);
                RoadIO connectorIO = connector.GetRoadIOByID(entry.Value);

                originalIO.ConnectedTo = connectorIO;
                connectorIO.MoveRoadTo(originalIO.transform.position);
            }

            return true;
        }

        return false;
    }

    //Dada una lista de carreteras y una serie de io, devuelve una lista de carreteras que encajan ahi y un dictionario con las conexiones (id de ioToMatch, id de conector)
    public List<Road> FindSuitableRoads(in Road[] roadList, in List<RoadIO> ioToMatch, in float errorMargin, out List<Dictionary<string, string>> connectionsDictionary)
    {
        connectionsDictionary = new List<Dictionary<string, string>>();
        if (ioToMatch.Count > 0)
        {
            IODirection oppositeDirection = RoadIO.GetOppositeDirection(ioToMatch[0].Direction);
            List<Road> validRoads = new List<Road>();

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

            return validRoads;
        }
        return null;
    }

    private bool CheckIfValid(in List<RoadIO> ioToMatch, in List<RoadIO> candidateIO, in float errorMargin, out Dictionary<string, string> connections)
    {
        connections = new Dictionary<string, string>();

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

    private void CorrectRoadsPosition(RoadIO pivotIO, in float maxAcceptableDistance)
    {
        if (pivotIO != null)
        {
            pivotIO.MoveRoadTo(roadStartMarker.position);

            List<Road> processedRoads = new List<Road>();
            processedRoads.Add(pivotIO.GetParentRoad());

            Stack<RoadIO> ioToProc = new Stack<RoadIO>();

            RoadIO[] tmpe = pivotIO.GetParentRoad().GetAllIO();

            foreach (RoadIO rio in tmpe)
            {
                ioToProc.Push(rio);
            }

            while (ioToProc.Count > 0)
            {
                RoadIO toProc = ioToProc.Pop();
                RoadIO connectedTo = toProc.ConnectedTo;
                if (connectedTo != null)
                {
                    float distance = Vector3.Distance(toProc.transform.position, connectedTo.transform.position);
                    if (distance > maxAcceptableDistance)
                    {
                        connectedTo.MoveRoadTo(toProc.transform.position);
                    }

                    processedRoads.Add(connectedTo.GetParentRoad());

                    tmpe = connectedTo.GetParentRoad().GetAllIO();

                    foreach (RoadIO rio in tmpe)
                    {
                        if (rio.ConnectedTo != null)
                        {
                            if (!processedRoads.Contains(rio.ConnectedTo.GetParentRoad()))
                            {
                                ioToProc.Push(rio);
                            }
                        }
                    }
                }
            }
        }
    }
}