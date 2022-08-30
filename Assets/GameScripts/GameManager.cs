using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using Boid.OOP;
using Boid;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    StartInstruction startInstruction;
    [SerializeField]
    UIManager uiManager;
    [SerializeField]
    ManipulateController manipulateController;
    [SerializeField]
    Simulation simulation;
    [SerializeField]
    Param param;
    [SerializeField]
    Text RemainTimeText;
    [SerializeField]
    AudioClip StartInstructionBGM;
    [SerializeField]
    AudioSource gameAudio;
    [SerializeField]
    AudioClip GameBGM;
    [SerializeField]
    AudioClip ScoreBGM;
    [SerializeField]
    AudioClip gameStartSE;
    [SerializeField]
    AudioClip gameFinishSE;

    public float GameTime = 100;
    public float GameRemainTime;
    public int ResultScore;
    public bool isGaming; /// 初めてTrueになるのは<seealso cref="StartInstruction.GameStartButtonDown()"</>
    public bool BoidsAreAllDead;
    public bool beforeStart;
    public bool firstStart;

    public GameObject[] gamingObj;


    void Awake()
    {
        // 非アクティブにする前にGamingObjタグ付きのものを配列化
        gamingObj = GameObject.FindGameObjectsWithTag("GamingObj");
    }
    // Start is called before the first frame update
    void Start()
    {
        GameTime = 100f;
        GameRemainTime = GameTime;
        beforeStart = true;
        firstStart = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isGaming)
        {
            // ゲーム開始
            beforeStart = false;

            // 開始SE
            if (firstStart)
            {
                gameAudio.PlayOneShot(gameStartSE);
                firstStart = false;
            }
            // 残り時間減らしていく
            GameRemainTime -= Time.deltaTime;
            RemainTimeText.text = "残り時間\n" + GameRemainTime.ToString("F1") + "秒";

            // 非アクティブだったものをアクティブ化
            simulation?.gameObject.SetActive(true);
            manipulateController.gameObject.SetActive(true);

            // ゲーム中BGM処理
            if (gameAudio.clip != GameBGM)
            {
                gameAudio.clip = GameBGM;
                gameAudio.Play();
            }

            // ゲーム終了処理
            if (GameRemainTime < 0 || BoidsAreAllDead)
            {
                /// ゲーム開始直後(まだBoidいない時)にここが実行されて即終了になってしまうので、開始直後はスルー
                if (GameTime - GameRemainTime < 1) return;

                // deltatimeのござを埋めるため綺麗に0を入れておく
                GameRemainTime = 0;

                // ゲーム中のObjectを消す処理
                var gamingObj = GameObject.FindGameObjectsWithTag("GamingObj");
                foreach (var obj in gamingObj)
                {
                    obj.SetActive(false);
                }

                // 開始SEのフラグ下げる
                firstStart = false;

                // 終了SE
                gameAudio.PlayOneShot(gameFinishSE);

                //スコア画面BGM処理
                if (gameAudio.clip != ScoreBGM)
                {
                    gameAudio.clip = ScoreBGM;
                    gameAudio.Play();
                }

                // スコア表示UI処理
                StartCoroutine(uiManager.GameFinishAnim());

                isGaming = false;
                BoidsAreAllDead = false;
            }
        }
        // ゲーム中でない
        else
        {
            // Simulationはゲーム終了後も動いてほしいので非アクティブはゲーム開始前のみにする
            if (beforeStart) simulation.gameObject.SetActive(false);
            manipulateController.gameObject.SetActive(false);
        }

        // ゲーム前のルール見ますかの時からStartボタン押すまでの間
        if (beforeStart && gameAudio.clip != StartInstructionBGM)
        {
            gameAudio.clip = StartInstructionBGM;
            gameAudio.Play();
        }
    }
    public void ReloadScene()
    {
        // もう一度ルール見ますかのシーンへ
        startInstruction.gameObject.SetActive(true);
        startInstruction.Start();

        // フラグを立てることでリロード後スライドを見ている最中にBoidが泳ぎだすのを防ぐ
        beforeStart = true;
        foreach (Transform boidT in simulation.transform)
        {
            simulation.RemoveBoid(boidT.GetComponent<Boid.OOP.Boid>());
        }
        // Boidが全部消えてもReloadのときはフラグを立てない(スタート後すぐに終わってしまう)
        BoidsAreAllDead = false;

        try
        {
            // 障害物があったら消す
            simulation.Obstacles[0].gameObject?.SetActive(false);
            // 餌パーティクルが残ってたら消す
            manipulateController.TargetPosParticleInstantiated.gameObject.SetActive(false);
        }
        catch (System.Exception)
        {
            Debug.Log("E");
        }

        // 進行中のコルーチンの停止
        StopAllCoroutines();

        // ScoreUIの非表示とバーの値の初期化
        uiManager.Awake();

        // simulation paramの初期化
        param.Reset();

        // GameTimeの初期化処理とフラグ
        Start();
    }
}

