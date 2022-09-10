using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject pausePanel;
    public GameObject loadingScreen;
    public Slider loadingSlider;
    private bool paused = false;
    private int sceneId = 1;


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
        loadingSlider.value = 0;
    }


    public void LoadScene(int s)
    {
        StartCoroutine(LoadAsync(s));
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
        LoadScene(SceneManager.GetActiveScene().buildIndex);

        Time.timeScale = 1;
        paused = false;
    }

    public void startGame()
    {   
        LoadScene(sceneId);
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

  
    IEnumerator LoadAsync(int s)
    {   
        AsyncOperation operation = SceneManager.LoadSceneAsync(s);
        loadingScreen.SetActive(true);
        while (!operation.isDone)
        {   
            loadingSlider.value  = Mathf.Clamp01(operation.progress / .9f);
       
            yield return null;
        }
    }
}
