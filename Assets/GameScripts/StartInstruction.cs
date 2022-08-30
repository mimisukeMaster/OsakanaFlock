using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Boid.OOP;

public class StartInstruction : MonoBehaviour
{
    [SerializeField]
    Text AskText;
    [SerializeField]
    Button showSlideYes;
    [SerializeField]
    Button showSlideNo;
    [SerializeField]
    Text NextPageInfoText;
    [SerializeField]
    Sprite Opening_01;
    [SerializeField]
    Sprite Opening_02;

    [SerializeField]
    Sprite Opening_03;

    [SerializeField]
    Sprite Opening_04;

    [SerializeField]
    Sprite Opening_05;

    [SerializeField]
    Sprite Opening_06;

    [SerializeField]
    Sprite Opening_07;
    [SerializeField]
    Button GameStartButton;
    [SerializeField]
    GameManager gameManager;
    [SerializeField]
    Simulation simulation;
    [SerializeField]
    AudioSource gameAudio;
    [SerializeField]
    AudioClip showSlideSE;
    [SerializeField]
    AudioClip nextSlideSE;

    Image instructionImg;
    bool showSlide;
    int slideCount = 1;
    List<GameObject> HideObjs = new List<GameObject>();

    // Start is called before the first frame update
    public void Start()
    {
        instructionImg = GetComponent<Image>();
        slideCount = 1;

        // はい　いいえ　のスライド
        instructionImg.gameObject.SetActive(true);
        AskText.gameObject.SetActive(true);
        showSlideYes.gameObject.SetActive(true);
        showSlideNo.gameObject.SetActive(true);

        // まだ現れなくていいものは見えなくする
        instructionImg.color = Color.clear;
        GameStartButton.gameObject.SetActive(false);
        NextPageInfoText.gameObject.SetActive(false);

        foreach (GameObject obj in GameObject.FindObjectsOfType(typeof(GameObject)))
        {
            if (obj.tag == "GamingObj")
            {
                HideObjs.Add(obj);
                obj.gameObject.SetActive(false);
            }
        }

        Camera.main.transform.rotation = Quaternion.Euler(0, 0, 0);

    }

    // Update is called once per frame
    void Update()
    {
        if (!showSlide) Camera.main.transform.Rotate(Vector3.up * 0.01f);
        if (Input.GetMouseButtonDown(0) && showSlide)
        {
            switch (slideCount)
            {
                // 1の時はYesボタンクリック時に見せる
                case 2:
                    instructionImg.sprite = Opening_02;
                    gameAudio.PlayOneShot(nextSlideSE);
                    break;
                case 3:
                    instructionImg.sprite = Opening_03;
                    gameAudio.PlayOneShot(nextSlideSE);
                    break;
                case 4:
                    instructionImg.sprite = Opening_04;
                    gameAudio.PlayOneShot(nextSlideSE);
                    break;
                case 5:
                    instructionImg.sprite = Opening_05;
                    gameAudio.PlayOneShot(nextSlideSE);
                    break;
                case 6:
                    instructionImg.sprite = Opening_06;
                    gameAudio.PlayOneShot(nextSlideSE);
                    break;
                case 7:
                    instructionImg.sprite = Opening_07;
                    gameAudio.PlayOneShot(nextSlideSE);
                    NextPageInfoText.gameObject.SetActive(false);
                    GameStartButton.gameObject.SetActive(true);
                    showSlide = false;
                    break;
            }
            slideCount++;
        }
    }
    public void ShowSlideButtonDown()
    {
        showSlide = true;
        instructionImg.color = Color.white;
        instructionImg.sprite = Opening_01;
        slideCount++;
        gameAudio.PlayOneShot(showSlideSE);
        NextPageInfoText.gameObject.SetActive(true);

        AskText.gameObject.SetActive(false);
        showSlideYes.gameObject.SetActive(false);
        showSlideNo.gameObject.SetActive(false);
    }
    public void GameStartButtonDown()
    {
        /// ゲーム開始フラグ
        /// このフラグで<seealso cref="ManipulateController"</>経由でSimulationがアクティブになる
        gameManager.isGaming = true;
        simulation.isGameStarted = true;
        simulation.isFirstGameFrame = true;

        // Game Obj アクティブ
        foreach (GameObject obj in HideObjs)
        {
            obj.SetActive(true);
        }

        // ジェスチャーシグナルのタイマーセット
        simulation.timer_powerful = Time.realtimeSinceStartup;
        simulation.timer_flocking = Time.realtimeSinceStartup;

        // instruction UI 非アクティブ
        AskText.gameObject.SetActive(false);
        showSlideYes.gameObject.SetActive(false);
        showSlideNo.gameObject.SetActive(false);
        GameStartButton.gameObject.SetActive(false);
        instructionImg.gameObject.SetActive(false); // 自身
    }

}
