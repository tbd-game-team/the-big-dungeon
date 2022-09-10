using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject pausePanel;
    public GameObject loadingScreen;
    public Slider loadingSlider;
    private bool paused = false;
    private int sceneId = 1;

    public AudioMixerSnapshot snapshotPaused;
    public AudioMixerSnapshot snapshotUnpaused;


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

    
    /// <summary>
    /// @author: Neele Kemper
    /// Load a scene, defined by passed scene id
    /// </summary>
    /// <param name="s">the scenen id</param>
    /// <returns></returns>
    public void LoadScene(int s)
    {
        StartCoroutine(LoadAsync(s));
    }

    
    /// <summary>
    /// @author: Neele Kemper
    /// Load a scene asynchon and display the loading screen
    /// </summary>
    /// <param name="s">the scenen id</param>
    /// <returns></returns>
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {   
            if(!isPaused)
            {
                pause();
                pausePanel.SetActive(true);
            }
        }
    }


    public void restartGame()
    {
        LoadScene(SceneManager.GetActiveScene().buildIndex);

        Time.timeScale = 1;
        paused = false;
        Lowpass();
    }

    public void startGame()
    {   
        SceneManager.LoadScene(sceneId);
        Time.timeScale = 1;
        paused = false;
        Lowpass();
    }

    private void Lowpass()
    {
        if(Time.timeScale == 0)
        {
            Debug.Log("Snapchot Paused");
            snapshotPaused.TransitionTo(.01f);
        } 
        else
        {
            Debug.Log("Snapchot Unpaused");
            snapshotUnpaused.TransitionTo(.01f);
        }
    }

    public void pause()
    {
        paused = true;
        Time.timeScale =  0;
        Lowpass();
    }

    public void resume()
    {
        paused = false;
        Time.timeScale = 1;
        Lowpass();
    }

    public bool isPaused {
        get {
            return paused;
        }
    }

}
