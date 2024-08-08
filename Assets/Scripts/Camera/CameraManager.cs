using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private GameManager gameManager;
    public CinemachineFreeLook freeLookCam;
    public CinemachineFreeLook lockedCam;
    public Transform listener;
    private Transform player;
    private Animator playerAnim;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerAnim = player.GetComponent<Animator>();
        gameManager = GameManager.Instance;
    }

    void Update()
    {
        if (gameManager.GetGameOver()||gameManager.GetGamePause())
        {
            freeLookCam.m_YAxis.m_InputAxisValue = 0;
            freeLookCam.m_XAxis.m_InputAxisValue = 0;
            lockedCam.m_YAxis.m_InputAxisValue = 0;
            return;
        }

        float y_input = Input.GetAxis("Mouse Y");
        float x_input = Input.GetAxis("Mouse X");

        freeLookCam.m_YAxis.m_InputAxisValue = y_input;
        freeLookCam.m_XAxis.m_InputAxisValue = x_input;

        lockedCam.m_YAxis.m_InputAxisValue = y_input;

        if (!playerAnim.GetBool("LockedCamera"))
        {
            listener.position = freeLookCam.gameObject.transform.position;
            listener.transform.LookAt(player.position);
        }
        else
        {
            listener.position = lockedCam.gameObject.transform.position;
            listener.transform.LookAt(player.position);
        }


    }
}
