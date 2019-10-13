using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMover : MonoBehaviour
{
    public GameObject platform;
    public float distance=1f;
    public float speed=1f;
    private int direction;
    private float currentDistance;

    // Start is called before the first frame update
    private void Start()
    {
        direction = -1;
        currentDistance = 0;
    }

    // Update is called once per frame
    private void Update()
    {
        if (currentDistance >= distance)
        {
            direction = direction * -1;
            currentDistance = 0;
        }
        currentDistance += Time.deltaTime * speed;
        platform.transform.position += Vector3.up * Time.deltaTime * speed * direction;
    }
}
