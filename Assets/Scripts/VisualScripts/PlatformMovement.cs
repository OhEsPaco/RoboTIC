using UnityEngine;

public class PlatformMovement : MonoBehaviour
{
    public float distance = 1f;
    public float speed = 1f;
    private int dir;
    private float currentDistance;

    // Start is called before the first frame update
    private void Start()
    {
        dir = -1;
        currentDistance = 0;
    }

    // Update is called once per frame
    private void Update()
    {
        if (currentDistance >= distance)
        {
            dir = dir * -1;
            currentDistance = 0;
        }
        currentDistance += Time.deltaTime * speed;
        transform.position += Vector3.up * Time.deltaTime * speed * dir;
    }
}