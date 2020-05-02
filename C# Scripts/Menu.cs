using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    
    
    
    public GameObject menuPanel;
    void Start()
    {
        
    }
    
    public void openMenu()
    {
        if (menuPanel != null)
        {
            menuPanel.SetActive(true);
        }
    }

    public void closeMenu()
    {
        if (menuPanel != null)
        {
            menuPanel.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}