using System;
using System.Collections.Generic;
using UnityEngine;
using static PathContainer;

public class RoadMovementLogic : MonoBehaviour
{
    private bool movementStarted = false;
    private float pathPercent = 0;

    [SerializeField] private MiniCharacter player;
    [SerializeField] private float minimumDist = 0.1f;
    private RoadInput input;
    private RoadOutput output;

    private List<Vector3> controlPath = new List<Vector3>();
    private RoadOutput nextOutput;
    private LTSpline track;
    private float carAdd;
    private int trackMaxItems = 10;
    [SerializeField] private float speed = 20;
    private void Start()
    {
        //plop the character pieces in the "Ignore Raycast" layer so we don't have false raycast data:
        foreach (Transform child in player.transform)
        {
            child.gameObject.layer = 2;
        }
        player.transform.parent = transform;
    }

    // GetPathAndOutput(in RoadInput input, out Path path, out RoadOutput output)
    public void StartMovement(RoadInput input, RoadOutput output)
    {
        this.input = input;
        this.output = output;

        this.nextOutput = null;

        player.gameObject.SetActive(true);
        player.transform.position = input.transform.position;
        if (GetNewPath(input))
        {
            movementStarted = true;
        }
        else
        {
            Debug.LogError("No path available");
        }
        pathPercent = 0;
    }

    private bool GetNewPath(RoadInput input)
    {
        Path path;
        RoadOutput outp;
        if (input.GetParentRoad().GetPathAndOutput(input, out path, out outp))
        {
            Vector3[] trackPoints = new Vector3[path.points.Length];
            for (int i = 0; i < trackPoints.Length; i++)
            {
                trackPoints[i] = path.points[i].position;
            }

            if (controlPath.Count == 0)
            {
                //Necesita al menos 4 puntos
                if (trackPoints.Length == 3)
                {
                    Array.Resize<Vector3>(ref trackPoints, 5);
                    trackPoints[4] = new Vector3(trackPoints[2].x, trackPoints[2].y, trackPoints[2].z);
                    trackPoints[2] = new Vector3(trackPoints[1].x, trackPoints[1].y, trackPoints[1].z);

                    trackPoints[1] = Vector3.Lerp(trackPoints[0], trackPoints[2], 0.5f);

                    trackPoints[3] = Vector3.Lerp(trackPoints[2], trackPoints[4], 0.5f);
                }

                if (trackPoints.Length == 2)
                {
                    Array.Resize<Vector3>(ref trackPoints, 5);
                    trackPoints[4] = new Vector3(trackPoints[1].x, trackPoints[1].y, trackPoints[1].z);

                    trackPoints[2] = Vector3.Lerp(trackPoints[0], trackPoints[4], 0.5f);
                    trackPoints[1] = Vector3.Lerp(trackPoints[0], trackPoints[2], 0.5f);
                    trackPoints[3] = Vector3.Lerp(trackPoints[2], trackPoints[4], 0.5f);
                }

                controlPath.Add(trackPoints[0]);
                foreach (Vector3 v in trackPoints)
                {
                    Debug.Log(v);
                    controlPath.Add(v);
                }
              
            }
            else
            {
                controlPath.RemoveAt(controlPath.Count-1);
                for (int i = 1; i < trackPoints.Length; i++)
                {
                    controlPath.Add(trackPoints[i]);
                }
               
            }

            while (controlPath.Count > trackMaxItems)
            {
                controlPath.RemoveAt(0); // Remove the trailing spline node
            }

            controlPath.Add(controlPath[controlPath.Count-1]);
            track = new LTSpline(controlPath.ToArray());

            carAdd = speed / track.distance; // we want to make sure the speed is based on the distance of the spline for a more constant speed
            pathPercent += carAdd * Time.deltaTime;
            Debug.Log(pathPercent);

            nextOutput = outp;

            return true;
        }

        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        foreach (Vector3 v in controlPath.ToArray())
        {
            Gizmos.DrawSphere(v, 0.1f);
        }
    }

    private void Update()
    {
        if (movementStarted)
        {
             float zLastDist = Vector3.Distance(controlPath[controlPath.Count-1], player.transform.position);
              //Debug.Log(zLastDist);
              if (zLastDist < minimumDist)
              {
                  //Debug.Log("Hola");
                  //Hace falta un nuevo camino

                  if (nextOutput == output)
                  {
                     Debug.Log("End of the road!!");
                      // movementStarted = false;
                  }
                  else
                  {
                     if (!GetNewPath((RoadInput)nextOutput.connectedTo))
                      {
                          //movementStarted = false;

                          Debug.LogError("No path available");
                      }
                      else
                      {
                          pathPercent = track.ratioAtPoint(player.transform.position);
                          Debug.Log("Marcha atras");
                      }
                  }
              }

         
            track.place(player.transform, pathPercent);
            pathPercent += carAdd * Time.deltaTime;
        }
    }
}