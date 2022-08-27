using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Boid;
using Boid.OOP;
using DG.Tweening;
using TMPro;
using MediaPipe.HandPose;

public class UIManager : MonoBehaviour
{
    [Header("Canvas")]
    [SerializeField]
    RectTransform canvas;
    [SerializeField]
    Camera uiCamera;

    [Header("Gauge/Time")]
    public Slider ScoreGauge;
    [SerializeField]
    Slider ActionGauge;
    /// RemainTimeTextはゲームの時間管理なので<seealso cref="GameManager.RemainTimeText"/>へ

    [Header("image attach to")]
    [SerializeField]
    Sprite ActionGaugeImg;
    [SerializeField]
    Sprite ActionGaugeImg_max;
    [SerializeField]
    RectTransform ActionGaugeImg_info;
    [SerializeField]
    Image NormaCursor;

    [Header("gauge images")]
    [SerializeField]
    Image ActionGaugeContent;

    [SerializeField]
    Image IsFlockingCursor;
    [SerializeField]
    Image IsPowerfulCursor;

    [Header("flock UI images")]
    [SerializeField]
    Sprite isFlockTrueImg_1;
    [SerializeField]
    Sprite isFlockTrueImg_2;
    [SerializeField]
    Sprite isFlockFalseImg;
    [SerializeField]
    TextMeshProUGUI FlockInvokedInfo;
    [SerializeField]
    ParticleSystem FlockInvokedParticle;

    [Header("powerful UI images")]
    [SerializeField]
    Sprite isPowerfulTrueImg;
    [SerializeField]
    Sprite isPowerfulFalseImg;

    [SerializeField]
    TextMeshProUGUI PowerfulInvokedInfo;
    [SerializeField]
    ParticleSystem PowrfulInvokedParticle;

    [Header("Finish後のUI")]
    [SerializeField]
    Image FinishUIPanel;
    [SerializeField]
    TextMeshProUGUI ScoreText;
    [SerializeField]
    ParticleSystem NormaAchievedParticle;
    [SerializeField]
    TextMeshProUGUI NormaResultText;
    [SerializeField]
    TMP_ColorGradient NormaClearSuccess;
    [SerializeField]
    TMP_ColorGradient NormaClearFailed;
    [SerializeField]
    Button RestartButton;
    [SerializeField]
    Animation SceneRoadTransition;

    [Space()]
    [SerializeField]
    Param param;
    [SerializeField]
    Simulation simulation;
    [SerializeField]
    ManipulateController manipulateController;
    [SerializeField]
    HandAnimator handAnimator;
    [SerializeField]
    GameManager gameManager;


    float score_isFlocking;
    float score_isPowerful;
    float currentTime;
    float maxBoids;
    float NumOfBoids;
    public float normaScore;
    float normaProportion;
    Color FlockInfoColor;
    Color PowerfulInfoColor;
    Color ClassificationResultColor;

    bool isFlockTrueAnimation;
    bool isPowerTrueAnimation;



    public void Awake()
    {
        // 初期化 Startの前に呼ばれる、リロードするとき値が変わっているものを戻す
        PowerfulInvokedInfo.color = new Color(1, 1, 1, 0);
        FlockInvokedInfo.color = new Color(1, 1, 1, 0);

        FinishUIPanel.color = new Color(1, 1, 1, 0);
        FinishUIPanel.gameObject.SetActive(false);
        ScoreText.gameObject.SetActive(false);
        NormaResultText.gameObject.SetActive(false);

        RestartButton.gameObject.SetActive(false);
        SceneRoadTransition.gameObject.SetActive(false);

        ScoreGauge.value = 0;
        manipulateController.chargeTime = 0;
        maxBoids = simulation.boidCount;


    }
    // Start is called before the first frame update
    void Start()
    {
        // UIバーのMAX時を設定する
        ScoreGauge.maxValue = gameManager.GameTime;
        ActionGauge.maxValue = manipulateController.chargeTimeMax;

        // 値参照
        PowerfulInfoColor = PowerfulInvokedInfo.color;
        FlockInfoColor = FlockInvokedInfo.color;
        maxBoids = simulation.boidCount;

        // ノルマの設定
        normaProportion = 0.8f;
        normaScore = ScoreGauge.maxValue * normaProportion;

        // UISpriteの８割のところにカーソル描画
        NormaCursor.rectTransform.anchoredPosition = new Vector3(
            ScoreGauge.GetComponent<RectTransform>().sizeDelta.x * (normaProportion - 0.5f),
            NormaCursor.rectTransform.anchoredPosition.y, 0);
    }

    // Update is called once per frame
    void Update()
    {
        // スコア計算　他の処理が入ったらCaculateScore()つくる
        score_isFlocking = param.isFlocking == true ? 1 : 0.5f;
        score_isPowerful = param.isPowerful == true ? 1 : 0.5f;
        NumOfBoids = simulation.GetNowAliveBoids();
        // 現在のスコアを反映 一秒ごとに処理する
        currentTime += Time.deltaTime;
        if (currentTime > 1)
        {
            ScoreGauge.value += score_isFlocking * score_isPowerful * (NumOfBoids / maxBoids);
            currentTime = 0;
        }
        // 命令ゲージ計算
        ActionGauge.value = manipulateController.chargeTime;

        // 満タンならSpriteを変えてアニメーション
        if (ActionGauge.value == ActionGauge.maxValue && gameManager.GameRemainTime > 0)
        {
            StartCoroutine(ActionGaugeInfoAnim());
        }
        else
        {
            ActionGaugeContent.sprite = ActionGaugeImg;
            ActionGaugeImg_info.gameObject.SetActive(false);
        }

        // isFlocking, isPowerful の信号処理
        if (!isFlockTrueAnimation)
        {
            // isFlocking中なら
            if (score_isFlocking == 1 && !isFlockTrueAnimation)
            {
                StartCoroutine(isFlockTrueAnim());
            }
            else IsFlockingCursor.sprite = isFlockFalseImg;
        }

        if (!isPowerTrueAnimation)
        {
            // isPowerful中なら
            if (score_isPowerful == 1)
            {
                StartCoroutine(isPowerfulTrueAnim());
            }
            else IsPowerfulCursor.sprite = isPowerfulFalseImg;
        }


    }

    IEnumerator isFlockTrueAnim()
    {
        isFlockTrueAnimation = true;
        IsFlockingCursor.sprite = isFlockTrueImg_1;
        yield return new WaitForSeconds(0.3f);
        IsFlockingCursor.sprite = isFlockTrueImg_2;
        yield return new WaitForSeconds(0.3f);
        isFlockTrueAnimation = false;
    }
    IEnumerator isPowerfulTrueAnim()
    {
        IsPowerfulCursor.sprite = isPowerfulTrueImg;
        yield return new WaitForSeconds(Time.deltaTime);
    }
    IEnumerator ActionGaugeInfoAnim()
    {
        ActionGaugeContent.sprite = ActionGaugeImg_max;
        if (gameManager.isGaming) ActionGaugeImg_info.gameObject.SetActive(true);
        while (true)
        {
            ActionGaugeImg_info.DORotate(new Vector3(0, 0, 20), 1, RotateMode.Fast);
            yield return new WaitForSeconds(1.0f);
            ActionGaugeImg_info.DORotate(new Vector3(0, 0, -20), 1, RotateMode.Fast);
            yield return new WaitForSeconds(1.0f);
            if (ActionGauge.value != ActionGauge.maxValue || gameManager.isGaming) break;
        }
    }
    public IEnumerator FlockGestureInvoked()
    {
        // Textの表示
        while (FlockInfoColor.a < 1)
        {
            FlockInfoColor.a += Time.deltaTime * 4;
            FlockInvokedInfo.color = FlockInfoColor;
            yield return new WaitForEndOfFrame();
        }
        // particle 演出
        FlockInvokedParticle.Play();
        yield return new WaitForSeconds(0.8f);

        // TextのFade
        while (FlockInfoColor.a > 0)
        {
            FlockInfoColor.a -= Time.deltaTime * 4;
            FlockInvokedInfo.color = FlockInfoColor;
            yield return new WaitForEndOfFrame();
        }
    }
    public IEnumerator PowerfulGestureInvoked()
    {
        // Textの表示
        while (PowerfulInfoColor.a < 1)
        {
            PowerfulInfoColor.a += Time.deltaTime * 4;
            PowerfulInvokedInfo.color = PowerfulInfoColor;
            yield return new WaitForEndOfFrame();
        }
        // particle 演出
        PowrfulInvokedParticle.Play();
        yield return new WaitForSeconds(0.8f);

        // TextのFade
        while (PowerfulInfoColor.a > 0)
        {
            PowerfulInfoColor.a -= Time.deltaTime * 4;
            PowerfulInvokedInfo.color = PowerfulInfoColor;
            yield return new WaitForEndOfFrame();
        }
    }
    public void DisableGameObjects()
    {
        var gamingObj = GameObject.FindGameObjectsWithTag("GamingObj");
        foreach (var obj in gamingObj)
        {
            obj.SetActive(false);
        }
    }

    /// <seealso cref="GameManager.Update()"/>からゲーム終了後呼ばれる
    public IEnumerator GameFinishAnim()
    {
        //表示中の場合は消す
        ActionGaugeImg_info.gameObject.SetActive(false);

        FinishUIPanel.gameObject.SetActive(true);
        FinishUIPanel.DOFade(0.6f, 2);
        yield return new WaitForSeconds(3f);

        // ノルマスコア達成ならスコアの数字をデコって達成表示
        ScoreText.canvas.sortingOrder = 2;
        if (ScoreGauge.value >= normaScore)
        {
            ScoreText.gameObject.SetActive(true);
            ScoreText.text = ScoreGauge.value.ToString("F0");
            ScoreText.color = Color.yellow;
            NormaAchievedParticle.Play();

            yield return new WaitForSeconds(1f);
            NormaResultText.gameObject.SetActive(true);
            NormaResultText.text = "ノルマクリア!";
            NormaResultText.colorGradientPreset = NormaClearSuccess;

        }
        else
        {
            ScoreText.gameObject.SetActive(true);
            ScoreText.text = ScoreGauge.value.ToString("F0");

            yield return new WaitForSeconds(1f);
            NormaResultText.gameObject.SetActive(true);
            NormaResultText.text = "ノルマ失敗";
            NormaResultText.colorGradientPreset = NormaClearFailed;
        }
        yield return new WaitForSeconds(1.5f);
        RestartButton.gameObject.SetActive(true);

    }

    // リトライボタンでもう一回
    public void RetryButtonDown() => StartCoroutine(RetryUIAnim());
    IEnumerator RetryUIAnim()
    {
        // 再生中のノルマ達成パーティクルを停止
        NormaAchievedParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        SceneRoadTransition.gameObject.SetActive(true);
        SceneRoadTransition.Play();
        yield return new WaitForSeconds(2);
        gameManager.ReloadScene();
    }
}