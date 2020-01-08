using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static RoadConstants;

public abstract class Road : MonoBehaviour
{
    private RoadInput[] inputs;
    private RoadOutput[] outputs;

    [SerializeField] private RoadType roadType = RoadType.Undetermined;

    private void Start()
    {
        inputs = GetComponentsInChildren<RoadInput>();
        outputs = GetComponentsInChildren<RoadOutput>();

        
    }

    public RoadType RoadType { get => roadType; }
    public RoadInput[] Inputs { get => inputs;  }
    public RoadOutput[] Outputs { get => outputs;  }

    public RoadOutput ReturnOutputByType(IOType iOType)
    {
        foreach(RoadOutput output in outputs)
        {
            if (output.OutputType == iOType)
            {
                return output;
            }
        }
        return null;
    }
    public abstract void ExecuteAction(Actions action,int[] arguments);
}
