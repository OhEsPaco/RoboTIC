using UnityEngine;
using static RoadConstants;

public class IfIn : Road
{
    [SerializeField] private GameObject obstacleYes;
    [SerializeField] private GameObject obstacleNo;
    [SerializeField] private CardPicker cardPicker;

    public override void ExecuteAction(in Actions action, in int[] arguments)
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

            case Actions.Lock:
                cardPicker.Lock();
                break;

            case Actions.Unlock:
                cardPicker.Unlock();
                break;
        }
    }
}