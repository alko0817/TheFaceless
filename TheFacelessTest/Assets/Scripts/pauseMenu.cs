using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pauseMenu : MonoBehaviour
{
    public GameObject menu;
    public GameObject[] otherUI;
    public TimeManager timeManager;
    bool paused = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!paused)
            {
                foreach (GameObject element in otherUI)
                {
                    element.SetActive(false);
                }

                menu.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                timeManager.stopUpdate = true;
                Time.timeScale = 0f;
                paused = true;
                
            }

            else
            {
                foreach (GameObject element in otherUI)
                {
                    element.SetActive(true);
                }

                menu.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                timeManager.stopUpdate = false;
                
                paused = false;
            }
        }
    }

    public void Continue()
    {
        foreach (GameObject element in otherUI)
        {
            element.SetActive(true);
        }

        menu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        timeManager.stopUpdate = false;
        paused = false;
    }

    public void QuitGame ()
    {
        Application.Quit();
    }
}
