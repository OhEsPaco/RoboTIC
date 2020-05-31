using System.Collections;
using UnityEngine;
using static Block;

public class ConditionCardPicker : MonoBehaviour
{
    private ConditionCard[] cards;
    private bool hidedCards = true;
    private ConditionCard selectedCard;

    private void Start()
    {
        cards = GetComponentsInChildren<ConditionCard>(true);
        selectedCard = cards[0];
        foreach (ConditionCard card in cards)
        {
            card.gameObject.SetActive(true);
            card.InformOnTap = TappedCard;
            card.gameObject.SetActive(false);
        }
        selectedCard.gameObject.SetActive(true);
    }

    private bool isBusy = false;
    private bool locked = false;

    public void Lock()
    {
        locked = true;
    }

    public void Unlock()
    {
        locked = false;
    }

    public BlockProperties GetCardProperty()
    {
        return selectedCard.Condition;
    }

    private void TappedCard(ConditionCard card)
    {
        if (!isBusy && !locked)
        {
            if (hidedCards)
            {
                hidedCards = false;
                StartCoroutine(ShowCards());
            }
            else
            {
                hidedCards = true;
                selectedCard = card;
                StartCoroutine(HideCards(card));
            }
        }
    }

    private IEnumerator ShowCards()
    {
        isBusy = true;
        foreach (ConditionCard card in cards)
        {
            card.gameObject.SetActive(true);
        }
        yield return new WaitUntil(() => AllActionsFinished());
        foreach (ConditionCard card in cards)
        {
            card.ShowCard();
        }
        yield return new WaitUntil(() => AllActionsFinished());
        isBusy = false;
    }

    private IEnumerator HideCards(ConditionCard cardToShow)
    {
        isBusy = true;
        yield return new WaitUntil(() => AllActionsFinished());
        foreach (ConditionCard card in cards)
        {
            card.gameObject.SetActive(true);
            card.HideCard();
        }
        yield return new WaitUntil(() => AllActionsFinished());
        foreach (ConditionCard card in cards)
        {
            if (card != cardToShow)
            {
                card.gameObject.SetActive(false);
            }
        }
        isBusy = false;
    }

    private bool AllActionsFinished()
    {
        foreach (ConditionCard card in cards)
        {
            if (!card.ActionsFinished)
            {
                return false;
            }
        }
        return true;
    }
}