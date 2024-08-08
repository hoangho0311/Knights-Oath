using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashCollider : MonoBehaviour
{
    private void OnParticleCollision(GameObject other)
    {
        if (other.gameObject.name.Contains("Player") && !other.GetComponentInChildren<Animator>().GetBool("Intangible"))
        {
            other.GetComponentInChildren<Player_Controller>().TakeDamage(3);
            return;
        }
    }
}
