using UnityEngine;
using static RoadConstants;

public class LoopIn : Road
{
    [SerializeField] private LoopCounter counter;
    [SerializeField] private GameObject obstacleYes;
    [SerializeField] private GameObject obstacleNo;

    public override void ExecuteAction(in Actions action, in int[] arguments)
    {
        switch (action)
        {
            case Actions.Reset:
                obstacleYes.SetActive(false);
                obstacleNo.SetActive(false);
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

            case Actions.Lock:
                counter.Lock();
                break;

            case Actions.Unlock:
                counter.Unlock();
                break;
        }
    }
}