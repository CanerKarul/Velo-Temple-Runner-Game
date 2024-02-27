using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SceneMan : MonoBehaviour
{
    public TMP_Text bestDistanceScoreText;
    public TMP_Text maxCoinsScoreText;
    
    // Start is called before the first frame update
    void Start()
    {
        bestDistanceScoreText.text = "Best Running Distance Score: " + PlayerPrefs.GetInt("highscoreD") + "M";
        maxCoinsScoreText.text = "Max. Coins Collected: " + PlayerPrefs.GetInt("highscoreC");
    }

    public void ToGame()
    {
        SceneManager.LoadScene("VeloTempleRunner");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
