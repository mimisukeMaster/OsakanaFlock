using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using Boid.OOP;

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
    Text RemainTimeText;

    public float GameTime = 100;
    public float GameRemainTime;
    public int ResultScore;
    public bool isGaming; /// 初めてTrueになるのは<seealso cref="StartInstruction.GameStartButtonDown()"</>
    public bool BoidsAreAllDead;
    public bool beforeStart;

    // Start is called before the first frame update
    void Start()
    {
        GameTime = 100f;
        GameRemainTime = GameTime;
        beforeStart = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isGaming)
        {
            // ゲーム中
            beforeStart = false;
            GameRemainTime -= Time.deltaTime;
            RemainTimeText.text = "残り時間\n" + GameRemainTime.ToString("F1") + "秒";

            simulation?.gameObject.SetActive(true);
            manipulateController.gameObject.SetActive(true);

            // ゲーム終了処理
            if (GameRemainTime < 0 || BoidsAreAllDead)
            {
                /// ゲーム開始直後(まだBoidいない時)にここが実行されて即終了になってしまうので、開始直後はスルー
                if (GameTime - GameRemainTime < 1) return;

                // deltatimeのござを埋めるため綺麗に0を入れておく
                GameRemainTime = 0;

                // ゲーム中のUIを消す処理
                uiManager.DisableGameObjects();

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

        // 進行中のコルーチンの停止
        StopAllCoroutines();

        // ScoreUIの非表示とバーの値の初期化
        uiManager.Awake();

        // simulation paramの初期化
        simulation.Awake();

        // GameTimeの初期化処理とフラグ
        Start();
    }
}

