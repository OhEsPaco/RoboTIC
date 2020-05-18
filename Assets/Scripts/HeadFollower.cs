using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadFollower : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        transform.position = Camera.main.transform.position;
    }
}
