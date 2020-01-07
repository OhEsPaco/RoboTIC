using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRoadScrp : MonoBehaviour
{
   public RoadInput start;
    public RoadOutput finish;
    public int loopReps = 3;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        int[] reps = { loopReps };
        start.GetParentRoad().ExecuteAction(RoadConstants.Actions.SetCounter,reps);
    }

    public void FinishedAction(RoadOutput arrivedAt)
    {

    }
}
