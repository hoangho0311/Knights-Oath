using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

//Execute first
[DefaultExecutionOrder(0)]
public class GameManager : MonoBehaviour
{
    [DllImport("user32.dll")]
    static extern bool SetCursorPos(int X, int Y);
    public static GameManager Instance;
    public UIManager uIManager;
    [Header("Controll")]
    public Animator playerAnim;

    #region Encapsulation
    private bool isGameOVer;
    private bool isGameStarted;
    private bool isPaused;

    public bool GetGameOver()
    {
        return this.isGameOVer;
    }

    public bool GetGameStart()
    {
        return this.isGameStarted;
    }

    public bool GetGamePause()
    {
        return this.isPaused;
    }

    public void SetGameOver(bool status)
    {
        this.isGameOVer = status;
    }

    public void SetGameStart(bool status)
    {
        this.isGameStarted = status;
    }

    public void SetGamePause(bool status)
    {
        this.isPaused = status;
    }
    #endregion

    private void Awake()
    {
        HideCursor(true);
        Instance = this; 
    }

    private void Start()
    {
        uIManager = UIManager.instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseManager();
        }
    }

    public void HideCursor(bool value)
    {
        if (value) 
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else 
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            int xPos = Screen.width / 2, yPos = Screen.height / 3;
            SetCursorPos(xPos, yPos);
        }
    }

    private void PauseManager()
    {
        if (!uIManager.GetPauseScreen().activeSelf) // Ativa a tela de pause
        {
            uIManager.ShowPauseScreen();
        }

        Time.timeScale = 0;
    }
}
