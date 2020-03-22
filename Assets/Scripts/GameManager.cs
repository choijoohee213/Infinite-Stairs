using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public Player player;
    public GameObject[] stairs;
    public Sprite[] character;
    int stairCount;
    Queue<int> stairInform;

    Vector3 beforePos;
    Vector3 startPos = new Vector3(0, -2.5f, 0);
    Vector3 leftPos = new Vector3(-1, 0.5f, 0);
    Vector3 rightPos = new Vector3(1, 0.5f, 0);

    enum State { start, leftDir, rightDir }
    State state = State.start;

    void Awake()
    {
        stairInform = new Queue<int>();
        stairCount = Random.Range(1, 11);       
        for(int i=0; i<stairCount; i++)
            stairInform.Enqueue(0);
        Init();

    }

    void Update()
    {
        if (player.isStairMove)
        {
            player.isStairMove = false;
            Invoke("StairMove", 0.1f);
        }
    }

    //Initially Spawn The Stairs
    void Init()
    {      
        for (int i = 0; i < 20; i++)
        {
            switch (state)
            {
                case State.start:
                    stairs[i].transform.position = startPos;
                    break;
                case State.leftDir:
                    stairs[i].transform.position = beforePos + leftPos;
                    break;
                case State.rightDir:
                    stairs[i].transform.position = beforePos + rightPos;
                    break;
            }
            beforePos = stairs[i].transform.position;
         
            if (state == State.start) state = State.leftDir;
            else
                if (--stairCount == 0)
            {
                if (state == State.leftDir) state = State.rightDir;
                else if (state == State.rightDir) state = State.leftDir;
                stairCount = Random.Range(1, 11);
                PushQueue(stairCount);
            }
        }    
    }



    //Spawn The Stairs At The Random Location
    void SpawnStair(int num)
    {
        Debug.Log(num);
        beforePos = stairs[num == 0? 19 : num-1].transform.position;
        switch (state)
        {            
            case State.leftDir:
                stairs[num].transform.position = beforePos + leftPos;
                break;
            case State.rightDir:
                stairs[num].transform.position = beforePos + rightPos;
                break;
        }
   
        if(--stairCount == 0)
        {
            if (state == State.leftDir) state = State.rightDir;
            else if (state == State.rightDir) state = State.leftDir;
            stairCount = Random.Range(1, 11);
            PushQueue(stairCount);
        }
    }


    //Put Information Of Stairs In Queue
    void PushQueue(int stairCount)
    {
        for(int i=0; i<stairCount; i++)
        {
            if (i == 0) stairInform.Enqueue(1);
            else stairInform.Enqueue(0);
        }
    }


    //Stairs Moving Along The Direction       
    void StairMove()
    {
        Vector3 leftDir = new Vector3(1, -0.5f, 0);
        Vector3 rightDir = new Vector3(-1, -0.5f, 0);
        int stairIndex = stairInform.Dequeue();

        for (int i = 0; i < stairs.Length; i++)
        {
            if (player.isleft) { stairs[i].transform.position += leftDir; Debug.Log("left"); }
            else {stairs[i].transform.position += rightDir; Debug.Log("right"); }
        }

        for (int i = 0; i < stairs.Length; i++)
            if (stairs[i].transform.position.y < -5) SpawnStair(i);

        if (player.buttonIndex != stairIndex)
            GameOver();

    }



    void GameOver()
    {
        player.isDie = true;
    }
}
