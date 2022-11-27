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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            FindObjectOfType<HealthBarController>().TakeDamage(20);
        }
    }

}
