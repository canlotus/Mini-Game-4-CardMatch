using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public Sprite frontSprite;
    public Sprite backSprite;
    private Image cardImage;
    private bool isFlipped = false;

    private void Awake()
    {
        cardImage = GetComponent<Image>();
        cardImage.sprite = backSprite; 
    }

    public void SetFaceUp()
    {
        isFlipped = true;
        cardImage.sprite = frontSprite;
    }

    public void FlipCard()
    {
        if (isFlipped) return;

        isFlipped = true;
        cardImage.sprite = frontSprite; 

        CardManager.instance?.CheckMatch(this);
    }

    public void ResetCard()
    {
        isFlipped = false;
        cardImage.sprite = backSprite; 
    }
}