using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualsMenu : MonoBehaviour
{
    public void SetQuality (int index)
    {
        QualitySettings.SetQualityLevel(index);
    }

    public void SetFullscreen (bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
}
