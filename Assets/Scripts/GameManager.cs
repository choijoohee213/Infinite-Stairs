using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour {
    public Player player;
    public ObjectManager objectManager;
    public DSLManager dslManager;
    public DontDestory dontDestory;
    public GameObject[] players, stairs, UI;
    public GameObject pauseBtn, backGround;

    public AudioSource[] sound;
    public Animator[] anim;
    public Text finalScoreText, bestScoreText, scoreText;
    public Image gauge;
    public Button[] settingButtons;

    int score, sceneCount, selectedIndex;
    public bool gaugeStart = false, vibrationOn = true, isGamePaused = false;
    float gaugeRedcutionRate = 0.0025f;
    public bool[] IsChangeDir = new bool[20];

    Vector3 beforePos,
    startPos = new Vector3(-0.8f, -1.5f, 0),
    leftPos = new Vector3(-0.8f, 0.4f, 0),
    rightPos = new Vector3(0.8f, 0.4f, 0),
    leftDir = new Vector3(0.8f, -0.4f, 0),
    rightDir = new Vector3(-0.8f, -0.4f, 0);

    enum State { start, leftDir, rightDir }
    State state = State.start;


    void Awake() {
        players[selectedIndex].SetActive(true);
        player = players[selectedIndex].GetComponent<Player>();

        StairsInit();
        GaugeReduce();
        StartCoroutine("CheckGauge");

        UI[0].SetActive(dslManager.IsRetry());
        UI[1].SetActive(!dslManager.IsRetry());        
    }


    //Initially Spawn The Stairs
    void StairsInit() {
        for (int i = 0; i < 20; i++) {
            switch (state) {
                case State.start:
                    stairs[i].transform.position = startPos;
                    state = State.leftDir;
                    break;
                case State.leftDir:
                    stairs[i].transform.position = beforePos + leftPos;
                    break;
                case State.rightDir:
                    stairs[i].transform.position = beforePos + rightPos;
                    break;
            }
            beforePos = stairs[i].transform.position;

            if (i != 0) {
                //Coin object activation according to random probability
                if (Random.Range(1, 9) < 3) objectManager.MakeObj("coin", i);
                if (Random.Range(1, 9) < 3 && i < 19) {
                    if (state == State.leftDir) state = State.rightDir;
                    else if (state == State.rightDir) state = State.leftDir;
                    IsChangeDir[i + 1] = true;
                }
            }
        }
    }




    //Spawn The Stairs At The Random Location
    void SpawnStair(int num) {
        IsChangeDir[num + 1 == 20 ? 0 : num + 1] = false;
        beforePos = stairs[num == 0 ? 19 : num - 1].transform.position;
        switch (state) {
            case State.leftDir:
                stairs[num].transform.position = beforePos + leftPos;
                break;
            case State.rightDir:
                stairs[num].transform.position = beforePos + rightPos;
                break;
        }

        //Coin object activation according to random probability
        if (Random.Range(1, 9) < 3) objectManager.MakeObj("coin", num);
        if (Random.Range(1, 9) < 3) {
            if (state == State.leftDir) state = State.rightDir;
            else if (state == State.rightDir) state = State.leftDir;
            IsChangeDir[num+1 == 20? 0 : num+1] = true;
        }
    }



    //Stairs Moving Along The Direction       
    public void StairMove(int stairIndex, bool isChange, bool isleft) {
        if (player.isDie) return;

        //Move stairs to the right or left
        for (int i = 0; i < 20; i++) {
            if (isleft) stairs[i].transform.position += leftDir;
            else stairs[i].transform.position += rightDir;
        }

        //Move the stairs below a certain height
        for (int i = 0; i < 20; i++)
            if (stairs[i].transform.position.y < -5) SpawnStair(i);

        //Game over if climbing stairs is wrong
        if(IsChangeDir[stairIndex] != isChange) {
            GameOver();
            return;
        }

        //Score Update & Gauge Increase
        scoreText.text = (++score).ToString();
        gauge.fillAmount += 0.7f ;
        backGround.transform.position += backGround.transform.position.y < -14f ?
            new Vector3(0, 4.7f, 0) : new Vector3(0, -0.05f, 0);
    }


    //#.Gauge
    void GaugeReduce() {
        if (gaugeStart) {
            //Gauge Reduction Rate Increases As Score Increases
            if (score > 30) gaugeRedcutionRate = 0.0033f;
            if (score > 60) gaugeRedcutionRate = 0.0037f;
            if (score > 100) gaugeRedcutionRate = 0.0043f;
            if (score > 150) gaugeRedcutionRate = 0.005f;
            if (score > 200) gaugeRedcutionRate = 0.005f;
            if (score > 300) gaugeRedcutionRate = 0.0065f;
            if (score > 400) gaugeRedcutionRate = 0.0075f;
            gauge.fillAmount -= gaugeRedcutionRate;
        }
        Invoke("GaugeReduce", 0.01f);
    }

    
    IEnumerator CheckGauge() {
        while (gauge.fillAmount != 0) {
            yield return new WaitForSeconds(0.4f);
        }
        GameOver();
    }


    void GameOver() {
        //Animation
        anim[0].SetBool("GameOver", true);
        player.anim.SetBool("Die", true);

        //UI
        ShowScore();
        pauseBtn.SetActive(false);

        player.isDie = true;
        player.MoveAnimation();
        if (vibrationOn) Vibration();
        dslManager.SaveMoney(player.money);

        CancelInvoke();  //GaugeBar Stopped      
        Invoke("DisableUI", 1.5f);
    }


    //Show score after game over
    void ShowScore() {
        finalScoreText.text = score.ToString();
        dslManager.SaveRankScore(score);
        bestScoreText.text = dslManager.GetBestScore().ToString();

        //When the highest score is recorded
        if (score == dslManager.GetBestScore() && score != 0)
            UI[2].SetActive(true);
    }



    public void BtnDown(GameObject btn) {
        btn.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        if (btn.name == "ClimbBtn")  player.Climb(false);
        else if (btn.name == "ChangeDirBtn") player.Climb(true);
    }


    public void BtnUp(GameObject btn) {
        btn.transform.localScale = new Vector3(1f, 1f, 1f);
        if (btn.name == "PauseBtn") {
            CancelInvoke();  //Gauge Stopped
            isGamePaused = true;
        }
        if (btn.name == "ResumeBtn") {
            GaugeReduce();
            isGamePaused = false;
        }
    }



    //#.Setting
    public void SoundInit() {
        selectedIndex = dslManager.GetSelectedCharIndex();
        player = players[selectedIndex].GetComponent<Player>();
        sound[3] = player.sound[0];
        sound[4] = player.sound[1];
        sound[5] = player.sound[2];
    }


    public void SettingBtnInit() {
        bool on;
        for (int i = 0; i < 2; i++) {
            on = dslManager.GetSettingOn("BgmBtn");
            if (on) settingButtons[i].image.color = new Color(1, 1, 1, 1f);
            else settingButtons[i].image.color = new Color(1, 1, 1, 0.5f);
        }

        for (int i = 2; i < 4; i++) {
            on = dslManager.GetSettingOn("SoundBtn");
            if (on) settingButtons[i].image.color = new Color(1, 1, 1, 1f);
            else settingButtons[i].image.color = new Color(1, 1, 1, 0.5f);
        }

        for (int i = 4; i < 6; i++) {
            on = dslManager.GetSettingOn("VibrateBtn");
            if (on) settingButtons[i].image.color = new Color(1, 1, 1, 1f);
            else settingButtons[i].image.color = new Color(1, 1, 1, 0.5f);
        }
    }


    public void SettingBtnChange(Button btn) {
        bool on = dslManager.GetSettingOn(btn.name);
        if (btn.name == "BgmBtn")
            for (int i = 0; i < 2; i++) {
                if (on) settingButtons[i].image.color = new Color(1, 1, 1, 1f);
                else settingButtons[i].image.color = new Color(1, 1, 1, 0.5f);
            }
        if (btn.name == "SoundBtn") {
            for (int i = 2; i < 4; i++) {               
                if (on) settingButtons[i].image.color = new Color(1, 1, 1, 1f);
                else settingButtons[i].image.color = new Color(1, 1, 1, 0.5f);
            }
        }
        if (btn.name == "VibrateBtn") {
            for (int i = 4; i < 6; i++) {
                if (on) settingButtons[i].image.color = new Color(1, 1, 1, 1f);
                else settingButtons[i].image.color = new Color(1, 1, 1, 0.5f);
            }
        }
    }

    public void SettingOnOff(string type) {
        switch (type) {
            case "BgmBtn":
                if (dslManager.GetSettingOn(type)) { dontDestory.BgmPlay(); }
                else dontDestory.BgmStop();
                break;
            case "SoundBtn":
                bool isOn = !dslManager.GetSettingOn(type);
                for (int i = 0; i < sound.Length; i++) 
                    sound[i].mute = isOn;
                break;
            case "VibrateBtn":
                vibrationOn = dslManager.GetSettingOn(type);
                break;
        }       
    }

    void Vibration()
    {
        Handheld.Vibrate();
        sound[0].playOnAwake = false;
    }


    public void PlaySound(int index) {
        sound[index].Play();
    }

    void DisableUI()
    {
        UI[0].SetActive(false);
    }


    public void LoadScene(int i)
    {
        SceneManager.LoadScene(i);
    }

    
    private void OnApplicationQuit() {
        dslManager.SaveMoney(player.money);
    }   
}
