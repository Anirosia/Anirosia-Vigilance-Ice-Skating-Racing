using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuBounce : MonoBehaviour
{
    public GameObject menuTitle;
    public GameObject playbtn;
    public GameObject settingsbtn;
    public GameObject shopbtn;

    private float titleStart;
    private float playbtnStart;
    private float settingsbtnStart;
    private float shopbtnStart;

    float movement_1 = 20;
    float movement_2 = 8;

    void Start()
    {
        titleStart = menuTitle.transform.position.y;
        playbtnStart = playbtn.transform.position.y;

        settingsbtnStart = settingsbtn.transform.position.y;
        shopbtnStart = shopbtn.transform.position.y;
    }

    
    void Update()
    { 
       
        menuTitle.transform.position = new Vector3(menuTitle.transform.position.x, titleStart + ((float)Mathf.Sin(Time.time) * movement_1));
        playbtn.transform.position = new Vector3(playbtn.transform.position.x, playbtnStart + ((float)Mathf.Sin(Time.time) * movement_1));

        settingsbtn.transform.position = new Vector3(settingsbtn.transform.position.x, settingsbtnStart + ((float)Mathf.Sin(Time.time) * movement_2));
        shopbtn.transform.position = new Vector3(shopbtn.transform.position.x, shopbtnStart + ((float)Mathf.Sin(Time.time) * movement_2));
    }
}
