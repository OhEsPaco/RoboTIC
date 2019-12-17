﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadInput : MonoBehaviour
{

    void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 0.1f);
    }
}