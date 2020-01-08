using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static RoadConstants;

public class RoadOutput : MonoBehaviour
{
    public static float InputDetectionDistance = 0.1f;
    private RoadInput roadInput;
    private Color uniqueColor=Color.red;
    //public RoadInput RoadInput { get => roadInput; set => roadInput = value; }
    [SerializeField] private IOType type = IOType.Generic;
    public IOType OutputType { get => type; }
    public RoadInput RoadInput { get => roadInput;  }

    void OnDrawGizmos()
    {
        

        if (RoadInput != null)
        {
            
            RoadInput.GizmoColor = uniqueColor;
         
        }
        Gizmos.color = uniqueColor;
        Gizmos.DrawSphere(transform.position, 0.1f);
    }

     void Start()
    {
        uniqueColor = RandomColor();
        GameObject closestInput = FindClosestInput(InputDetectionDistance);
        if (closestInput != null)
        {
            roadInput = closestInput.GetComponent<RoadInput>();
        }
    }

    private GameObject FindClosestInput(float distance)
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("Input");
        GameObject closest = null;
       
        Vector3 position = transform.position;

        foreach (GameObject go in gos)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }
        return closest;
    }

    // Generate a random color value.
    private Color RandomColor()
    {
        // A different random value is used for each color component (if
        // the same is used for R, G and B, a shade of grey is produced).
        return new Color(Random.value, Random.value, Random.value);
    }

    public Road GetParentRoad()
    {
        return transform.parent.GetComponent<Road>();
    }

    private void OnTriggerEnter(Collider other)
    {
        LevelManager.instance.TestRoad.FinishedAction(this);
    }

}
