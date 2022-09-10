using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject pausePanel;
    private bool paused = false;


    private static GameManager _instance;

    public static GameManager Instance {
        get {
            if (_instance == null)
            {
                _instance = new GameManager();
            }

            return _instance;
        }
    }
    private void Awake()
    {
        _instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pause();
            pausePanel.SetActive(true);
        }
    }

    public void restartGame()
    {   
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;
        paused = false;
    }

    public void startGame()
    {
        SceneManager.LoadScene(1);
        Time.timeScale = 1;
        paused = false;
    }

    public void pause()
    {
        paused = true;
        Time.timeScale = 0;
    }

    public void resume()
    {
        paused = false;
        Time.timeScale = 1;
    }

    public bool isPaused {
        get {
            return paused;
        }
    }
}
