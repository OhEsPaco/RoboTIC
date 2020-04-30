﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CagePoint : MonoBehaviour
{
   
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(this.transform.position,0.2f);
        Handles.Label(transform.position, name);
    }
}