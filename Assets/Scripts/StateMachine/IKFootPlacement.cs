using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKFootPlacement : MonoBehaviour
{
    private Animator anim;
    private Rigidbody rb;
    private CapsuleCollider capsuleCol;

    public GameObject backWeapon;
    public GameObject handWeapon_l;
    public GameObject handWeapon_r;

    void Start()
    {
        anim = this.GetComponent<Animator>();
        capsuleCol = this.GetComponent<CapsuleCollider>();
        rb = GetComponentInParent<Rigidbody>();
        //handWeapon.SetActive(false); // espada da mao comeca desativada
    }

    #region Unity Anim Event

    public void TakeWeapon()
    {
        if (!anim.GetBool("Equipped"))
        {
            handWeapon_l.SetActive(true);
            handWeapon_r.SetActive(true);
            backWeapon.SetActive(false);
            anim.SetBool("Equipped", true);
        }
        else
        {
            handWeapon_l.SetActive(false);
            handWeapon_r.SetActive(false);
            backWeapon.SetActive(true);
            anim.SetBool("Equipped", false);
        }
    }

    public void TakeWeaponEnemy()
    {
        if (!anim.GetBool("Equipped"))
        {
            anim.SetBool("Equipped", true);
        }
        else
        {
            anim.SetBool("Equipped", false);
        }
    }

    public void SetAttackingTrue()
    {
        anim.SetBool("Attacking", true);
    }

    public void SetAttackingFalse()
    {
        anim.SetBool("Attacking", false);
    }

    public void SetCanAttackTrue()
    {
        anim.SetBool("CanAttack", true);
    }

    public void SetIntangibleOn()
    {
        //capsuleCol.isTrigger = true;
        //rb.isKinematic = true;
        anim.SetBool("Intangible", true);
    }

    public void RestoreRigidbodyAndCollider()
    {
        capsuleCol.isTrigger = false;
        rb.isKinematic = false;
        //anim.SetBool("CanMove", true);
        anim.SetBool("Intangible", false);
    }
    #endregion
}
