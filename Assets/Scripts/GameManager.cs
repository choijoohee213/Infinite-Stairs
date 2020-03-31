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
    public GameObject[] stairs, UI;
    public GameObject pauseBtn, backGround;

    public AudioSource[] sound;
    public Animator[] anim;
    public Text finalScoreText, bestScoreText, scoreText;
    public Image gauge;
    public Button[] settingButtons;

    Queue<int> stairInform;
    int stairCount, score, sceneCount;
    bool gaugeStart = false, vibrationOn = true;
    float gaugeRedcutionRate = 0.0016f;

    Vector3 beforePos,
    startPos = new Vector3(-0.9f, -1.5f, 0),
    leftPos = new Vector3(-0.9f, 0.5f, 0),
    rightPos = new Vector3(0.9f, 0.5f, 0),
    leftDir = new Vector3(0.9f, -0.5f, 0),
    rightDir = new Vector3(-0.9f, -0.5f, 0);

    enum State { start, leftDir, rightDir }
    State state = State.start;


    void Awake() {

        player = objectManager.player.GetComponent<Player>();
        player.gameManager = this;
        player.dslManager = dslManager;

        stairInform = new Queue<int>();
        stairCount = Random.Range(1, 11);
        for (int i = 0; i < stairCount + 1; i++) stairInform.Enqueue(0);

        Init();
        GaugeReduce();
        UI[0].SetActive(dslManager.IsRetry());
        UI[1].SetActive(!dslManager.IsRetry());
    }


    void Update() {
        if (player.isStairMove) {
            player.isStairMove = false;
            gaugeStart = true;
            
        }

        //Game Over When Gauge Becomes 0
        if (gauge.fillAmount == 0 && !player.isDie)
            GameOver();

    }



    //Initially Spawn The Stairs
    void Init() {
        for (int i = 0; i < 20; i++) {
            switch (state) {
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

            if (state == State.start)
                state = State.leftDir;
            else {
                //Coin object activation according to random probability
                if (Random.Range(1, 9) < 3) objectManager.MakeObj("coin", i);

                if (--stairCount == 0) {
                    if (state == State.leftDir) state = State.rightDir;
                    else if (state == State.rightDir) state = State.leftDir;
                    stairCount = Random.Range(1, 9);
                    PushQueue(stairCount);
                }
            }
        }
    }


    //Spawn The Stairs At The Random Location
    void SpawnStair(int num) {
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

        if (--stairCount == 0) {
            //Direction change required
            if (state == State.leftDir) state = State.rightDir;
            else if (state == State.rightDir) state = State.leftDir;
            stairCount = Random.Range(1, 9);
            PushQueue(stairCount);
        }
    }



    //Put Information Of Stairs In Queue
    void PushQueue(int stairCount) {
        for (int i = 0; i < stairCount; i++) {
            if (i == 0) stairInform.Enqueue(1);
            else stairInform.Enqueue(0);
        }
    }



    //Stairs Moving Along The Direction       
    public void StairMove(int buttonIndex, bool isleft) {
        if (player.isDie) return;
        int stairIndex = stairInform.Dequeue();
        print("dequeue" + stairIndex);
        print(buttonIndex);

        //Move stairs to the right or left
        for (int i = 0; i < stairs.Length; i++) {
            if (isleft) stairs[i].transform.position += leftDir;
            else stairs[i].transform.position += rightDir;
        }

        //Move the stairs below a certain height
        for (int i = 0; i < stairs.Length; i++)
            if (stairs[i].transform.position.y < -5) SpawnStair(i);

        //Game over if climbing stairs is wrong
        if (buttonIndex != stairIndex) {
            GameOver();
            return;
        }

        //Score Update & Gauge Increase
        scoreText.text = (++score).ToString();
        gauge.fillAmount += 0.2f;
        backGround.transform.position += backGround.transform.position.y < -14f ?
            new Vector3(0, 4.7f, 0) : new Vector3(0, -0.05f, 0);
    }



    void GaugeReduce() {
        if (gaugeStart) {
            //Gauge Reduction Rate Increases As Score Increases
            if (score > 30) gaugeRedcutionRate = 0.0023f;
            if (score > 60) gaugeRedcutionRate = 0.003f;
            if (score > 100) gaugeRedcutionRate = 0.0035f;
            if (score > 150) gaugeRedcutionRate = 0.004f;
            if (score > 200) gaugeRedcutionRate = 0.005f;
            gauge.fillAmount -= gaugeRedcutionRate;
        }
        Invoke("GaugeReduce", 0.01f);
    }


    void GameOver() {
        //Animation
        anim[0].SetBool("GameOver", true);
        player.anim.SetBool("Die", true);

        //UI
        ShowScore();
        pauseBtn.SetActive(false);

        dslManager.SaveMoney(player.money);
        player.isDie = true;
        if (vibrationOn) Vibration();

        CancelInvoke();  //GaugeBar Stopped      
        Invoke("DisableUI", 1.5f);
        dontDestory.InvokeDestoryObj();

    }

    void ShowScore() {
        finalScoreText.text = score.ToString();
        dslManager.SaveRankScore(score);
        bestScoreText.text = dslManager.GetBestScore().ToString();

        //If you set the highest record
        if (score == dslManager.GetBestScore())
            UI[2].SetActive(true);
    }


    public void BtnDown(GameObject btn) {
        btn.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        if (btn.name == "ClimbBtn") {
            player.isClimbBtn = true;
            player.Climb();            
        }
        if (btn.name == "ChangeDirBtn") {
            player.isChangeDirBtn = true;
            player.ChangeDir();
        }
    }


    public void BtnUp(GameObject btn) {
        btn.transform.localScale = new Vector3(1f, 1f, 1f);
        if (btn.name == "ClimbBtn") {
            player.isClimbBtn = false;
        }
        if (btn.name == "ChangeDirBtn") {
            player.isChangeDirBtn = false;
        }

        if (btn.name == "PauseBtn") CancelInvoke();
        if (btn.name == "ResumeBtn") GaugeReduce();
       
            
    }



    //#.Setting
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
                on = dslManager.GetSettingOn(btn.name);
                if (on) settingButtons[i].image.color = new Color(1, 1, 1, 1f);
                else settingButtons[i].image.color = new Color(1, 1, 1, 0.5f);
            }
        if (btn.name == "SoundBtn") {
            for (int i = 2; i < 4; i++) {
                on = dslManager.GetSettingOn("SoundBtn");
                if (on) settingButtons[i].image.color = new Color(1, 1, 1, 1f);
                else settingButtons[i].image.color = new Color(1, 1, 1, 0.5f);
            }
        }
        if (btn.name == "VibrateBtn") {
            for (int i = 4; i < 6; i++) {
                on = dslManager.GetSettingOn("VibrateBtn");
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
                for (int i = 0; i < sound.Length; i++)
                    sound[i].mute = !dslManager.GetSettingOn(type);
                player.SoundOnOff(dslManager.GetSettingOn(type));
                break;
            case "VibrateBtn":
                vibrationOn = dslManager.GetSettingOn(type);
                break;
        }       
        print(dslManager.GetSettingOn(type));
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
        dslManager.SaveMoney(player.money);
        SceneManager.LoadScene(i);
    }

}
