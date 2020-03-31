using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestory : MonoBehaviour
{ 
    public static DontDestory Instance;
    AudioSource bgm;

    private void Start() {
        bgm = GetComponent<AudioSource>();
    }
    private void Awake() {
       
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
    }

    public void InvokeDestoryObj() {
        Invoke("DestoryObj", 1.5f);
    }

    void DestoryObj() {
        Destroy(gameObject);
    }

    public void BgmPlay() {
        bgm = Instance.GetComponent<AudioSource>();
        bgm.enabled = true;
    }

    public void BgmStop() {
        bgm = Instance.GetComponent<AudioSource>();
        bgm.enabled = false;
    }
}


