using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    public Image loadImage;
    public int levelToLoad;
    public CanvasGroup transitionFade;
    private float maxWidth = 1200;
    private float height = 25;
    private AsyncOperation operation;

    private void Start()
    {
        loadImage.rectTransform.sizeDelta = new Vector2(0f, height);
        LoadLevel(levelToLoad);
    }

    public void LoadLevel(int sceneIndex)
    {
        StartCoroutine(LoadAsynchronously(sceneIndex));
    }

    IEnumerator LoadAsynchronously (int sceneIndex)
    {
        operation = SceneManager.LoadSceneAsync(sceneIndex);

        operation.allowSceneActivation = false;
        
        while (operation.progress < .9f)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            loadImage.rectTransform.sizeDelta = new Vector2(progress * maxWidth, height);
            yield return null;
        }
        loadImage.rectTransform.sizeDelta = new Vector2(maxWidth, height);
        StartCoroutine(TransitionFade());
    }

    IEnumerator TransitionFade()
    {
        while (transitionFade.alpha < 1)
        {
            transitionFade.alpha += 0.05f;
            yield return new WaitForSeconds(0.1f);
        }
        operation.allowSceneActivation = true;
    }

}
