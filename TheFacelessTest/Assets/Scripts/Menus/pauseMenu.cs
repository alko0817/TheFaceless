using UnityEngine;

public class pauseMenu : MonoBehaviour
{
    public GameObject menu;
    public GameObject[] otherUI;
    TimeManager timeManager;
    bool paused = false;


    private void Start()
    {
        timeManager = GameObject.FindGameObjectWithTag("Time Manager").GetComponent<TimeManager>();
    }
    private void Update()
    {
        if (timeManager == null)
        {
            Debug.LogWarning("There is no time manager in the scene! The game will break without it!");
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!paused)
            {
                foreach (GameObject element in otherUI)
                {
                    if (element == null) continue;
                    element.SetActive(false);
                }

                menu.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                timeManager.stopUpdate = true;
                Time.timeScale = 0f;
                paused = true;
                
            }

            else
            {
                foreach (GameObject element in otherUI)
                {
                    if (element == null) continue;
                    element.SetActive(true);
                }

                menu.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
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
        Cursor.visible = false;
        timeManager.stopUpdate = false;
        paused = false;
    }

    public void QuitGame ()
    {
        Application.Quit();
    }
}
