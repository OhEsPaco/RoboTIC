using System.Collections;
using System;
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

    public void DoJump(GameObject player, List<int> currentBlock, List<int> intendedBlock, int playerOrientation)
    {
        Vector3 target = new Vector3(player.transform.position.x + (intendedBlock[0] - currentBlock[0]) * manager.MapRenderer.blockLength,
            player.transform.position.y + (intendedBlock[1] - currentBlock[1]) * manager.MapRenderer.blockLength,
            player.transform.position.z + (intendedBlock[2] - currentBlock[2]) * manager.MapRenderer.blockLength);
        bool isX = true;
        //0 - z+
        //1 - x+
        //2 - z-
        //3 - x-
        switch (playerOrientation)
        {
            case 0:
               
                isX = false;
                break;
            case 1:
                
                isX = true;
                break;
            case 2:
               
                isX = false;
                break;
            case 3:
                
                isX = true;
                break;
            default:
                Debug.LogError("Unknown orientation");
                NotifyEndOfAction();
                return;
        }


        StartCoroutine(JumpCoroutine(player, target, actionSpeed,isX,0.5f));
    }

    IEnumerator JumpCoroutine(GameObject player, Vector3 target,float speed,bool isX,float arcHeight)
    {


        // Compute the next position, with arc added in
        float x0 = isX?player.transform.position.x: player.transform.position.z;
        float x1 = isX?target.x:target.z;
        float dist = x1 - x0;

        float totalDist = Mathf.Abs(dist);

        for(float currentDist = 0; currentDist < totalDist;)
        {
            float step = speed * Time.deltaTime;
            currentDist += step;

            float nextX = Mathf.MoveTowards(isX?player.transform.position.x:player.transform.position.z, x1, step);
            float baseY = Mathf.Lerp(player.transform.position.y, target.y, (nextX - x0) / dist);
            float arc = arcHeight * (nextX - x0) * (nextX - x1) / (-0.25f * dist * dist);
           

            player.transform.position = new Vector3(isX ? nextX: player.transform.position.x, baseY + arc, isX?player.transform.position.z:nextX) ;
            yield return null;

        }

        //error correction
        player.transform.position = new Vector3(target.x, target.y, target.z);
        NotifyEndOfAction();


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
        float currentDegrees = 0f;
        for (; currentDegrees < degrees; currentDegrees += speed * Time.deltaTime)
        {

            float step = sign*(speed * Time.deltaTime);
            player.transform.Rotate(0, step, 0);
            yield return null;
        }

        //error correction
        player.transform.Rotate(0, sign*(degrees- currentDegrees), 0);
        NotifyEndOfAction();


    }

    public void DoMove(GameObject player,List<int>currentBlock, List<int>intendedBlock)
    {
        Vector3 target = new Vector3(player.transform.position.x + (intendedBlock[0] - currentBlock[0]) * manager.MapRenderer.blockLength,
            player.transform.position.y + (intendedBlock[1] - currentBlock[1]) * manager.MapRenderer.blockLength,
            player.transform.position.z + (intendedBlock[2] - currentBlock[2]) * manager.MapRenderer.blockLength);

        StartCoroutine(MoveCoroutine(actionSpeed, player, target, manager.MapRenderer.blockLength));
    }

    private void NotifyEndOfAction()
    {
        manager.Logic.NotifyEndOfAction();
    }

    IEnumerator MoveCoroutine(float speed,GameObject player, Vector3 target,float distance)
    {
        
        for (float currentDistance=0f;currentDistance<distance;currentDistance+= speed * Time.deltaTime)
        {
            
            float step = speed * Time.deltaTime;
            player.transform.position = Vector3.MoveTowards(player.transform.position, target, step);
            yield return null;
        }
        //error correction
        player.transform.position = new Vector3(target.x, target.y, target.z);
        NotifyEndOfAction();


    }

  
}
