using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;
public class TitleSceneManager : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI nextText;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (nextText.alpha >= 0.95f) nextText.DOFade(0, 1.5f);
        if (nextText.alpha <= 0.05f) nextText.DOFade(1, 1.5f);

        if (Input.GetMouseButtonDown(0)) SceneManager.LoadScene("FlockingScene");


    }
}
