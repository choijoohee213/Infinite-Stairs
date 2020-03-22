using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Animator anim;
    public GameManager gameManager;
    public bool isStairMove = false;
    public int buttonIndex;
    public bool isleft = true;
    public bool isDie = false;

    void Start()
    {
        anim = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) && !isDie)
        {
            if (Input.GetMouseButtonDown(0)) buttonIndex = 0;
            else if (Input.GetMouseButtonDown(1))
            {
                buttonIndex = 1;
                isleft = !isleft;
            }
            MoveAnimation();
            isStairMove = true;
        }


        if (isDie) anim.SetTrigger("Die");
    }

    public void MoveAnimation()
    {
        //FilpX
        if (!isleft)
            transform.rotation = Quaternion.Euler(0, -180, 0);
        else
            transform.rotation = Quaternion.Euler(0, 0, 0);

        anim.SetInteger("Moving",0);
        Invoke("IdleAnimation", 0.05f);
        
    }

    public void IdleAnimation()
    {
        anim.SetInteger("Moving", -1);
    }

}
