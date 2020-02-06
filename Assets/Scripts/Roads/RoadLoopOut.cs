using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadLoopOut : Road
{

    [SerializeField] private RoutePoint routePoint;

    public RoutePoint RoutePoint { get => routePoint;  }

    public override void ExecuteAction(in RoadConstants.Actions action, in int[] arguments)
    {
       //Do nothing
    }
}
