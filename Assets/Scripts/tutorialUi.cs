using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tutorialUi : MonoBehaviour
{
    public GameObject tutorialPanel;
    public Animator tutorialAnimator;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(tutorialAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1){
            tutorialPanel.SetActive(false);
        }
    }
}
