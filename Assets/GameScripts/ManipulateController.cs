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
    Boid.OOP.Boid boid;
    [SerializeField]
    Camera cam;

    bool actionReadyFlag;
    bool isMovingObstacle;
    public float chargeTime = 0;
    public float chargeTimeMax = 10;
    float sensitiveRotate = 5.0f;
    float lookObsDistance = 12f;
    float nonVisibleDistance = 13f;
    float lookTargetDistance = 15f;
    float startTime;
    float obsSpeed = 100f;
    Vector3 spawnPos;
    Vector3 disappearPos;
    Vector3 obsVelocity = Vector3.zero;
    GameObject Obstacle;


    // Start is called before the first frame update
    void Start()
    {
        Obstacle = simulation.Obstacles[0].gameObject;

        // 障害物は未だ非表示
        Obstacle.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        // 秒計算
        chargeTime += Time.deltaTime;

        // Action可能フラグを立てる 満タンだよ示す処理UI→ UIManager
        if (chargeTime >= chargeTimeMax) actionReadyFlag = true;

        


        // マウスクリックされ続け、Action可能なら
        if (Input.GetMouseButton(0) && actionReadyFlag)
        {
            // クリック中ジェスチャ判定することで待機が再現できる(Finish, Point以外ははじくから)
            switch (classfication.inferenceResult)
            {

                // Obstacle
                case "Finish":
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

                    // 発動したのでリセット
                    chargeTime = 0;
                    actionReadyFlag = false;
                    break;

                // Feed
                case "Point":
                    /// 餌やりフラグ立てる → <seealso cref="Simulation.Update()"/>で処理
                    simulation.isFeeded = true;

                    /// Boidsが向かう座標をカメラの中央に設定する → <seealso cref="Simulation.SetBoidTargetPos(Vector3)"/>で処理
                    ///  障害物と同じカメラからの距離
                    simulation.SetBoidTargetPos(cam.transform.forward * lookTargetDistance);
                    Debug.DrawLine(cam.transform.position, cam.transform.forward * lookTargetDistance, Color.red);
                    Debug.Log("point ges");
                    // 発動したのでリセット
                    chargeTime = 0;
                    actionReadyFlag = false;
                    break;

                // Others
                default:
                    break;

            }
        }
        Debug.DrawLine(cam.transform.position, spawnPos, Color.yellow);
        Debug.DrawLine(cam.transform.position, disappearPos, Color.green);

        // 移動処理
        if (isMovingObstacle)
        {
            // 具現化
            Obstacle.SetActive(true);

            //現在フレームの補間値を計算
            float interpolatedValue = (Time.time - startTime) / Vector3.Distance(spawnPos, disappearPos);

            // 線形補完で移動
            Obstacle.transform.position = Vector3.Lerp(spawnPos, disappearPos, interpolatedValue * obsSpeed);

            // 移動終了時
            if (Obstacle.transform.position == disappearPos)
            {
                // フラグ下げる
                isMovingObstacle = false;

                Obstacle.SetActive(false);
            }
        }
    }
}
