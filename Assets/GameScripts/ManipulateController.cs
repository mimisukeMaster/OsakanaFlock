using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MediaPipe.HandPose;
using Boid.OOP;


public class ManipulateController : MonoBehaviour
{
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
    [SerializeField]
    Camera cam;
    [SerializeField]
    AudioSource gameAudio;
    [SerializeField]
    AudioClip SwipeSE;
    [SerializeField]
    AudioClip FeedSE;
    [HideInInspector]
    public ParticleSystem TargetPosParticleInstantiated;
    public ParticleSystem PowerfulTargetParticle;

    bool actionReadyFlag;
    bool isMovingObstacle;
    public float chargeTime = 0;
    public float chargeTimeMax = 10;
    float sensitiveRotate = 5.0f;
    float lookObsDistance = 12f;
    float nonVisibleDistance = 13f;
    float lookTargetDistance = 15f;
    float startTime;
    float obsSpeed;
    Vector3 spawnPos;
    Vector3 disappearPos;
    GameObject Obstacle;


    // Start is called before the first frame update
    void Start()
    {
        Obstacle = simulation.Obstacles[0].gameObject;

        // 障害物は未だ非表示
        Obstacle.SetActive(false);

        obsSpeed = 5f;  // 3~10

    }

    // Update is called once per frame
    void Update()
    {
        // 秒計算
        chargeTime += Time.deltaTime;

        // Action可能フラグを立てる 満タンだよ示す処理UI→ UIManager
        if (chargeTime >= chargeTimeMax) actionReadyFlag = true;

        // マウスクリックされ続け、Action可能なら処理をする
        if (Input.GetMouseButton(0) && actionReadyFlag)
        {

            // クリック中ジェスチャ判定することで待機が再現できる(Finish, Point以外ははじくから)
            // ここからよびだすとよびだしにいったときにはもうFeedじゃない辺帝になってしまう時があるので
            //むこうでFeed出た時のこっちを呼ぶまたはフラグを立てるようにする？
            switch (classfication.inferenceResult)
            {

                // Obstacle
                case "Swipe":

                    //ローカル軸に沿った、奥行き・start-end距離のベクトル
                    Vector3 camVectorForward = cam.transform.forward * lookObsDistance;
                    Vector3 camVectorRight = cam.transform.right * nonVisibleDistance;

                    // 生成座標
                    spawnPos = camVectorForward - camVectorRight;

                    // 消える座標
                    disappearPos = camVectorForward + camVectorRight;

                    // 生成する座標にセット
                    Obstacle.transform.position = spawnPos;

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


                // Feed
                case "Feed":

                    /// 餌やりフラグ立てる → <seealso cref="Simulation.Update()"/>で処理
                    simulation.isFeeded = true;

                    /// Boidsが向かう座標をカメラの中央に設定する → <seealso cref="Simulation.SetBoidTargetPos(Vector3)"/>で処理
                    ///  障害物と同じカメラからの距離
                    Vector3 targetPos = cam.transform.forward * lookTargetDistance;
                    simulation.SetBoidTargetPos(targetPos);

                    Debug.DrawLine(cam.transform.position, cam.transform.forward * lookTargetDistance, Color.red);

                    // 集まる座標のところにParticle出してしばらくして消す
                    targetPos.y += 2;
                    TargetPosParticleInstantiated = Instantiate(PowerfulTargetParticle, targetPos, Quaternion.identity);
                    TargetPosParticleInstantiated.Play();
                    Destroy(TargetPosParticleInstantiated.gameObject, param.DurationPowerful);
                    if (!gameManager.isGaming) Destroy(TargetPosParticleInstantiated.gameObject);

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
            // 具現化
            if (!Obstacle.gameObject.activeInHierarchy) Obstacle.SetActive(true);

            //現在フレームの補間値を計算
            float interpolatedValue = (Time.time - startTime) / Vector3.Distance(spawnPos, disappearPos);

            // 線形補完で移動
            Obstacle.transform.position = Vector3.Lerp(spawnPos, disappearPos, interpolatedValue * obsSpeed);

            // 進行方向を向ける、-zが前になっているので * -1
            Obstacle.transform.forward = (disappearPos - spawnPos).normalized * -1;

            // 移動終了時
            if (Obstacle.transform.position == disappearPos)
            {
                // フラグ下げる
                isMovingObstacle = false;

                Obstacle.SetActive(false);
            }

        }

        // ゲーム終了していたらこのManipulaterも非アクティブになる　餌パーティクル削除・障害物非表示(シーンオブジェクトのため削除×)
        if (!gameManager.isGaming)
        {
            Destroy(PowerfulTargetParticle);
        }
    }
}
