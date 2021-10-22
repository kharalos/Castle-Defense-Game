using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopController : MonoBehaviour
{
    Animator anim;
    GameManager gm;
    bool shopIsOpen;
    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
        gm = FindObjectOfType<GameManager>();
    }
    public void SlideLeft()
    {
        if (!shopIsOpen)
        {
            anim.SetTrigger("Slide Left");
            shopIsOpen = true;
        }
    }
    public void SlideRight()
    {
        if (shopIsOpen)
        {
            anim.SetTrigger("Slide Right");
            gm.Unpause();
            shopIsOpen = false;
        }
    }
    void ShopOpened()
    {
        gm.Pause();
    }
    void ShopClosed()
    {
        //
    }
}
