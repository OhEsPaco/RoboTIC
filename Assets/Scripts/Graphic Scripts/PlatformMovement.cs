using UnityEngine;

public class PlatformMovement : MonoBehaviour
{
    public GameObject gameObject;
    public float distance;
    public float speed;
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
        gameObject.transform.position += Vector3.up * Time.deltaTime * speed * dir;
    }
}