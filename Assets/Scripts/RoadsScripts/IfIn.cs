using UnityEngine;
using static RoadConstants;

public class IfIn : Road
{
    [SerializeField] private GameObject obstacleYes;
    [SerializeField] private GameObject obstacleNo;

    public override void ExecuteAction(Actions action, int[] arguments)
    {
        
        switch (action)
        {
            case Actions.Reset:
                obstacleYes.SetActive(false);
                obstacleNo.SetActive(false);
                break;

            case Actions.GoToNo:
                obstacleYes.SetActive(true);
                obstacleNo.SetActive(false);
                break;

            case Actions.GoToYes:
                obstacleYes.SetActive(false);
                obstacleNo.SetActive(true);
                break;
        }
    }
}