using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    

    void Start()
    {
        
    }

    
    void Update()
    {
        
    }

    public void switchPanel(GameObject panel) // Sets all panels to false before activating the passed in panel.
    {
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            gameObject.transform.GetChild(i).gameObject.SetActive(false);
        }

        panel.SetActive(true);
    }
}
