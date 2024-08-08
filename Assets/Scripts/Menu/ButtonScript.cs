using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private EventSystem eventSystem;
    private GameObject gameManager;
    private Animator anim;

    [Header("Transition")]
    public GameObject screenBeforeTransition;
    public GameObject screenAfterTransition;


    private void Start()
    {
        eventSystem = GameObject.Find("EventSystem").GetComponent<UnityEngine.EventSystems.EventSystem>();
        gameManager = GameObject.FindGameObjectWithTag("GameManager");
        anim = this.GetComponent<Animator>();
    }

    public void MyToggleMethod()
    {
        if (!Application.isPlaying) return;
        if(gameManager == null) gameManager = GameObject.FindGameObjectWithTag("GameManager");
       // gameManager.GetComponent<GameManagerScript>().CheckForChanges(); // aplica as mudancas
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        eventSystem.SetSelectedGameObject(null);
        eventSystem.SetSelectedGameObject(this.gameObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        eventSystem.SetSelectedGameObject(null);
        anim.SetTrigger("Normal");
    }

    public void OnClickTransition()
    {
        StartCoroutine(DoTransition());
    }

    IEnumerator DoTransition()
    {
        while (screenBeforeTransition.GetComponent<CanvasGroup>().alpha > 0)
        {
            screenBeforeTransition.GetComponent<CanvasGroup>().alpha -= 0.05f;
            yield return new WaitForSeconds(0.075f);
        }
        screenAfterTransition.SetActive(true);
        screenAfterTransition.GetComponent<CanvasGroup>().alpha = 1f;
        screenBeforeTransition.SetActive(false);
    }
}
