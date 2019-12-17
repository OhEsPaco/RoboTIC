using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadOutput : MonoBehaviour
{
    public RoadInput roadInput;

    //public RoadInput RoadInput { get => roadInput; set => roadInput = value; }

    void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position

        if (roadInput != null)
        {
            Gizmos.color = Color.red;
        }
        else
        {
            Gizmos.color = Color.cyan;
        }
       
        Gizmos.DrawSphere(transform.position, 0.1f);
    }
}
