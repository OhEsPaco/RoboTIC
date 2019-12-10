using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionRenderer : MonoBehaviour
{

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

    public void DoMove()
    {
       
        Vector3 target = new Vector3(manager.MapRenderer.MainCharacter.transform.position.x, manager.MapRenderer.MainCharacter.transform.position.y, manager.MapRenderer.MainCharacter.transform.position.z+1f);
        StartCoroutine(MoveCoroutine(1f, manager.MapRenderer.MainCharacter, target,1f));
    }

    private void NotifyEndOfAction()
    {
        manager.Logic.NotifyEndOfAction();
    }
    IEnumerator MoveCoroutine(float speed,GameObject position, Vector3 target,float d)
    {
        
        for(float distance=0f;distance<=d;distance+= speed * Time.deltaTime)
        {
            
            float step = speed * Time.deltaTime;
            position.transform.position = Vector3.MoveTowards(position.transform.position, target, step);
            //manager.MapRenderer.MainCharacter.transform.position = Vector3.Lerp(position, target, step);
            yield return null;
        }
        NotifyEndOfAction();


    }
}
