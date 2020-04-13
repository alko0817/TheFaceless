using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class levelLoader : MonoBehaviour
{
    public GameObject loadingScreen;
    public TextMeshProUGUI textProgress;
    public Image loaderFill;

    public void LoadLevel (int levelIndex)
    {
        loadingScreen.SetActive(true);
        StartCoroutine(Loading(levelIndex));
        
    }

    IEnumerator Loading (int levelIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(levelIndex);
        

        while (!operation.isDone)
        {
            //show operation.progress
            float progress = Mathf.Clamp01(operation.progress / .9f);
            Debug.Log(progress);
            loaderFill.fillAmount = progress;
            textProgress.text = "Loading " + progress * 100 + "%";

            yield return null;
        }
    }
}
