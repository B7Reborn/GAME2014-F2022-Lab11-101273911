using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject onScreenControls;


    void Awake()
    {
        onScreenControls = GameObject.Find("OnScreenControls");

        onScreenControls.SetActive(Application.isMobilePlatform);

        
    }

    void Start()
    {
        FindObjectOfType<SoundManager>().PlayMusic(Sound.MAIN_MUSIC);
    }

}
