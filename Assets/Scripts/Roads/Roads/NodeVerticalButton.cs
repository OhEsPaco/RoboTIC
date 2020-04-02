using UnityEngine;
using static PathContainer;

public class NodeVerticalButton : Road
{
    [SerializeField] private VerticalButton[] buttonList = new VerticalButton[0];
    [SerializeField] private RoadInput rInput;

    public override void ExecuteAction(in string[] args)
    {
        if (args.Length > 0)
        {
            switch (args[0])
            {
                case "activate":
                    if (args.Length > 1)
                    {
                        string buttonName = args[1];
                        bool foundButton = false;
                        foreach (VerticalButton vbutton in buttonList)
                        {
                            if (vbutton.ButtonName.Equals(buttonName))
                            {
                                vbutton.gameObject.SetActive(true);
                                foundButton = true;
                            }
                            else
                            {
                                vbutton.gameObject.SetActive(false);
                            }
                        }

                        if (foundButton)
                        {
                            Debug.Log("Button " + buttonName + " activated.");
                        }
                        else
                        {
                            Debug.Log("Button " + buttonName + " not found.");
                        }
                    }

                    break;

                case "lock":
                    foreach (VerticalButton vbutton in buttonList)
                    {
                        vbutton.Lock();
                    }
                    Debug.Log("Buttons locked.");
                    break;

                case "unlock":
                    foreach (VerticalButton vbutton in buttonList)
                    {
                        vbutton.Unlock();
                        Debug.Log("Buttons unlocked.");
                    }
                    break;

                default:
                    Debug.LogWarning("Undefined action: " + args[0]);
                    break;
            }
        }
    }

    public override bool GetPathAndOutput(in RoadInput input, out Path path, out RoadOutput output)
    {
        if (input == rInput)
        {
            GetPathByName("Path", out path);
            output = path.ioEnd;
            return true;
        }

        path = new Path();
        output = null;
        return false;
    }
}