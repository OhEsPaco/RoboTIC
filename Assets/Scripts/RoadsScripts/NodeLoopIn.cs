using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeLoopIn : Road
{
    [SerializeField] private RoadInput roadInputTop;
    [SerializeField] private RoadInput roadInputDown;
    [SerializeField] private RoadOutput roadOutputYes;
    [SerializeField] private RoadOutput roadOutputNo;

    public RoadInput RoadInputTop { get => roadInputTop;  }
    public RoadInput RoadInputDown { get => roadInputDown;  }
    public RoadOutput RoadOutputYes { get => roadOutputYes;  }
    public RoadOutput RoadOutputNo { get => roadOutputNo;  }

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
