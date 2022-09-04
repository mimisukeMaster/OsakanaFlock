using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Boid;
using Boid.OOP;
using DG.Tweening;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("ScoreGauge/Info")]
    public Slider ScoreGauge;
    public Text RemainTimeText;
    public Button BoidViewBack;
    [SerializeField]
    Text AliveBoidsText;

    [Header("ActionGauge/Cursor")]
    [SerializeField]
    Slider ActionGauge;
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

    [Header("Finish UIs")]
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
    Button RetryButton;
    [SerializeField]
    Animation SceneRoadTransition;

    [Header("Public references")]
    [SerializeField]
    Param param;
    [SerializeField]
    Simulation simulation;
    [SerializeField]
    ManipulateController manipulateController;
    [SerializeField]
    GameManager gameManager;
    [SerializeField]
    AudioSource gameAudio;
    [SerializeField]
    AudioClip scoreResultSE;
    [SerializeField]
    AudioClip normaClearSE;
    [SerializeField]
    AudioClip normaFailedSE;
    [SerializeField]
    AudioClip retrySE;

    /// <summary>
    /// スコア制御：ノルマスコアの値
    /// </summary>
    public float normaScore;

    /// <summary>
    /// スコア制御：MAXを1とした時のノルマスコアの割合
    /// </summary>
    float normaProportion;

    /// <summary>
    /// スコア制御：群れているかどうかを数値化
    /// </summary>
    float score_isFlocking;

    /// <summary>
    /// スコア制御：元気かどうかを数値化
    /// </summary>
    float score_isPowerful;

    /// <summary>
    /// 1秒間を測るタイマー
    /// </summary>
    float currentTime;

    /// <summary>
    /// Boids処理：ゲーム開始直後の最大Boid数
    /// </summary>
    int maxBoids;

    /// <summary>
    /// Boids処理：現在のBoid数
    /// </summary>
    int NumOfBoids;


    /// <summary>
    /// 状況フラグ：群れステータスが点灯中かどうか
    /// </summary>
    bool isFlockTrueAnimation;

    /// <summary>
    /// 状況フラグ：元気ステータスが点灯中かどうか
    /// </summary>
    bool isPowerTrueAnimation;

    /// <summary>
    /// 状況フラグ：おさかなビュー中かどうか
    /// </summary>
    bool boidViewMode;

    /// <summary>
    /// 「餌食え！」のテキストの色情報
    /// </summary>
    Color FlockInfoColor;

    /// <summary>
    /// 「食われるぞ！」のテキストの色情報
    /// </summary>
    Color PowerfulInfoColor;

    /// <summary>
    /// 初期化 リロード時 値が変わっているものを戻す
    /// </summary>
    public void Awake()
    {
        // 色(DOTween.DOFadeにより変化)
        PowerfulInvokedInfo.color = new Color(1, 1, 1, 0);
        FlockInvokedInfo.color = new Color(1, 1, 1, 0);
        FinishUIPanel.color = new Color(1, 1, 1, 0);

        // スコア表示のUI
        FinishUIPanel.gameObject.SetActive(false);
        ScoreText.gameObject.SetActive(false);
        NormaResultText.gameObject.SetActive(false);

        // リスタートのUI
        RetryButton.gameObject.SetActive(false);
        SceneRoadTransition.gameObject.SetActive(false);

        // 値の初期化
        ScoreGauge.value = 0;
        manipulateController.chargeTime = 0;
        //maxBoids = simulation.boidCount;


    }

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
        normaProportion = 0.7f;
        normaScore = ScoreGauge.maxValue * normaProportion;

        // UISpriteの (normaProportion * 100) % のところにカーソル描画
        NormaCursor.rectTransform.anchoredPosition = new Vector3(
            ScoreGauge.GetComponent<RectTransform>().sizeDelta.x * (normaProportion - 0.5f),
            NormaCursor.rectTransform.anchoredPosition.y, 0);
        NormaCursor.GetComponentInChildren<Text>().text = normaProportion * 100 + "点";
    }


    void Update()
    {
        // ゲーム中でなければ何もしない
        if (!gameManager.isGaming) return;

        // スコア計算
        score_isFlocking = param.isFlocking == true ? 1 : 0.5f;
        score_isPowerful = param.isPowerful == true ? 1 : 0.5f;
        NumOfBoids = simulation.boids_.Count;

        /// 現在のスコアを反映 一秒ごとに処理する
        /// <seealso cref="GameManager.GameTime"/> の値が最大のスコアになる計算
        currentTime += Time.deltaTime;
        if (currentTime > 1)
        {
            // 一度に追加される値の最大は 1 * 1 * 1 = 1
            ScoreGauge.value += score_isFlocking * score_isPowerful * (NumOfBoids / maxBoids);
            currentTime = 0;
        }

        // 命令ゲージの値
        ActionGauge.value = manipulateController.chargeTime;

        // 残りBoid数も表示
        AliveBoidsText.text = "お魚:" + NumOfBoids + "匹";

        // 文字色変更
        if (NumOfBoids < 100) AliveBoidsText.color = Color.red;

        // 元々赤で100匹以上いたら戻す
        else if (AliveBoidsText.color == Color.red) AliveBoidsText.color = Color.white;


        // 満タンならSpriteを変えてアニメーション
        if (ActionGauge.value == ActionGauge.maxValue)
        {
            StartCoroutine(ActionGaugeInfoAnim());
        }
        else
        {
            ActionGaugeContent.sprite = ActionGaugeImg;
            ActionGaugeImg_info.gameObject.SetActive(false);
        }

        // isFlocking(群れ), isPowerful(元気) の信号処理
        if (!isFlockTrueAnimation)
        {
            // isFlocking中なら
            if (score_isFlocking == 1) StartCoroutine(isFlockTrueAnim());
            else IsFlockingCursor.sprite = isFlockFalseImg;
        }

        if (!isPowerTrueAnimation)
        {
            // isPowerful中なら
            if (score_isPowerful == 1) StartCoroutine(isPowerfulTrueAnim());
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
        isPowerTrueAnimation = true;
        IsPowerfulCursor.sprite = isPowerfulTrueImg;
        yield return new WaitForSeconds(0.3f);
        isPowerTrueAnimation = false;
    }
    IEnumerator ActionGaugeInfoAnim()
    {
        // 満タンならゲージ色を変える
        ActionGaugeContent.sprite = ActionGaugeImg_max;

        // 非アクティブからアクティブにしアニメーション
        if (!ActionGaugeImg_info.gameObject.activeInHierarchy && !boidViewMode)
        {
            ActionGaugeImg_info.gameObject.SetActive(true);

            while (true)
            {
                ActionGaugeImg_info.DORotate(new Vector3(0, 0, 10), 1, RotateMode.Fast);
                yield return new WaitForSeconds(1.0f);
                ActionGaugeImg_info.DORotate(new Vector3(0, 0, -10), 1, RotateMode.Fast);
                yield return new WaitForSeconds(1.0f);
                if (ActionGauge.value != ActionGauge.maxValue) break;
            }
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

    /// <summary>
    /// <seealso cref="GameManager.Update()"/>からゲーム終了後呼ばれる
    /// </summary>
    public IEnumerator GameFinishAnim()
    {
        //表示中の場合は消す
        //ActionGaugeImg_info.gameObject.SetActive(false);

        FinishUIPanel.gameObject.SetActive(true);
        FinishUIPanel.DOFade(0.6f, 2);

        yield return new WaitForSeconds(2f);

        // ノルマスコア達成ならスコアの数字をデコって達成表示
        ScoreText.canvas.sortingOrder = 2;

        if (ScoreGauge.value >= normaScore)
        {
            ScoreText.gameObject.SetActive(true);
            ScoreText.text = ScoreGauge.value.ToString("F0");
            ScoreText.color = Color.yellow;
            NormaAchievedParticle.Play();
            gameAudio.PlayOneShot(scoreResultSE);

            yield return new WaitForSeconds(1f);
            NormaResultText.gameObject.SetActive(true);
            NormaResultText.text = "ノルマクリア!";
            NormaResultText.colorGradientPreset = NormaClearSuccess;
            gameAudio.PlayOneShot(normaClearSE);

        }
        else
        {
            ScoreText.gameObject.SetActive(true);
            ScoreText.text = ScoreGauge.value.ToString("F0");
            ScoreText.color = Color.white;
            gameAudio.PlayOneShot(scoreResultSE);

            yield return new WaitForSeconds(1f);
            NormaResultText.gameObject.SetActive(true);
            NormaResultText.text = "ノルマ失敗";
            NormaResultText.colorGradientPreset = NormaClearFailed;
            gameAudio.PlayOneShot(normaFailedSE);
        }
        yield return new WaitForSeconds(1.5f);
        RetryButton.gameObject.SetActive(true);

    }

    /// <summary> 
    /// おさかなビューボタンで鑑賞モード
    /// </summary>
    public void BoidViewButtonDown()
    {
        // 視点となるお魚がいなければreturn
        if (simulation.boids_.Count == 0) return;

        // おさかなビュー発動中
        boidViewMode = true;

        // listのうち0番目のお魚は上のreturnを回避したなら確実にいるので、その視点に移る
        Camera.main.transform.position = simulation.boids_[0].transform.position;
        Camera.main.transform.SetParent(simulation.boids_[0].transform);
        Camera.main.transform.localEulerAngles = Vector3.zero;

        // gamingObjタグのものは非アクティブ化
        foreach (var obj in gameManager.gamingObj)
        {
            obj.SetActive(false);
        }

        // 戻るボタンをアクティブに
        BoidViewBack.gameObject.SetActive(true);
    }

    /// <summary>
    /// お魚ビューから戻るボタン押下時
    /// </summary>
    public void BoidViewButtonBackDown()
    {
        // おさかなビュー終了
        boidViewMode = false;

        //元の座標へ
        Camera.main.transform.parent = null;
        Camera.main.transform.position = Vector3.zero;
        Camera.main.transform.eulerAngles = Vector3.zero;

        // gamingObjタグのものをアクティブ化
        foreach (var obj in gameManager.gamingObj)
        {
            obj.SetActive(true);
        }

        // 戻るボタンを非アクティブに
        BoidViewBack.gameObject.SetActive(false);

    }


    // リトライボタンでもう一回
    public void RetryButtonDown() => StartCoroutine(RetryUIAnim());
    IEnumerator RetryUIAnim()
    {
        gameAudio.PlayOneShot(retrySE);
        // 再生中のノルマ達成パーティクルを停止
        NormaAchievedParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        // シーン遷移アニメーション
        SceneRoadTransition.gameObject.SetActive(true);
        SceneRoadTransition.Play();

        yield return new WaitForSeconds(2);
        gameManager.ReloadScene();
    }
}