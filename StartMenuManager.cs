using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StartMenuManager : MonoBehaviour
{
    public GameObject startPanel;
    public GameObject gameOverPanel;
    public Slider difficultySlider;
    public TMP_Text difficultyText;

    private string[] difficultyLevels = { "Easy", "Medium", "Hard" };

    private void Start()
    {
        int savedDifficulty = PlayerPrefs.GetInt("CardSlider", 1);
        difficultySlider.value = savedDifficulty;
        UpdateDifficultyText();
        difficultySlider.onValueChanged.AddListener(delegate { UpdateDifficultyText(); });

        gameOverPanel.SetActive(false); 
    }

    public void UpdateDifficultyText()
    {
        int difficultyIndex = Mathf.RoundToInt(difficultySlider.value);
        difficultyText.text = difficultyLevels[difficultyIndex];
    }

    public void StartGame()
    {
        int difficultyIndex = Mathf.RoundToInt(difficultySlider.value);
        PlayerPrefs.SetInt("CardSlider", difficultyIndex);
        PlayerPrefs.Save();

        string difficultyName = difficultyLevels[difficultyIndex];
        FindObjectOfType<CardManager>().SetDifficulty(difficultyName);

        startPanel.SetActive(false);
        gameOverPanel.SetActive(false); 
    }

    public void ReturnToMenu()
    {
        gameOverPanel.SetActive(false);
        startPanel.SetActive(true);
    }

    public void RestartGame()
    {
        StartGame(); 
    }
}