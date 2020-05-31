using UnityEngine;
using static PathContainer;

public class NodeIfIn : Road
{
    [SerializeField] private RoadInput inputIf;
    [SerializeField] private ConditionCardPicker cPicker;
    [SerializeField] private GameLogic gameLogic;

    public override void ExecuteAction(in string[] args)
    {
        if (args.Length > 0)
        {
            switch (args[0])
            {
                case "lock":
                    cPicker.Lock();
                    Debug.Log("Picker locked.");
                    break;

                case "unlock":
                    cPicker.Unlock();
                    Debug.Log("Picker unlocked.");
                    break;

                default:
                    Debug.LogWarning("Undefined action: " + args[0]);
                    break;
            }
        }
    }

    public override bool GetPathAndOutput(in RoadInput input, out Path path, out RoadOutput output)
    {
        if (input == inputIf)
        {
            if (IsConditionMet())
            {
                GetPathByName("Yes", out path);
            }
            else
            {
                GetPathByName("No", out path);
            }

            output = path.ioEnd;
            return true;
        }

        path = new Path();
        output = null;
        return false;
    }

    private bool IsConditionMet()
    {
        //Comunicarse con la logica
        return gameLogic.CheckNextBlockDownProperty(cPicker.GetCardProperty());
    }
}