using UnityEngine;
using Boid.OOP;
using Boid;

public class GameManager : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField]
    AudioSource gameAudio;
    [SerializeField]
    AudioClip StartInstructionBGM;
    [SerializeField]
    AudioClip GameBGM;
    [SerializeField]
    AudioClip ScoreBGM;
    [SerializeField]
    AudioClip gameStartSE;
    [SerializeField]
    AudioClip gameFinishSE;

    [Header("Public references")]
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

    /// <summary>
    /// 時間制御：1プレイの時間
    /// </summary>
    public float GameTime = 100;

    /// <summary>
    /// 時間制御：プレイ中の残り時間
    /// </summary>
    public float GameRemainTime;

    /// <summary>
    /// "GamingObj"タグのオブジェクトを格納する配列
    /// </summary>
    public GameObject[] gamingObj;

    /// <summary>
    /// 場面フラグ：ゲーム開始前かどうか
    /// </summary>
    public bool beforeStart;

    /// <summary>
    /// 場面フラグ：ゲーム中かどうか<br></br>
    /// 初めてTrueになるのは<seealso cref="StartInstruction.GameStartButtonDown()"/>
    /// </summary>
    public bool isGaming;

    /// <summary>
    /// 場面フラグ：ゲーム開始後最初のフレームかどうか(firstStart ⊂ <seealso cref="isGaming"/>)
    /// </summary>
    public bool firstStart;

    /// <summary>
    /// 状況フラグ：Boidがシーンからすべて消えたかどうか
    /// </summary>
    public bool BoidsAreAllDead;


    void Awake()
    {
        /// GamingObjタグ付きのものを配列に格納
        /// <seealso cref="UIManager.BoidViewButtonBackDown()"/>で使う

        gamingObj = GameObject.FindGameObjectsWithTag("GamingObj");

        // gamingObj登録後BoidViewBackだけはまだ非表示
        uiManager.BoidViewBack.gameObject.SetActive(false);
    }

    void Start()
    {
        // 諸々初期化
        GameTime = 100f;
        GameRemainTime = GameTime;
        beforeStart = true;
        firstStart = true;
    }

    /// <summary>
    /// 場面フラグに応じて処理
    /// </summary>
    void Update()
    {
        /// ゲーム中
        /// スタートボタン<seealso cref="StartInstruction.GameStartButtonDown"/>押下時から
        /// GameRemainTime < 0 || BoidsAreAllDead まで
        if (isGaming)
        {
            // ゲーム開始前ではなくなった
            beforeStart = false;

            // 開始SE
            if (firstStart)
            {
                gameAudio.PlayOneShot(gameStartSE);
                firstStart = false;
            }
            // 残り時間減らしていく
            GameRemainTime -= Time.deltaTime;
            uiManager.RemainTimeText.text = "残り時間\n" + GameRemainTime.ToString("F1") + "秒";

            // 非アクティブだったものをアクティブ化
            simulation.gameObject.SetActive(true);
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
                /// ゲーム開始直後(Boids未生成時)にBoidsAreAllDeadが立ち
                /// ここが実行されて即終了になってしまうので、開始後1秒程度はフラグをさげてreturn
                if (GameTime - GameRemainTime < 1)
                {
                    BoidsAreAllDead = false;
                    return;
                }
                // deltatimeの誤差を埋めるため綺麗に0を入れておく
                //GameRemainTime = 0;

                // 終了SE
                gameAudio.PlayOneShot(gameFinishSE);

                // スコア画面BGM処理
                if (gameAudio.clip != ScoreBGM)
                {
                    gameAudio.clip = ScoreBGM;
                    gameAudio.Play();
                }

                // ゲーム中のObjectを消す処理
                //var gamingObj = GameObject.FindGameObjectsWithTag("GamingObj");
                foreach (var obj in gamingObj)
                {
                    obj.SetActive(false);
                }

                // スコア表示UI処理
                StartCoroutine(uiManager.GameFinishAnim());

                // ゲーム終了
                isGaming = false;
                //BoidsAreAllDead = false;
            }
        }

        /// ゲーム中でない時(リロードされてから
        /// スタートボタン<seealso cref="StartInstruction.GameStartButtonDown"/>押されるまで)
        else if (beforeStart)
        {
            // ゲーム開始前は非アクティブにする
            simulation.gameObject.SetActive(false);

            if (gameAudio.clip != StartInstructionBGM)
            {
                gameAudio.clip = StartInstructionBGM;
                gameAudio.Play();
            }
        }

        /// ゲーム中でない時(ゲーム終了からリロードされる<seealso cref="ReloadScene()"/>まで)
        else manipulateController.gameObject.SetActive(false);
    }


    public void ReloadScene()
    {
        // もう一度ルール見ますかのシーンへ
        startInstruction.gameObject.SetActive(true);
        startInstruction.Start();

        // ゲーム開始前
        beforeStart = true;


        // Boidが全部消えてもReloadのときはフラグを立てない(スタート後すぐに終わってしまう)
        //BoidsAreAllDead = false;

        /// Simulation(GameObject型)の子のもつ<seealso cref="Boid.OOP.Boid"/>を全て取得
        /// 親・子の関係が絡んでいるのでforeachの型をBoid.OOP.Boidにできない
        foreach (Transform boidT in simulation.transform)
        {
            simulation.RemoveBoid(boidT.GetComponent<Boid.OOP.Boid>());
        }

        // 命令による生成物削除
        // 元々無い時のNull回避のため try catch 構文
        try
        {
            // 障害物があったら消す
            simulation.Obstacle.SetActive(false);
            // 餌パーティクルが残ってたら消す
            manipulateController.PowerfulTargetPosParticleInstantiated.gameObject.SetActive(false);
        }
        catch (System.Exception)
        {
            // Debug.Log("E");
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

