using TMPro;
using UnityEngine;
using static CardConstants;

public class CardPicker : MonoBehaviour
{
    //Rehacer esta clase con animaciones y esas cosas
    [SerializeField] private TextMeshPro textMesh;

    private Cards selectedCard = Cards.NoCard;

    private bool isLocked = false;

    private void OnMouseDown()
    {
        if (!isLocked)
        {
            if (Cards.IsDefined(typeof(Cards), selectedCard + 1))
            {
                selectedCard++;
            }
            else
            {
                selectedCard = Cards.NoCard;
            }

            textMesh.text = selectedCard.ToString();
            InformOfCardChanged();
        }
    }

    private void Start()
    {
        textMesh.text = selectedCard.ToString();
    }

    private void InformOfCardChanged()
    {
        LevelManager.instance.RoadLogic.InformOfCardChanged(GetParentRoad(), selectedCard);
    }

    private Road GetParentRoad()
    {
        return transform.parent.GetComponent<Road>();
    }

    public void Lock()
    {
        isLocked = true;
    }

    public void Unlock()
    {
        isLocked = false;
    }
}