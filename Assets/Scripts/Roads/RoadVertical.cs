using UnityEngine;
using static ButtonConstants;
using static RoadConstants;

public class RoadVertical : Road
{
    [SerializeField] private VerticalButton[] buttons = new VerticalButton[0];

    public override void ExecuteAction(in Actions action, in int[] arguments)
    {
        if (action == Actions.SetButton)
        {
            if (arguments.Length == 1)
            {
                Buttons buttonToActivate = (Buttons)arguments[0];
                foreach (VerticalButton button in buttons)
                {
                    if (button.ButtonIndex == buttonToActivate)
                    {
                        button.gameObject.SetActive(true);
                    }
                    else
                    {
                        button.gameObject.SetActive(false);
                    }
                }
            }
        }
    }
}