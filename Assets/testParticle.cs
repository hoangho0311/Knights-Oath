using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testParticle : MonoBehaviour
{

    private void Start()
    {
    }

    private void OnParticleCollision(GameObject other)
    {
        //if (other.gameObject.name.Contains("Shock") && !anim.GetBool("Intangible"))
        //{
        //    TakeDamage(4);
        //    return;
        //}

        Debug.Log(other.transform.name.Contains("Pivot"));

        //if (other.transform.name.Contains("Flame") && !anim.GetBool("Intangible"))
        //{
        //    TakeDamage(4.2f);
        //    return;
        //}

        if (other.gameObject.name.Contains("Pivot"))
        {
            Debug.Log("Detected collision with Player");

            Player_Controller playerController = GetComponentInChildren<Player_Controller>();

            if (playerController != null)
            {
                Debug.Log("Player_Controller found, applying damage");
                playerController.TakeDamage(3);
                return;
            }
            else
            {
                Debug.LogWarning("Player_Controller not found on " + other.gameObject.name);
            }
        }
        else
        {
            Debug.Log("Collision detected with non-Player object: " + other.gameObject.name);
        }



    }
}
