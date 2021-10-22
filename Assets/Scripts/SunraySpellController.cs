using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunraySpellController : MonoBehaviour
{
    public Animator circleAnimator, rayAnimator;
    public GameObject spellCircle;
    void Start()
    {
        circleAnimator.SetTrigger("circle");
    }

    // Update is called once per frame
    void Update()
    {
        /*Animation ass;
        if (as)
            rayAnimator.SetTrigger("ray");*/

    }
}
