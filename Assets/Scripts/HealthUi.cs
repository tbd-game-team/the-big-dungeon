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
    public int maxHealthPlayer = 5;

    private void Start() {
        hearts = gameObject.GetComponentsInChildren<Image>();
        foreach(Image heart in hearts){
            heart.sprite = fullHeart;
        }
    }

    public void updateHearts(int currentHealth, bool damageTaken){
        for(int i = 0; i < maxHealthPlayer; i++){
            if(i<currentHealth){
                hearts[i].sprite = fullHeart;
            }else{
                hearts[i].sprite = emptyHeart;
            }
        }
        if(damageTaken){
            hitPanel.SetTrigger("hit");
        }
    }


}
