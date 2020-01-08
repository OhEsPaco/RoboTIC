using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using static RoadConstants;

public class TestRoadScrp : MonoBehaviour
{
    public RoadInput start;
    public RoadOutput finish;
    public int loopReps = 3;
    public GameObject character;
    private bool keepExecuting = true;

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private void OnMouseDown()
    {
        StartCoroutine(ExampleCoroutine());
        

        //character.GetComponent<NavMeshAgent>().SetDestination(finish.transform.position);
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
            if (output.RoadInput != null)
            {
                Road nextRoad = output.RoadInput.GetParentRoad();
                RoadInput thisInput = output.RoadInput;
                switch (nextRoad.RoadType)
                {
                    case RoadConstants.RoadType.Undetermined:
                        //Nothing
                        break;

                    case RoadConstants.RoadType.CurveIn:
                        GoToPos(nextRoad.Outputs[0].transform.position);
                        break;

                    case RoadConstants.RoadType.CurveOut:
                        GoToPos(nextRoad.Outputs[0].transform.position);
                        break;

                    case RoadConstants.RoadType.Double:
                        if (thisInput.InputType == IOType.Top)
                        {
                            GoToPos(nextRoad.ReturnOutputByType(IOType.Bottom).transform.position);
                        }

                        if (thisInput.InputType == IOType.Bottom)
                        {
                            GoToPos(nextRoad.ReturnOutputByType(IOType.Top).transform.position);
                        }

                        break;

                    case RoadConstants.RoadType.HorizontalIn:
                        GoToPos(nextRoad.Outputs[0].transform.position);
                        break;

                    case RoadConstants.RoadType.HorizontalOut:
                        GoToPos(nextRoad.Outputs[0].transform.position);
                        break;

                    case RoadConstants.RoadType.Vertical:
                        GoToPos(nextRoad.Outputs[0].transform.position);
                        break;

                    case RoadConstants.RoadType.IfIn:
                        if (IsConditionMet(nextRoad))
                        {
                            Debug.Log("Condition is true");

                            ExecuteActionNoArguments(nextRoad, Actions.GoToYes);
                            GoToPos(nextRoad.ReturnOutputByType(IOType.Yes).transform.position);
                        }
                        else
                        {
                            Debug.Log("Condition is false");
                            ExecuteActionNoArguments(nextRoad, Actions.GoToNo);
                            GoToPos(nextRoad.ReturnOutputByType(IOType.No).transform.position);
                        }

                        break;

                    case RoadConstants.RoadType.IfOut:
                        GoToPos(nextRoad.Outputs[0].transform.position);
                        break;

                    case RoadConstants.RoadType.LoopIn:
                        if (loopReps > 0)
                        {
                            loopReps--;
                            SetLoopReps(start, loopReps);
                            ExecuteActionNoArguments(nextRoad, Actions.GoToYes);
                            GoToPos(nextRoad.ReturnOutputByType(IOType.Yes).transform.position);
                        }
                        else
                        {
                            SetLoopReps(start, loopReps);
                            ExecuteActionNoArguments(nextRoad, Actions.GoToNo);
                            GoToPos(nextRoad.ReturnOutputByType(IOType.No).transform.position);
                        }

                        break;

                    case RoadConstants.RoadType.LoopOut:

                        if (thisInput.InputType == IOType.Top)
                        {
                            GoToPos(nextRoad.ReturnOutputByType(IOType.Bottom).transform.position);
                        }

                        if (thisInput.InputType == IOType.Right)
                        {
                            GoToPos(nextRoad.ReturnOutputByType(IOType.Top).transform.position);
                        }

                        break;
                }
            }
            else
            {
                Debug.LogError("Unexpected output without input.");
            }
        }

        // {Undetermined, CurveIn,CurveOut,Double,HorizontalIn,HorizontalOut,Vertical,IfIn,IfOut,LoopIn,LoopOut};
    }

    public void FinishedAction(RoadOutput arrivedAt)
    {
        if (keepExecuting)
        {
            Debug.Log("Arrived at output " + arrivedAt.OutputType + " of road " + arrivedAt.GetParentRoad().RoadType);
            ProccessNextAction(arrivedAt);
        }
    }

    private void SetLoopReps(RoadInput roadInput, int loopReps)
    {
        int[] reps = { loopReps };
        roadInput.GetParentRoad().ExecuteAction(RoadConstants.Actions.SetCounter, reps);
        character.GetComponent<NavMeshAgent>().SetDestination(finish.transform.position);
    }

    private void ExecuteActionNoArguments(Road road, Actions action)
    {
        int[] reps = { };
        road.ExecuteAction(action, reps);
    }

    private bool IsConditionMet(Road road)
    {
        System.Random gen = new System.Random();

        int prob = gen.Next(100);
        return prob <= 50;
    }


    IEnumerator ExampleCoroutine()
    {

        SetLoopReps(start, loopReps);

        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(5);

        SetLoopReps(start, loopReps);

        if (loopReps > 0)
        {
            loopReps--;
             SetLoopReps(start, loopReps);
            ExecuteActionNoArguments(start.GetParentRoad(), Actions.GoToYes);
            GoToPos(start.GetParentRoad().ReturnOutputByType(IOType.Yes).transform.position);
        }
        else
        {
            SetLoopReps(start, loopReps);
            ExecuteActionNoArguments(start.GetParentRoad(), Actions.GoToNo);
            GoToPos(start.GetParentRoad().ReturnOutputByType(IOType.No).transform.position);
        }

    }
}