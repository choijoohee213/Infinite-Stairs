using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public GameManager gameManager;
    public DSLManager dslManager;
    public GameObject coinPrefab;
    public GameObject[] characterPrefabs;

    GameObject[] coin;
    public GameObject player;
    GameObject[] targetPool;

    void Awake()
    {
        coin = new GameObject[20];
        Generate();
    }

    void Generate()
    {
        for(int i=0; i<coin.Length; i++)
        {
            coin[i] = Instantiate(coinPrefab, gameManager.stairs[i].transform);
            coin[i].transform.position += new Vector3(0, 0.6f, 0);
            coin[i].SetActive(false);
        }

        player = Instantiate(characterPrefabs[dslManager.GetSelectedCharIndex()], new Vector3(0f, -0.5f, 0), Quaternion.Euler(new Vector3(0, 0, 0)));
    }

    public void MakeObj(string type, int index)
    {
        switch (type)
        {
            case "coin":
                targetPool = coin;
                break;

        }

        if (!targetPool[index].activeSelf)
        {
            targetPool[index].SetActive(true);
        }
    }


    public GameObject[] GetPool(string type)
    {
        switch (type)
        {
            case "coin":
                targetPool = coin;
                break;
           
        }
        return targetPool;
    }
}
