using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    private GameManager gameManager;


    [Header("Health Player")]
    public Slider lifeBar;
    public Slider lifeGhost;

    [Header("Canvas")]
    public GameObject LoseScreen;
    public GameObject pauseScreen;
    public Volume volume; // Reference to the Volume component
    private ColorAdjustments colorAdjustments;
    public CanvasGroup transitionFade;

    
    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(TransitionFadeOut());
        gameManager = GameManager.Instance;
        volume.profile.TryGet<ColorAdjustments>(out colorAdjustments);
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.GetGameOver())
        {
            colorAdjustments.saturation.value = Mathf.Lerp(colorAdjustments.saturation.value, -100, 1 * Time.deltaTime);
            LoseScreen.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(LoseScreen.GetComponent<CanvasGroup>().alpha, 1, 0.5f * Time.deltaTime);
        }
    }

    public void ShowPauseScreen()
    {
        pauseScreen.SetActive(true);
    }
    
    public GameObject GetPauseScreen()
    {
        return pauseScreen;
    }

    IEnumerator TransitionFadeOut()
    {
        transitionFade.gameObject.SetActive(true);
        transitionFade.alpha = 1;
        while (transitionFade.alpha > 0)
        {
            transitionFade.alpha -= 0.05f;
            yield return new WaitForSeconds(0.1f);
        }
        transitionFade.gameObject.SetActive(false);
    }

    IEnumerator TransitionFadeIn(string n)
    {
        transitionFade.gameObject.SetActive(true);
        transitionFade.alpha = 0;
        while (transitionFade.alpha < 1)
        {
            transitionFade.alpha += 0.05f;
            yield return new WaitForSeconds(0.1f);
        }
        SceneManager.LoadScene(n);
    }

    public void Restart()
    {
        StartCoroutine(TransitionFadeIn("GamePlay"));
    }

    public void ReturnMainMenu()
    {
        Time.timeScale = 1;
        StartCoroutine(TransitionFadeIn("Main_Menu"));
    }

    #region Update Health Player
    public void UpdateLifeBar(float life)
    {
        // Chuyển đổi giá trị life thành phần trăm
        float lifePercentage = (life / 10) * 200;

        // Gán giá trị phần trăm cho slider
        lifeBar.value = lifePercentage;
    }
    
    public void UpdateLifeGhost(float ghost)
    {
        // Chuyển đổi giá trị ghost thành phần trăm
        float ghostPercentage = (ghost / 10) * 200;

        // Gán giá trị phần trăm cho slider
        lifeGhost.value = ghostPercentage;
    }
    #endregion
}
