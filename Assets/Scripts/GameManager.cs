using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    private static GameManager _instance;   
 
    public static GameManager Instance {
        get {
            if(_instance==null) {
                _instance = new GameManager();
            }
 
            return _instance;
        }
    }
    private void Awake() {
        _instance = this;
    }


    public void restartGame(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;
    }

    public void pause(){
        Time.timeScale = 0;
    }

    public void resume(){
        Time.timeScale = 1;
    }
}
