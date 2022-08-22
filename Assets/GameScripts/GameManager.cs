using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    Image FinishUIPanel;
    public float GameTime;
    public float GameRemainTime;
    public bool isGameStarted;
    public bool BoidsAreAllDead;

    // Start is called before the first frame update
    void Start()
    {
        GameRemainTime = GameTime;
        FinishUIPanel.color = new Color(1, 1, 1, 0);
        StartCoroutine(GameFinishAnim());
    }

    // Update is called once per frame
    void Update()
    {

        if (isGameStarted) GameRemainTime -= Time.deltaTime;

        if (GameRemainTime < 0) StartCoroutine(GameFinishAnim());

    }
    IEnumerator GameFinishAnim()
    {
        FinishUIPanel.DOFade(0.6f, 2);
        yield return new WaitForSeconds(1f);

        ///　ぽん、ポン、ぽｎ　score表示
    }
}

