using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour
{
    public GameManager gameManager;
    public UIManager uiManager;
    public TMP_Dropdown qualityDropdown;
    public TMP_Dropdown fullscreenDropdown;
    public GameObject FPSText;
    // Start is called before the first frame update
    void Start()
    {
        // Populate the quality dropdown with the quality levels
        qualityDropdown.ClearOptions();
        qualityDropdown.AddOptions(new List<string>(QualitySettings.names));

        // Set the quality dropdown to the current quality level
        qualityDropdown.value = QualitySettings.GetQualityLevel();
        qualityDropdown.RefreshShownValue();

        // Set the fullscreen dropdown to the current fullscreen state
        fullscreenDropdown.value = Screen.fullScreen ? 1 : 0;
        fullscreenDropdown.RefreshShownValue();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        gameManager = GameManager.Instance;
        uiManager = UIManager.instance;
        gameManager.SetGamePause(true);
        gameManager.HideCursor(false);
    }

    public void ExitBtn()
    {
        gameManager.SetGamePause(false);
        gameManager.HideCursor(true);
        gameObject.SetActive(false);
        Time.timeScale = 1;
    }

    public void ChangeQualityLevel(int index)
    {
        QualitySettings.SetQualityLevel(index);
    }

    public void ToggleFullScreen(int index)
    {
        Debug.Log("Fullscreen dropdown value before change: " + fullscreenDropdown.value);
        Screen.fullScreen = index == 0;
        Debug.Log("Fullscreen state after change: " + Screen.fullScreen);
    }
    public void ShowFPS(bool i)
    {
        FPSText.SetActive(i);
    }

}
