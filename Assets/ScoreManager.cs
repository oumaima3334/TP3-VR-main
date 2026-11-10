using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private TextMeshPro scoreText;

    private static ScoreManager instance;
    private int score = 0;

    void Awake()
    {
        instance = this;
        UpdateText();
    }

    
    public static void AddPoint()
    {
        if (instance == null) return;
        instance.score++;
        instance.UpdateText();
    }

    private void UpdateText()
    {
        if (scoreText != null)
            scoreText.text = "Score : " + score;
    }
}