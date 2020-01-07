using UnityEngine;
using static RoadConstants;

public class LoopIn : Road
{
    [SerializeField] private Counter counter;
    [SerializeField] private GameObject obstacleYes;
    [SerializeField] private GameObject obstacleNo;

    public override void ExecuteAction(Actions action, int[] arguments)
    {
      
        switch (action)
        {
            case Actions.Reset:
                obstacleYes.SetActive(true);
                obstacleNo.SetActive(true);
                break;

            case Actions.GoToNo:
                obstacleYes.SetActive(false);
                obstacleNo.SetActive(true);
                break;

            case Actions.GoToYes:
                obstacleYes.SetActive(true);
                obstacleNo.SetActive(false);
                break;

            case Actions.SetCounter:
                if (arguments.Length == 1)
                {
                    counter.SetNumber(arguments[0]);
                }
                break;
        }
    }
}