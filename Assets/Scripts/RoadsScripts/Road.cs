using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Road : MonoBehaviour
{
    [SerializeField] private Vector3 buttonPosition;
    [SerializeField] private Quaternion buttonRotation;
    private GameObject buttonGameObject;
    [SerializeField] private float speed = 1f;
    public float Speed { get => speed; set => speed = value; }

    //in out
    //1   1
    //1   2
    //2   1
    //2   2

    private void RenderButton()
    {

    }
    public abstract RoadOutput DoYourThing(GameObject character, RoadInput roadInput, float speed);
    public abstract RoadOutput DoYourThing(GameObject character, RoadInput roadInput);
    public abstract bool HaveYouFinishedYourThing();

}
