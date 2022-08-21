using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Boid;
using Boid.OOP;

public class UIManager : MonoBehaviour
{
    // UI objects
    [SerializeField]
    Slider UIGauge;
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

    //powerful UI images
    [SerializeField]
    Sprite isPowerfulTrueImg;
    [SerializeField]
    Sprite isPowerfulFalseImg;


    [SerializeField]
    Param param;
    [SerializeField]
    Simulation simulation;

    float sc_isFlocking;
    float sc_isPowerful;
    float NumOfBoids;

    bool isFlockTrueAnimation;
    bool isPowerTrueAnimation;

    // Start is called before the first frame update
    void Start()
    {
        // UIバーのMAX時を設定する
        UIGauge.maxValue = 1 * 1 * simulation.boidCount;
    }

    // Update is called once per frame
    void Update()
    {
        // スコア計算　他の処理が入ったらCaculateScore()つくる
        sc_isFlocking = param.isFlocking == true ? 1 : 0.5f;
        sc_isPowerful = param.isPoweful == true ? 1 : 0.5f;
        NumOfBoids = simulation.GetNowAliveBoids();

        // 現在のスコアを反映
        UIGauge.value = sc_isFlocking * sc_isPowerful * NumOfBoids;

        // isFlocking, isPowerful の信号処理
        if (!isFlockTrueAnimation)
        {
            // isFlocking中なら
            if (sc_isFlocking == 1 && !isFlockTrueAnimation)
            {
                StartCoroutine(isFlockTrueAnim());
            }
            else IsFlockingCursor.sprite = isFlockFalseImg;
        }

        if (!isPowerTrueAnimation)
        {
            // isPowerful中なら
            if (sc_isPowerful == 1)
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
}