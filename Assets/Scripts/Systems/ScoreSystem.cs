using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class HighScore
{
    public string playerName;
    public int score;
    public string date;
    public int seed;
}

[System.Serializable]
public class HighScoreList
{
    public List<HighScore> highScores = new List<HighScore>();
}

public class ScoreSystem : MonoBehaviour
{
    public int currentScore = 0;
    private List<HighScore> highScores;
    private string highScorePath;
    private const int MAX_HIGH_SCORES = 10;
    
    private void Awake()
    {
        highScorePath = Path.Combine(Application.persistentDataPath, "highscores.json");
        LoadHighScores();
    }
    
    public void AddScore(int points)
    {
        currentScore += points;
    }
    
    public void ResetScore()
    {
        currentScore = 0;
    }
    
    public void SaveHighScore(string playerName)
    {
        if (highScores == null)
        {
            highScores = new List<HighScore>();
        }
        
        HighScore newScore = new HighScore
        {
            playerName = playerName,
            score = currentScore,
            date = DateTime.Now.ToString("yyyy-MM-dd"),
            seed = GameManager2D.Instance != null ? GameManager2D.Instance.currentSeed : 0
        };
        
        highScores.Add(newScore);
        
        // Sorba rendezés pontszám szerint, csak a legjobb 10 pontszámot tartjuk meg
        highScores = highScores.OrderByDescending(s => s.score).Take(MAX_HIGH_SCORES).ToList();
        
        // Mentés fájlba
        try
        {
            HighScoreList scoreList = new HighScoreList { highScores = highScores };
            string json = JsonUtility.ToJson(scoreList, true);
            File.WriteAllText(highScorePath, json);
            Debug.Log($"Highscore mentve: {playerName} - {currentScore}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Hiba a highscore mentésekor: {e.Message}");
        }
    }
    
    public List<HighScore> GetHighScores()
    {
        // Mindig újratöltjük a fájlból, hogy biztosan aktuális legyen
        LoadHighScores();
        return highScores ?? new List<HighScore>();
    }
    
    private void LoadHighScores()
    {
        try
        {
            if (File.Exists(highScorePath))
            {
                string json = File.ReadAllText(highScorePath);
                HighScoreList scoreList = JsonUtility.FromJson<HighScoreList>(json);
                highScores = scoreList?.highScores ?? new List<HighScore>();
                Debug.Log($"[ScoreSystem] Highscore-ok betöltve: {highScores.Count} db, fájl: {highScorePath}");
            }
            else
            {
                highScores = new List<HighScore>();
                Debug.Log($"[ScoreSystem] Highscore fájl nem létezik: {highScorePath}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Hiba a highscore betöltésekor: {e.Message}");
            highScores = new List<HighScore>();
        }
    }
}
