using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionRenderer : MonoBehaviour
{
    public float actionSpeed = 1f;
    public float rotationSpeedMultiplier = 100f;
    private LevelManager manager;
    void Awake()
    {
        manager = LevelManager.instance;

      
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void DoTurnRight(GameObject player)
    {
        StartCoroutine(TurnCoroutine(rotationSpeedMultiplier * actionSpeed, player, 90f, false));
    }

    public void DoTurnLeft(GameObject player)
    {
        StartCoroutine(TurnCoroutine(rotationSpeedMultiplier*actionSpeed, player, 90f,true)); 
    }

    IEnumerator TurnCoroutine(float speed, GameObject player,float degrees,bool left)
    {
        int sign = left?-1:1;
        
        for (float currentDegrees = 0f; currentDegrees <= degrees; currentDegrees += speed * Time.deltaTime)
        {

            float step = sign*(speed * Time.deltaTime);
            player.transform.Rotate(0, step, 0);
            yield return null;
        }
        NotifyEndOfAction();


    }

    public void DoMove(int playerOrientation, GameObject player)
    {
        Vector3 target=new Vector3(0,0,0);
        //0 - z+
        //1 - x+
        //2 - z-
        //3 - x-
        //90f * data.playerOrientation
        switch (playerOrientation)
        {
            case 0:
                 target = new Vector3(player.transform.position.x, player.transform.transform.position.y, player.transform.transform.position.z+ manager.MapRenderer.blockLength);
                
                break;
            case 1:
                 target = new Vector3(player.transform.position.x+ manager.MapRenderer.blockLength, player.transform.transform.position.y, player.transform.transform.position.z);
                
                break;
            case 2:
                 target = new Vector3(player.transform.position.x, player.transform.transform.position.y, player.transform.transform.position.z- manager.MapRenderer.blockLength);
               
                break;
            case 3:
                 target = new Vector3(player.transform.position.x- manager.MapRenderer.blockLength, player.transform.transform.position.y, player.transform.transform.position.z);
               
                break;
            default:
                Debug.LogError("Unknown orientation");
                NotifyEndOfAction();
                return;
        }

        StartCoroutine(MoveCoroutine(actionSpeed, player, target, manager.MapRenderer.blockLength));
    }

    private void NotifyEndOfAction()
    {
        manager.Logic.NotifyEndOfAction();
    }

    IEnumerator MoveCoroutine(float speed,GameObject player, Vector3 target,float distance)
    {
        
        for(float currentDistance=0f;currentDistance<=distance;currentDistance+= speed * Time.deltaTime)
        {
            
            float step = speed * Time.deltaTime;
            player.transform.position = Vector3.MoveTowards(player.transform.position, target, step);
            yield return null;
        }
        NotifyEndOfAction();


    }
}
