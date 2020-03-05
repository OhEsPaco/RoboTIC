using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static CardConstants;
using static RoadConstants;

public class RoadMovementLogic : MonoBehaviour
{
    private RoadInput start;
    private RoadOutput finish;
    private GameObject character;
    private bool keepExecuting = true;
    [SerializeField] private NavMeshSurface navMesh;
    [SerializeField] private GameObject roadScaler;
    private Dictionary<Road, Cards> conditionDictionary;
    private Dictionary<Road, int> loopsDictionary;

    public void StartMovement(RoadInput start, RoadOutput finish, GameObject character, Dictionary<Road, Cards> conditionDictionary, Dictionary<Road, int> loopsDictionary)
    {
        this.keepExecuting = true;
        this.start = start;
        this.finish = finish;
        this.character = character;
        this.character.transform.parent = roadScaler.transform.parent;
        this.character.transform.localScale = roadScaler.transform.localScale;
        this.conditionDictionary = conditionDictionary;
        this.loopsDictionary = loopsDictionary;
        navMesh.BuildNavMesh();
        ProccessNextAction(start);
    }

    public void CancelMovement()
    {
        keepExecuting = false;
    }
    private void GoToPos(Vector3 position)
    {
        character.GetComponent<NavMeshAgent>().SetDestination(position);
    }

    private void ProccessNextAction(RoadOutput output)
    {
        if (output == finish)
        {
            Debug.Log("FINISH!!!!");
            keepExecuting = false;
        }
        else
        {
            if (output.ConnectedTo() != null)
            {
                RoadInput thisInput = (RoadInput)output.ConnectedTo();
                ProccessNextAction(thisInput);
            }
            else
            {
                Debug.LogError("Unexpected output without input.");
            }
        }

        // {Undetermined, CurveIn,CurveOut,Double,HorizontalIn,HorizontalOut,Vertical,IfIn,IfOut,LoopIn,LoopOut};
    }

    private void ProccessNextAction(RoadInput thisInput)
    {
        Road nextRoad = thisInput.GetParentRoad();
        RoadIO nextIO = null;
        switch (nextRoad.RoadType)
        {
            case RoadConstants.RoadType.Undetermined:
                //Nothing
                break;

            case RoadConstants.RoadType.Double:
                if (thisInput.IoType == IOType.No)
                {
                    nextIO = nextRoad.ReturnByIOAndDirection(InputOutput.Output, PointingTo.Forward)[0];
                }

                if (thisInput.IoType == IOType.Generic)
                {
                    nextIO = nextRoad.ReturnByIOAndDirection(InputOutput.Output, PointingTo.Back)[0];
                }

                break;

            case RoadConstants.RoadType.Vertical:
                nextIO = nextRoad.Outputs()[0];
                break;

            case RoadConstants.RoadType.IfIn:

                if (conditionDictionary.ContainsKey(nextRoad))
                {
                    if (IsConditionMet(nextRoad))
                    {
                        Debug.Log("Condition is true");

                        ExecuteActionNoArguments(nextRoad, Actions.GoToYes);
                        nextIO = nextRoad.ReturnByIOAndDirectionAndType(InputOutput.Output, PointingTo.Forward, IOType.Yes)[0];
                    }
                    else
                    {
                        Debug.Log("Condition is false");
                        ExecuteActionNoArguments(nextRoad, Actions.GoToNo);
                        nextIO = nextRoad.ReturnByIOAndDirectionAndType(InputOutput.Output, PointingTo.Forward, IOType.No)[0];
                    }
                }
                else
                {
                    Debug.LogWarning("Unrecognized IF");
                }

                break;

            case RoadConstants.RoadType.IfOut:
                nextIO = nextRoad.Outputs()[0];
                break;

            case RoadConstants.RoadType.LoopIn:
                if (loopsDictionary.ContainsKey(nextRoad))
                {
                    LoopIn loopIn = (LoopIn)nextRoad;
                    if (loopsDictionary[nextRoad] > 0)
                    {
                        loopsDictionary[nextRoad]--;
                        SetLoopReps(thisInput, loopsDictionary[nextRoad]);
                        ExecuteActionNoArguments(nextRoad, Actions.GoToYes);
                        nextRoad.SetThisOutputActive(nextRoad.ReturnByIOAndDirectionAndType(InputOutput.Output, PointingTo.Forward, IOType.Yes)[0]);
                        GoToPos(loopIn.RoutePoint.transform.position);
                    }
                    else
                    {
                        SetLoopReps(thisInput, loopsDictionary[nextRoad]);
                        ExecuteActionNoArguments(nextRoad, Actions.GoToNo);
                        nextIO = nextRoad.ReturnByIOAndDirectionAndType(InputOutput.Output, PointingTo.Forward, IOType.No)[0];
                    }
                }
                else
                {
                    Debug.LogWarning("Unrecognized loop");
                }

                break;

            case RoadConstants.RoadType.LoopOut:
                RoadLoopOut roadLoopOut = (RoadLoopOut)nextRoad;

                if (thisInput.IoType == IOType.No)
                {
                    nextIO = nextRoad.ReturnByIOAndDirection(InputOutput.Output, PointingTo.Forward)[0];
                }

                if (thisInput.IoType == IOType.Yes)
                {
                    nextRoad.SetThisOutputActive(nextRoad.ReturnByIOAndDirection(InputOutput.Output, PointingTo.Back)[0]);
                    GoToPos(roadLoopOut.RoutePoint.transform.position);
                }

                break;
        }
        if (nextIO != null)
        {
            nextRoad.SetThisOutputActive(nextIO);
            GoToPos(nextIO.transform.position);
        }
    }

    public void FinishedAction(RoadOutput arrivedAt)
    {
        if (keepExecuting)
        {
            Debug.Log("Arrived at output " + arrivedAt.IoType + " of road " + arrivedAt.GetParentRoad().RoadType);
            ProccessNextAction(arrivedAt);
        }
    }

    public void FinishedAction(Vector3 nextPos)
    {
        if (keepExecuting)
        {
            Debug.Log("Next point: " + nextPos);
            GoToPos(nextPos);
        }
    }

    private void SetLoopReps(RoadInput roadInput, int loopReps)
    {
        int[] reps = { loopReps };
        roadInput.GetParentRoad().ExecuteAction(RoadConstants.Actions.SetCounter, reps);
    }

    private void ExecuteActionNoArguments(Road road, Actions action)
    {
        int[] reps = { };
        road.ExecuteAction(action, reps);
    }

    private bool IsConditionMet(Road road)
    {
        switch (conditionDictionary[road])
        {
            case Cards.NoCard:
                Debug.LogError("No card selected");
                break;

            case Cards.IsWalkable:

                Debug.Log("Card: " + Cards.IsWalkable.ToString());
                return LevelManager.instance.Logic.IsWalkable();
                break;
        }

        return false;
    }
}