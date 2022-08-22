using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Boid;
using Boid.OOP;
using DG.Tweening;
using TMPro;
using TMPro.SpriteAssetUtilities;

public class UIManager : MonoBehaviour
{
    // gauge
    [SerializeField]
    Slider ScoreGauge;
    [SerializeField]
    Slider ActionGauge;

    // image attach to
    [SerializeField]
    Sprite ActionGaugeImg;
    [SerializeField]
    Sprite ActionGaugeImg_max;
    [SerializeField]
    RectTransform ActionGaugeImg_info;

    // gauge images
    [SerializeField]
    Image ActionGaugeContent;

    [SerializeField]
    Image IsFlockingCursor;
    [SerializeField]
    Image IsPowerfulCursor;

    // flock UI images
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

    //powerful UI images
    [SerializeField]
    Sprite isPowerfulTrueImg;
    [SerializeField]
    Sprite isPowerfulFalseImg;

    [SerializeField]
    TextMeshProUGUI PowerfulInvokedInfo;
    [SerializeField]
    ParticleSystem PowrfulInvokedParticle;



    [SerializeField]
    Param param;
    [SerializeField]
    Simulation simulation;
    [SerializeField]
    ManipulateController manipulateController;
    [SerializeField]
    GameManager gameManager;
    float score_isFlocking;
    float score_isPowerful;
    float currentTime;
    float maxBoids;
    float NumOfBoids;
    Color FlockInfoColor;
    Color PowerfulInfoColor;

    bool isFlockTrueAnimation;
    bool isPowerTrueAnimation;

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
        if (ActionGauge.value == ActionGauge.maxValue) StartCoroutine(ActionGaugeInfoAnim());
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
        ActionGaugeImg_info.gameObject.SetActive(true);

        while (true)
        {
            ActionGaugeImg_info.DORotate(new Vector3(0, 0, 20), 1, RotateMode.Fast);
            yield return new WaitForSeconds(1.0f);
            ActionGaugeImg_info.DORotate(new Vector3(0, 0, -20), 1, RotateMode.Fast);
            yield return new WaitForSeconds(1.0f);
            if (ActionGauge.value != ActionGauge.maxValue) break;
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
}