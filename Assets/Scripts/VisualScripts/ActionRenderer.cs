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
        StartCoroutine(MoveCoroutine(1f, manager.MapRenderer.MainCharacter.transform.position, new Vector3(transform.position.x + 1f, transform.position.y, transform.position.z)));
    }

    IEnumerator MoveCoroutine(float speed,Vector3 position, Vector3 target)
    {
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(position, target, step);
        yield return null;
    }
}
