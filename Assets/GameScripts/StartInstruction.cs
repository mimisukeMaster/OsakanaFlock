using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Boid.OOP;

public class StartInstruction : MonoBehaviour
{
    [Header("UI before game start")]
    [SerializeField]
    Text AskText;
    [SerializeField]
    Button showSlideYes;
    [SerializeField]
    Button showSlideNo;
    [SerializeField]
    Text NextPageInfoText;
    [SerializeField]
    Button GameStartButton;

    [Header("Slide before game start")]
    [SerializeField]
    Image instructionImg;

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

    [Header("Audio")]
    [SerializeField]
    AudioSource gameAudio;
    [SerializeField]
    AudioClip showSlideSE;
    [SerializeField]
    AudioClip nextSlideSE;

    [Header("Public references")]
    [SerializeField]
    GameManager gameManager;
    [SerializeField]
    UIManager uiManager;
    [SerializeField]
    Simulation simulation;

    /// <summary>
    /// 状況フラグ：スライドを見る/見ているか
    /// </summary>
    bool showSlide;

    /// <summary>
    /// スライドのページ送りカウンタ
    /// </summary>
    int slideCount = 1;
    //List<GameObject> HideObjs = new List<GameObject>();

    public void Start()
    {
        // 初期化
        slideCount = 1;

        // 自身のGameObjectはこのScriptがついていて非アクティブにできないので透明化して見えなくする
        //instructionImg.color = Color.clear;

        // ゲーム説明選択オブジェクト表示
        AskText.gameObject.SetActive(true);
        showSlideYes.gameObject.SetActive(true);
        showSlideNo.gameObject.SetActive(true);

        // ゲーム開始前でもまだ現れないものは非アクティブ
        instructionImg.gameObject.SetActive(false);
        GameStartButton.gameObject.SetActive(false);
        NextPageInfoText.gameObject.SetActive(false);


        foreach (var obj in gameManager.gamingObj)
        {
            obj.SetActive(false);
        }
        // foreach (GameObject obj in GameObject.FindObjectsOfType(typeof(GameObject)))
        // {
        //     if (obj.tag == "GamingObj")
        //     {
        //         HideObjs.Add(obj);
        //         obj.gameObject.SetActive(false);
        //     }
        // }

        Camera.main.transform.rotation = Quaternion.Euler(0, 0, 0);

    }

    // Update is called once per frame
    void Update()
    {

        // ゲーム開始前ゆっくり回転
        if (gameManager.beforeStart) Camera.main.transform.Rotate(Vector3.up * 0.01f);

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

                    // スライド終了
                    showSlide = false;
                    break;
            }
            slideCount++;
        }
    }

    // ゲーム説明「はい」押下時
    public void ShowSlideButtonDown()
    {
        // スライドを見る
        showSlide = true;

        // スライド遷移SE
        gameAudio.PlayOneShot(showSlideSE);

        // スライド表示をアクティブに
        instructionImg.gameObject.SetActive(true);
        NextPageInfoText.gameObject.SetActive(true);

        //ゲーム説明選択非アクティブに
        AskText.gameObject.SetActive(false);
        showSlideYes.gameObject.SetActive(false);
        showSlideNo.gameObject.SetActive(false);

        // スライドセット
        instructionImg.sprite = Opening_01;

        // ページカウントアップ
        slideCount++;

    }
    public void GameStartButtonDown()
    {
        /// ゲーム開始
        gameManager.isGaming = true;

        //simulation.isGameStarted = true;
        //simulation.isFirstGameFrame = true;

        // ゲーム開始前UI 非アクティブ
        AskText.gameObject.SetActive(false);
        showSlideYes.gameObject.SetActive(false);
        showSlideNo.gameObject.SetActive(false);
        GameStartButton.gameObject.SetActive(false);
        instructionImg.gameObject.SetActive(false);

        // GamingObj アクティブ
        foreach (var obj in gameManager.gamingObj)
        {
            obj.SetActive(true);
        }
        // ゲーム開始後BoidViewBackだけはまだ非表示
        uiManager.BoidViewBack.gameObject.SetActive(false);

        // foreach (GameObject obj in HideObjs)
        // {
        //     obj.SetActive(true);
        // }

        // カメラの回転リセット
        Camera.main.transform.eulerAngles = Vector3.zero;

        // 信号のタイマーセット
        simulation.timer_powerful = Time.realtimeSinceStartup;
        simulation.timer_flocking = Time.realtimeSinceStartup;

    }

}
