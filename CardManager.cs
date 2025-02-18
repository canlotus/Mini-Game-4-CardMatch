using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardManager : MonoBehaviour
{
    public static CardManager instance;

    public GameObject cardPrefab;
    public Transform cardContainer;
    public GridLayoutGroup gridLayout;
    public Sprite[] cardSprites;
    public GameObject matchEffectPrefab;

    public Image[] lifeImages;
    public Sprite defaultLifeSprite;
    public Sprite lostLifeSprite;
    public GameObject gameOverPanel;
    public TMP_Text gameOverText; 

    public TMP_Text countdownText;
    public Transform countdownTransform;

    private Card firstSelectedCard;
    private Card secondSelectedCard;
    private int wrongAttempts = 0;
    private int maxWrongAttempts = 3;
    private int matchedPairs = 0;
    private int totalPairs;
    private bool isCheckingMatch = false;
    private List<Sprite> shuffledCards = new List<Sprite>();
    private Card[] allCards;

    private void Awake()
    {
        instance = this;
    }

    public void SetDifficulty(string difficulty)
    {
        int rows = 3, columns = 4;

        switch (difficulty)
        {
            case "Easy":
                rows = 3;
                columns = 4;
                break;
            case "Medium":
                rows = 4;
                columns = 5;
                break;
            case "Hard":
                rows = 6;
                columns = 5;
                break;
        }

        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = columns;

        foreach (Transform child in cardContainer)
        {
            Destroy(child.gameObject);
        }

        ResetGame();
        GenerateCards(rows, columns);
        StartCoroutine(StartCountdown());
    }

    void GenerateCards(int rows, int columns)
    {
        int totalCards = rows * columns;
        totalPairs = totalCards / 2;
        matchedPairs = 0;
        shuffledCards.Clear();

        for (int i = 0; i < totalPairs; i++)
        {
            shuffledCards.Add(cardSprites[i]);
            shuffledCards.Add(cardSprites[i]);
        }

        ShuffleList(shuffledCards);

        allCards = new Card[totalCards];

        for (int i = 0; i < totalCards; i++)
        {
            GameObject newCard = Instantiate(cardPrefab, cardContainer);
            Card cardScript = newCard.GetComponent<Card>();
            cardScript.frontSprite = shuffledCards[i];
            cardScript.SetFaceUp();
            allCards[i] = cardScript;
        }

        StartCoroutine(HideAllCardsAfterDelay());
    }

    IEnumerator HideAllCardsAfterDelay()
    {
        yield return new WaitForSeconds(5f);
        foreach (Card card in allCards)
        {
            card.ResetCard();
        }
        isCheckingMatch = false;
    }

    private void ShuffleList(List<Sprite> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);
            Sprite temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    public void CheckMatch(Card selectedCard)
    {
        if (isCheckingMatch || selectedCard == firstSelectedCard) return;

        if (firstSelectedCard == null)
        {
            firstSelectedCard = selectedCard;
        }
        else
        {
            secondSelectedCard = selectedCard;
            isCheckingMatch = true;
            DisableAllCards();
            StartCoroutine(CheckMatchCoroutine());
        }
    }

    private IEnumerator CheckMatchCoroutine()
    {
        yield return new WaitForSeconds(1f);

        if (firstSelectedCard.frontSprite == secondSelectedCard.frontSprite)
        {
            firstSelectedCard.SetFaceUp();
            secondSelectedCard.SetFaceUp();

            SpawnMatchEffect(firstSelectedCard.transform.position);
            SpawnMatchEffect(secondSelectedCard.transform.position);

            matchedPairs++;

            if (matchedPairs >= totalPairs)
            {
                WinGame();
                yield break;
            }
        }
        else
        {
            firstSelectedCard.ResetCard();
            secondSelectedCard.ResetCard();
            wrongAttempts++;
            UpdateLifeUI();

            if (wrongAttempts >= maxWrongAttempts)
            {
                GameOver();
                yield break;
            }
        }

        firstSelectedCard = null;
        secondSelectedCard = null;
        yield return new WaitForSeconds(0.5f);
        isCheckingMatch = false;
        EnableAllCards();
    }

    private void SpawnMatchEffect(Vector3 position)
    {
        GameObject effect = Instantiate(matchEffectPrefab, position, Quaternion.identity, cardContainer);
        Destroy(effect, 1.5f);
    }

    private void UpdateLifeUI()
    {
        if (wrongAttempts > 0 && wrongAttempts <= maxWrongAttempts)
        {
            int index = maxWrongAttempts - wrongAttempts;
            lifeImages[index].sprite = lostLifeSprite;
        }
    }

    private void ResetGame()
    {
        wrongAttempts = 0;
        matchedPairs = 0;
        ResetLives();
        gameOverPanel.SetActive(false);
        isCheckingMatch = true;
    }

    private void ResetLives()
    {
        for (int i = 0; i < lifeImages.Length; i++)
        {
            lifeImages[i].sprite = defaultLifeSprite;
        }
    }

    private void GameOver()
    {
        gameOverText.text = "GAME OVER";
        gameOverPanel.SetActive(true);
    }

    private void WinGame()
    {
        gameOverText.text = "WIN";
        gameOverPanel.SetActive(true);
    }

    IEnumerator StartCountdown()
    {
        countdownText.gameObject.SetActive(true);
        isCheckingMatch = true;

        for (int i = 5; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return StartCoroutine(ScaleCountdown());
            yield return new WaitForSeconds(0.5f);
        }

        countdownText.gameObject.SetActive(false);
        isCheckingMatch = false;
    }

    IEnumerator ScaleCountdown()
    {
        float duration = 0.2f;
        Vector3 originalScale = countdownTransform.localScale;
        Vector3 targetScale = originalScale * 1.5f;

        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            countdownTransform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        elapsedTime = 0;
        while (elapsedTime < duration)
        {
            countdownTransform.localScale = Vector3.Lerp(targetScale, originalScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    private void DisableAllCards()
    {
        foreach (Card card in allCards)
        {
            card.GetComponent<Button>().interactable = false;
        }
    }

    private void EnableAllCards()
    {
        foreach (Card card in allCards)
        {
            card.GetComponent<Button>().interactable = true;
        }
    }
}