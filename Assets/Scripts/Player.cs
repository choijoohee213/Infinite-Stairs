using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Animator anim;
    public AudioSource[] sound;
    public GameManager gameManager;
    public DSLManager dslManager;
    public bool isStairMove = false, isleft = true, isDie = false, 
        isClimbBtn = false, isChangeDirBtn = false;
    public int buttonIndex, money;

    void Start()
    {
        anim = GetComponent<Animator>();
        money = dslManager.GetMoney();
    }

    public void Climb()
    {
        if (isDie || isChangeDirBtn) return;
        gameManager.StairMove(0,isleft);
        MoveAnimation();
        isStairMove = true;
    }

    public void ChangeDir()
    {
        if (isDie || isClimbBtn) return;
        isleft = !isleft;
        gameManager.StairMove(1, isleft);
        MoveAnimation();
        isStairMove = true;
    }


    public void MoveAnimation()
    {
        //Change left and right when changing direction
        if (!isleft)
            transform.rotation = Quaternion.Euler(0, -180, 0);
        else
            transform.rotation = Quaternion.Euler(0, 0, 0);

        anim.SetBool("Move",true);
        gameManager.PlaySound(1);
        Invoke("IdleAnimation", 0.05f);
        
    }

    public void IdleAnimation()
    {
        anim.SetBool("Move", false);
    }

    public void SoundOnOff(bool soundOn) {
        sound[0].mute = !soundOn;
        sound[1].mute = !soundOn;
        sound[2].mute = !soundOn;
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
