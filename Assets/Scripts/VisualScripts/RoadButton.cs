using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadButton : MonoBehaviour
{
    public int buttonIndex = 0;
    public GameObject mesh;
    private LevelManager manager;
    private Animation anim;

    // Start is called before the first frame update
    void Awake()
    {
        manager = LevelManager.instance;
        if (mesh != null)
        {
            anim = mesh.GetComponent<Animation>();
        }
    }
    void OnMouseDown()
    {
        
            manager.Logic.AddInputFromButton(buttonIndex);
        if(mesh != null)
        {
            if (anim.isPlaying)
            {
                return;
            }
            else
            {
                anim.Play("ButtonPressed");
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
