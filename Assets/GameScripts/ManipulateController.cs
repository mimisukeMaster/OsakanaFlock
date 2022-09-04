using UnityEngine;
using MediaPipe.HandPose;
using Boid.OOP;


public class ManipulateController : MonoBehaviour
{
    [SerializeField]
    Camera cam;
    [SerializeField]
    ParticleSystem PowerfulTargetParticle;
    [HideInInspector]
    public ParticleSystem PowerfulTargetPosParticleInstantiated;

    [Header("Audio")]
    [SerializeField]
    AudioSource gameAudio;
    [SerializeField]
    AudioClip SwipeSE;
    [SerializeField]
    AudioClip FeedSE;

    [Header("Public refernces")]
    [SerializeField]
    Classification classfication;
    [SerializeField]
    Simulation simulation;
    [SerializeField]
    Boid.Param param;
    [SerializeField]
    UIManager uiManager;
    [SerializeField]
    GameManager gameManager;

    /// <summary>
    /// 命令ゲージの値
    /// </summary>
    public float chargeTime = 0;
    /// <summary>
    /// 命令ゲージの満タンの値
    /// </summary>
    public float chargeTimeMax = 10;

    /// <summary>
    /// 状況フラグ：命令が発動できる状態かどうか
    /// </summary>
    bool actionReadyFlag;

    /// <summary>
    /// 状況フラグ：障害物が動いているかどうか
    /// </summary>
    bool isMovingObstacle;

    /// <summary>
    /// 障害物の動くスピード
    /// </summary>
    float obsSpeed;

    float lookObsDistance = 12f;
    float nonVisibleDistance = 13f;
    float lookTargetDistance = 15f;
    float startTime;

    Vector3 spawnPos;
    Vector3 disappearPos;



    void Start()
    {
        // 障害物は未だ非表示
        simulation.Obstacle.SetActive(false);

        // 障害物が動く速度指定
        obsSpeed = 5f;
    }


    void Update()
    {
        // 秒計算
        chargeTime += Time.deltaTime;

        // Action可能フラグを立てる 満タンだよ示す処理UI→ UIManager
        if (chargeTime >= chargeTimeMax) actionReadyFlag = true;

        // マウスクリック長押下中かつAction可能なら処理をする
        if (Input.GetMouseButton(0) && actionReadyFlag)
        {
            // 手の形状から推論して得られたジェスチャーごとに処理分け
            switch (classfication.inferenceResult)
            {
                // 「食われるぞ！」の命令 / Obstacle
                case "Swipe":

                    //ローカル軸に沿った、奥行き・start-end距離のベクトル
                    Vector3 camVectorForward = cam.transform.forward * lookObsDistance;
                    Vector3 camVectorRight = cam.transform.right * nonVisibleDistance;

                    // 生成座標
                    spawnPos = camVectorForward - camVectorRight;

                    // 消える座標
                    disappearPos = camVectorForward + camVectorRight;

                    // 生成する座標にセット
                    simulation.Obstacle.transform.position = spawnPos;

                    // 移動開始のフラグ立てる →移動開始
                    isMovingObstacle = true;

                    // スタート時間をキャッシュ(Vector3.Lerpで移動するときに使うため)
                    startTime = Time.time;

                    // 命令したことのUI処理
                    StartCoroutine(uiManager.FlockGestureInvoked());

                    // SE
                    gameAudio.PlayOneShot(SwipeSE);

                    // 発動したのでリセット
                    chargeTime = 0;
                    actionReadyFlag = false;
                    break;


                // 「餌食え！」の命令 / Feed
                case "Feed":

                    /// 餌やりフラグ立てる → <seealso cref="Simulation.Update()"/>で処理
                    simulation.isFeeded = true;

                    // Boidsが向かう餌のある座標をカメラの中央に設定する 
                    Vector3 targetPos = cam.transform.forward * lookTargetDistance;

                    // 個々のBoidに教える
                    simulation.SetBoidTargetPos(targetPos);

                    // 集まる座標のところにParticleを出現させ、再生、一定時間後削除
                    targetPos.y += 2;
                    PowerfulTargetPosParticleInstantiated =
                                    Instantiate(PowerfulTargetParticle, targetPos, Quaternion.identity);

                    PowerfulTargetPosParticleInstantiated.Play();

                    Destroy(PowerfulTargetPosParticleInstantiated.gameObject, param.DurationPowerful);
                    if (!gameManager.isGaming) Destroy(PowerfulTargetPosParticleInstantiated.gameObject);

                    // 命令したことのUI表示の処理
                    StartCoroutine(uiManager.PowerfulGestureInvoked());

                    // SE
                    gameAudio.PlayOneShot(FeedSE);

                    // 発動したのでリセット
                    chargeTime = 0;
                    actionReadyFlag = false;
                    break;

                // Others
                default:
                    break;
            }

        }

        // 移動処理
        if (isMovingObstacle)
        {
            // アクティブ化
            if (!simulation.Obstacle.activeInHierarchy) simulation.Obstacle.SetActive(true);

            //現在フレームの補間値を計算
            float interpolatedValue = (Time.time - startTime) / Vector3.Distance(spawnPos, disappearPos);

            // 線形補完で移動
            simulation.Obstacle.transform.position = Vector3.Lerp(spawnPos, disappearPos, interpolatedValue * obsSpeed);

            // 進行方向を向ける、-zが前になっているので * -1
            simulation.Obstacle.transform.forward = (disappearPos - spawnPos).normalized * -1;

            // 移動終了時
            if (simulation.Obstacle.transform.position == disappearPos)
            {
                // フラグ下げる
                isMovingObstacle = false;

                simulation.Obstacle.SetActive(false);
            }

        }

        // // ゲーム終了していたらこのManipulatorも非アクティブになる　餌パーティクル削除・障害物非表示(シーンオブジェクトのため削除×)
        // if (!gameManager.isGaming)
        // {
        //     Destroy(PowerfulTargetParticle);
        // }
    }
}
