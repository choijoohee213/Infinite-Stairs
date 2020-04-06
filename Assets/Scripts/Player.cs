using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Animator anim;
    public AudioSource[] sound;
    public GameManager gameManager;
    public DSLManager dslManager;
    public bool isleft = true, isDie = false;
    public int characterIndex, stairIndex, money;

    void Awake() {
        anim = gameObject.GetComponent<Animator>();
        money = dslManager.GetMoney();
    }

    public void Climb(bool isChange)
    {
        if (isChange) isleft = !isleft;
        gameManager.StairMove(stairIndex, isChange, isleft);
        if ((++stairIndex).Equals(20)) stairIndex = 0;
        MoveAnimation();
        gameManager.gaugeStart = true;
    }


    public void MoveAnimation()
    {
        //Change left and right when changing direction
        if (!isleft)
            transform.rotation = Quaternion.Euler(0, -180, 0);
        else
            transform.rotation = Quaternion.Euler(0, 0, 0);

        if (isDie) return;
        anim.SetBool("Move",true);
        gameManager.PlaySound(1);
        Invoke("IdleAnimation", 0.05f);        
    }

    public void IdleAnimation()
    {
        anim.SetBool("Move", false);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Coin")
        {
            collision.gameObject.SetActive(false);
            gameManager.PlaySound(0);
            money += 2;
            dslManager.LoadMoney(money);
        }
    }


   
}
