using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts;

public class HealthUi : MonoBehaviour
{
    public Sprite fullHeart;
    public Sprite emptyHeart;
    public Animator hitPanel;
    
    private Image[] hearts;
    private int maxHealth = 5;

    private void Start() {
        hearts = gameObject.GetComponentsInChildren<Image>();
        foreach(Image heart in hearts){
            heart.sprite = fullHeart;
        }
    }

    public void updateHearts(int currentHealth){
        for(int i = 0; i < maxHealth; i++){
            if(i<currentHealth){
                hearts[i].sprite = fullHeart;
            }else{
                hearts[i].sprite = emptyHeart;
            }
        }
        hitPanel.SetTrigger("hit");
    }


}
