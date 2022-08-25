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
    UIManager uiManager;


    public float GameTime;
    public float GameRemainTime;
    public int ResultScore;
    public bool isGaming;
    public bool BoidsAreAllDead;

    // Start is called before the first frame update
    void Start()
    {
        GameTime = 40f;
        isGaming = true;                                /// 臨時にここに記す、本当はスタート処理の後

        GameRemainTime = GameTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (isGaming) GameRemainTime -= Time.deltaTime;

        // ゲーム終了処理
        if (isGaming && (GameRemainTime < 0 || BoidsAreAllDead))
        {
            // ゲーム中のUIを消す処理
            uiManager.DisableGameObjects();

            StartCoroutine(uiManager.GameFinishAnim());
            isGaming = false;
        }
    }
}

