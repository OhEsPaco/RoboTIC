using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeIfOut : Road
{

    [SerializeField] private RoadInput roadInputYes;
    [SerializeField] private RoadInput roadInputNo;
    [SerializeField] private RoadOutput roadOutput;

    public RoadInput RoadInputYes { get => roadInputYes;  }
    public RoadInput RoadInputNo { get => roadInputNo;  }
    public RoadOutput RoadOutput { get => roadOutput; }

    public override RoadOutput DoYourThing(GameObject character, RoadInput roadInput, float speed)
    {
        throw new System.NotImplementedException();
    }

    public override RoadOutput DoYourThing(GameObject character, RoadInput roadInput)
    {
        throw new System.NotImplementedException();
    }

    public override bool HaveYouFinishedYourThing()
    {
        throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
