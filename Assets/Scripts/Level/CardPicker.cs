using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardPicker : MonoBehaviour
{

    public int maxCard;
    public Material textureCard;
    private float yLengthTexture;
    public float yLengthCard=380;
    private int currentCard=0;



    public float distance = 1f;
    public float speed = 1f;
    private int dir;
    private float currentDistance;

    private bool playedUp = false;
    private bool onAnim = false;
   

    bool LiftUp()
    {
        if (currentDistance >= distance)
        {
          
            dir = dir * -1;
            currentDistance = 0;
            return true;
        }
        currentDistance += Time.deltaTime * speed;
        transform.position += Vector3.up * Time.deltaTime * speed * dir;
        return false;
    }
   



    // Start is called before the first frame update
    void Start()
    {
        dir = 1;
        currentDistance = 0;
        yLengthTexture = textureCard.mainTexture.height;
        textureCard.mainTextureOffset = new Vector2(0, -(yLengthCard*currentCard / yLengthTexture));
    }

  

    // Update is called once per frame
    void Update()
    {
        if (onAnim)
        {
            if (!playedUp)
            {
                
                if (LiftUp())
                {
                    setCard(currentCard + 1);
                    playedUp = true;
                }
            }
            else
            {
                if (LiftUp())
                {
                    playedUp = false;
                    onAnim = false;
                }
            }
            
           
        }
        

    }

    public int setCard(int number)
    {
        int numberAux = number;
        if (number < 0)
        {
            numberAux = 0;
        }

        if (number > maxCard)
        {
            numberAux = 0;
        }

        currentCard = numberAux;
        textureCard.mainTextureOffset = new Vector2(0, -(yLengthCard * currentCard / yLengthTexture));
        return numberAux;
    }

    void OnMouseDown()
    {
  
        if (!onAnim)
        {
            playedUp = false;
            onAnim = true;
        }
    }
}
