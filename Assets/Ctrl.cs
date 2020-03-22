using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ctrl : MonoBehaviour
{
    public Transform[] stairs;
    private int currentStair;
    private bool[] isStairRight = new bool[20];
    public Text scoreTxt;
    private int score;
    public GameObject btn;
    private bool isOver;

    void Start()
    {
        Restart();
    }

    public void Restart()
    {
        isOver = false;
        btn.SetActive(false);
        currentStair = 1;
        score = 0;
        scoreTxt.text = "Score : 0";
        stairs[0].position = new Vector2(0, -0.25f);
        for (int i = 1; i < stairs.Length; i++)
        {
            isStairRight[i] = Random.Range(0, 2) == 0;
            if (isStairRight[i])
            {
                stairs[i].position = new Vector2(stairs[i - 1].position.x + 1, stairs[i - 1].position.y + 0.5f);
            }
            else
            {
                stairs[i].position = new Vector2(stairs[i - 1].position.x - 1, stairs[i - 1].position.y + 0.5f);
            }
        }
    }

    void Update()
    {
        if (isOver) return;
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (isStairRight[currentStair])
            {
                NextStair();
                score++;
                scoreTxt.text = "Score : " + score;
            }
            else
            {
                isOver = true;
                btn.SetActive(true);
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (!isStairRight[currentStair])
            {
                NextStair();
                score++;
                scoreTxt.text = "Score : " + score;
            }
            else
            {
                isOver = true;
                btn.SetActive(true);
            }
        }
    }

    void NextStair()
    {
        for (int i = 0; i < stairs.Length; i++)
        {
            if (isStairRight[currentStair])
            {
                stairs[i].position = new Vector2(stairs[i].position.x - 1, stairs[i].position.y - 0.5f);
            }
            else
            {
                stairs[i].position = new Vector2(stairs[i].position.x + 1, stairs[i].position.y - 0.5f);
            }
        }
        for (int i = 0; i < stairs.Length; i++)
        {
            if (stairs[i].position.y < -4.8f)
            {
                isStairRight[i] = Random.Range(0, 2) == 0;
                if (isStairRight[i])
                {
                    if (i == 0)
                    {
                        stairs[i].position = new Vector2(stairs[19].position.x + 1, 4.75f);
                    }
                    else
                    {
                        stairs[i].position = new Vector2(stairs[i - 1].position.x + 1, 4.75f);
                    }
                }
                else
                {
                    if (i == 0)
                    {
                        stairs[i].position = new Vector2(stairs[19].position.x - 1, 4.75f);
                    }
                    else
                    {
                        stairs[i].position = new Vector2(stairs[i - 1].position.x - 1, 4.75f);
                    }
                }
            }
        }
        currentStair++;
        if (currentStair >= 20) currentStair = 0;
    }
}
