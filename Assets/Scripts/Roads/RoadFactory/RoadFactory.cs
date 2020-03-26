using System.Collections.Generic;
using UnityEngine;
using static RoadIO;

public class RoadFactory : MonoBehaviour
{
    private Road[] roads;

    // Start is called before the first frame update
    private void Start()
    {
        roads = GetComponentsInChildren<Road>();
    }

    //Puedo ejecutar el algoritmo para un lado de un espacio, luego para el otro y si tienen pieza en comun pues se usa esa
    public List<Road> FindRoads(in List<RoadIO> ioToMatch, in IODirection iODirection, in float errorMargin, out List<RoadIO> ioToAttatchOriginalPiece, out List<RoadIO> ioToAttatchNewPiece)
    {
        IODirection oppositeDirection = RoadIO.GetOppositeDirection(iODirection);
        List<Road> validRoads = new List<Road>();
        ioToAttatchOriginalPiece = new List<RoadIO>();
        ioToAttatchNewPiece = new List<RoadIO>();

        if (oppositeDirection != IODirection.Undefined && ioToMatch.Count > 0)
        {
            foreach (Road road in roads)
            {
                //Tomamos los io de la direccion opuesta
                List<RoadIO> candidateIO = road.GetRoadIOByDirection(oppositeDirection);

                //Si la cuenta es la misma, continuamos
                if (candidateIO.Count == ioToMatch.Count)
                {
                    //Que el numero de inputs de uno sea igual que el numero de outputs de otro
                    if (GetNumberOfInputs(ioToMatch) == GetNumberOfOutputs(candidateIO))
                    {
                        //Elegimos un pivote, por ejemplo el primer io de ioToMatch
                        for (int i = 0; i < candidateIO.Count; i++)
                        {
                            if (CheckIfValid(ioToMatch, candidateIO, 0, i, errorMargin))
                            {
                                validRoads.Add(road);
                                ioToAttatchOriginalPiece.Add(ioToMatch[0]);
                                ioToAttatchNewPiece.Add(candidateIO[i]);
                            }
                        }
                    }
                }
            }
        }

        return validRoads;
    }

    private bool CheckIfValid(in List<RoadIO> ioToMatch, in List<RoadIO> candidateIO, in int ioToMatchIndex, in int candidateIndex, in float errorMargin)
    {
        RoadIO pivot = ioToMatch[ioToMatchIndex];
        RoadIO ioCandidate = candidateIO[candidateIndex];

        if (pivot.GetType() == ioCandidate.GetType())
        {
            return false;
        }
        else
        {
            Vector3 difference = pivot.transform.position - ioCandidate.transform.position;

            for (int i = 0; i < ioToMatch.Count; i++)
            {
                bool isSatisfaced = false;

                for (int j = 0; j < candidateIO.Count || isSatisfaced; j++)
                {
                    //Si el tipo es diferente podemos continuar
                    if (ioToMatch[i].GetType() != candidateIO[j].GetType())
                    {
                        //Si la distancia ajustada entre los puntos es menor que el margen de error
                        //queda satisfecho
                        Vector3 pointA = ioToMatch[i].transform.position;
                        Vector3 pointB = candidateIO[j].transform.position + difference;

                        if (Vector3.Distance(pointA, pointB) <= errorMargin)
                        {
                            isSatisfaced = true;
                        }
                    }
                }

                if (!isSatisfaced)
                {
                    return false;
                }
            }
        }

        return true;
    }

    private int GetNumberOfInputs(List<RoadIO> list)
    {
        int count = 0;

        foreach (RoadIO io in list)
        {
            if (io is RoadInput)
            {
                count++;
            }
        }

        return count;
    }

    private int GetNumberOfOutputs(List<RoadIO> list)
    {
        int count = 0;

        foreach (RoadIO io in list)
        {
            if (io is RoadOutput)
            {
                count++;
            }
        }

        return count;
    }
}