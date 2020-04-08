using UnityEngine;
using static PathContainer;

public class RoadMovementLogic : MonoBehaviour
{
    public enum Direction { Forward, Reverse };

    private Direction characterDirection;
    private bool movementStarted = false;
    private float pathPercent = 0;

    [SerializeField] private float speed = .2f;
    [SerializeField] private MiniCharacter player;
    private RoadInput input;
    private RoadOutput output;
    private Vector3 floorPosition;
    private Transform[] controlPath;
    private RoadOutput nextOutput;
    private float lookAheadAmount = .01f;

    private void Start()
    {
        //plop the character pieces in the "Ignore Raycast" layer so we don't have false raycast data:
        foreach (Transform child in player.transform)
        {
            child.gameObject.layer = 2;
        }
        player.transform.parent = transform;
        characterDirection = Direction.Forward;
    }

    // GetPathAndOutput(in RoadInput input, out Path path, out RoadOutput output)
    public void StartMovement(RoadInput input, RoadOutput output)
    {
        this.input = input;
        this.output = output;
        this.controlPath = null;
        this.nextOutput = null;

        pathPercent = 0;
        player.gameObject.SetActive(true);
        if (GetNewPath(input))
        {
            movementStarted = true;
        }
        else
        {
            Debug.LogError("No path available");
        }
    }

    private bool GetNewPath(RoadInput input)
    {
        Path path;
        RoadOutput outp;
        if (input.GetParentRoad().GetPathAndOutput(input, out path, out outp))
        {
            controlPath = path.points;
            nextOutput = outp;
            floorPosition = input.transform.position;
            return true;
        }

        return false;
    }

    private void Update()
    {
        if (movementStarted)
        {
            pathPercent += Time.deltaTime * speed;

            if (pathPercent > 1)
            {
                //Hace falta un nuevo camino
                pathPercent = pathPercent - 1;

                if (nextOutput == output)
                {
                    Debug.Log("End of the road!!");
                    movementStarted = false;
                }
                else
                {
                    if (!GetNewPath((RoadInput)nextOutput.connectedTo))
                    {
                        movementStarted = false;

                        Debug.LogError("No path available");
                    }
                }
            }

            if (movementStarted)
            {
                Vector3 coordinateOnPath = iTween.PointOnPath(controlPath, pathPercent);
                
                Vector3 lookTarget;
                //calculate look data if we aren't going to be looking beyond the extents of the path:
                if (pathPercent - lookAheadAmount >= 0 && pathPercent + lookAheadAmount <= 1)
                {
                    //leading or trailing point so we can have something to look at:
                    if (characterDirection == Direction.Forward)
                    {
                        lookTarget = iTween.PointOnPath(controlPath, pathPercent + lookAheadAmount);
                    }
                    else
                    {
                        lookTarget = iTween.PointOnPath(controlPath, pathPercent - lookAheadAmount);
                    }

                    //look:
                    player.transform.LookAt(lookTarget);

                    //nullify all rotations but y since we just want to look where we are going:
                    float yRot = player.transform.eulerAngles.y;
                    player.transform.eulerAngles = new Vector3(0, yRot, 0);
                }

                player.transform.position = new Vector3(coordinateOnPath.x, floorPosition.y, coordinateOnPath.z);
            }
        }
    }
}