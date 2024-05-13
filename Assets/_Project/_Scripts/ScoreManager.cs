using TMPro;
using UnityEngine;
using System;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text linesText;

    [SerializeField] private int score = 0;
    public int GetScore() { return score; }
    [SerializeField] private int lines;
    [SerializeField] private int level = 1;
    public int GetLevel() { return level; }

    [SerializeField] private int linesPerLevel = 5;

    [SerializeField] private int minLines = 1;
    [SerializeField] private int maxLines = 5;

    public event EventHandler levelUp;

    public void ScoreLines(int n)
    {
        n = Mathf.Clamp(n, minLines, maxLines);

        switch (n)
        {
            case 1:
                score += 40 * level;
                break;
            case 2:
                score += 100 * level;
                break;
            case 3:
                score += 300 * level;
                break;
            case 4:
                score += 1200 * level;
                break;
        }

        lines -= n;

        if (lines <= 0) LevelUp();

        UpdateUIText();
    }

    public void ResetScore()
    {
        level = 1;
        score = 0;
        lines = linesPerLevel * level;
        UpdateUIText();
    }

    private void Start()
    {
        ResetScore();
    }

    private void UpdateUIText()
    {
        if (linesText)
        {
            linesText.text = lines.ToString();
        }
        if (scoreText)
        {
            scoreText.text = PadZero(score, 5);
        }
        if (levelText)
        {
            levelText.text = level.ToString();
        }
    }

    private string PadZero(int n, int padDigits)
    {
        string nString = n.ToString();

        while (nString.Length < padDigits)
        {
            nString = "0" + nString;
        }

        return nString;
    }

    private void LevelUp()
    {
        level++;
        lines = linesPerLevel * level;
        levelUp?.Invoke(this, EventArgs.Empty);
    }
}
