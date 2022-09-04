using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;

public class TitleSceneManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField]
    TextMeshProUGUI nextText;

    [Header("Audio")]
    [SerializeField]
    AudioSource titleAudio;
    [SerializeField]
    AudioClip ClickSE;


    void Update()
    {
        // UI Textのフェードインアウト繰り返し
        if (nextText.alpha >= 0.95f) nextText.DOFade(0, 1.5f);
        if (nextText.alpha <= 0.05f) nextText.DOFade(1, 1.5f);

        // 押下時ロード
        if (Input.GetMouseButtonDown(0))
        {
            nextText.text = "ロード中…";
            titleAudio.PlayOneShot(ClickSE);
            SceneManager.LoadScene("FlockingScene");
        }
    }
}