using UnityEngine;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine.SceneManagement;

namespace Boid.OOP
{

    public class Simulation : MonoBehaviour
    {
        [SerializeField]
        public int boidCount = 400;

        [SerializeField]
        GameObject boidPrefab;

        [SerializeField]
        Param param;
        public GameManager gameManager;

        public List<Boid> boids_ = new List<Boid>();
        public ReadOnlyCollection<Boid> boids
        {
            get { return boids_.AsReadOnly(); }
        }

        /// <summary>
        /// 場面フラグ：現在がタイトルシーンかどうか
        /// </summary>
        public bool isTitleNow;
        /// <summary>
        /// 状況フラグ：今餌与えているかどうか
        /// </summary>
        public bool isFeeded;
        //public bool isFirstGameFrame;
        //public bool isGameStarted;

        /// <summary>
        /// 「群れ」の時間制御タイマー
        /// </summary>
        public float timer_flocking = 0;

        /// <summary>
        /// 「元気」の時間制御タイマー
        /// </summary>
        public float timer_powerful = 0;

        /// <summary>
        /// 障害物を見つけたBoidsのカウンター
        /// </summary>
        public int detectedCount = 0;

        /// <summary>
        /// 障害物
        /// </summary>
        public GameObject Obstacle;

        /// <summary>
        /// Boidsが動ける範囲のバウンディングボックス
        /// </summary>
        public Vector3 ColliderSize;

        void Start()
        {
            ColliderSize = GetComponent<BoxCollider>().size;

            // 初期化
            param.Reset();

            // Particleメモリ制限
            ParticleSystem.SetMaximumPreMappedBufferCounts(1, 1);

            // 規定数まで生成
            while (boids_.Count < boidCount)
            {
                AddBoid();
            }
        }

        #region 個体制御
        void AddBoid()
        {
            var go = Instantiate(boidPrefab, Random.insideUnitSphere, Random.rotation);
            go.transform.SetParent(transform);
            var boid = go.GetComponent<Boid>();
            boid.simulation = this;
            boid.param = param;
            boids_.Add(boid);
        }

        /// <summary>
        /// HP 0 のBoidを消す
        /// </summary>
        /// <param name="rip_boid"></param>
        public void RemoveBoid(Boid rip_boid)
        {
            // 死ぬBoidにMainCameraがついていたら、次のインデックスのBoidに移ってもらう
            // GetComponentInChildren<Camera>() は処理が重い
            // 5の内訳：　[Koi.002, アーマチュア, DyingParticle, AteParticle, MainCamera]
            if (rip_boid.transform.childCount == 5)
            {
                try
                {
                    Camera.main.transform.SetParent(boids_[boids_.IndexOf(rip_boid) + 1].transform);
                }
                // 次のインデックスがない(これが最後の一匹)とき
                catch (System.ArgumentException)
                {
                    // Cameraの親子関係、座標、角度を元に戻す
                    Camera.main.transform.parent = null;
                    Camera.main.transform.position = Vector3.zero;
                    Camera.main.transform.eulerAngles = Vector3.zero;
                }
            }

            // Boid listから削除
            boids_.RemoveAt(boids_.IndexOf(rip_boid));

            // 削除した結果、Boid list 空なら
            if (boids_.Count == 0)
            {
                gameManager.BoidsAreAllDead = true;

                // GameObjectとしても削除
                Destroy(rip_boid.gameObject);

                return;
            }


            // gameObjectを削除
            Destroy(rip_boid.gameObject);

        }
        #endregion 


        void Update()
        {
            // シーン再ロード時生成する
            while (boids_.Count < boidCount)
            {
                // ゲーム中～シーンロード、=beforeStartがfalseである間は生成しない
                // ゲーム開始後にSimulationがアクティブになるので、その後１秒間の間に生成する)
                if (gameManager.GameTime - gameManager.GameRemainTime > 1 && !gameManager.beforeStart) break;
                AddBoid();
            }

            // タイトルシーンではずっときれいな姿で泳がせる
            if (SceneManager.GetActiveScene().name == "TitleScene")
            {
                isTitleNow = true;
                //isFirstGameFrame = true;
                //param.Reset();
            }
            // ゲーム始まって最初のフレームで入る
            // else if (isFirstGameFrame && gameManager.isGaming)
            // {
            //     //ゲーム開始直後パラメータリセット(スタートボタン押すまでに)
            //     param.Reset();
            //     isFirstGameFrame = false;
            else isTitleNow = false;
            // }


            // 時間経過によるBoidsの変化
            // 障害物処理---
            if (detectedCount >= param.detectedObstacleBoids)
            {
                SetFlocking();
                ResetBoidsDetectedObstacleFlag(boids_);
            }
            // 一定時間むれたあと
            if (Time.realtimeSinceStartup - timer_flocking > param.DurationFlocking && param.isFlocking)
            {
                // タイトルシーンとゲーム前はきれいなパラメータを維持 タイマー更新のみ
                if (isTitleNow || gameManager.beforeStart)
                {
                    timer_flocking = Time.realtimeSinceStartup;
                    return;
                }

                SetDeFlocking();
                ResetBoidsDetectedObstacleFlag(boids_);
            }


            // 餌やり処理---
            if (isFeeded)
            {
                SetBoidPowerUp();
                SetBoidsMovingToTargetFlag(boids_);
            }
            // 餌与えてから一定期間たった後
            if (Time.realtimeSinceStartup - timer_powerful > param.DurationPowerful && param.isPowerful)
            {
                // タイトルシーンとゲーム前はきれいなパラメータを維持 タイマー更新のみ
                if (isTitleNow || gameManager.beforeStart)
                {
                    timer_powerful = Time.realtimeSinceStartup;
                    return;
                }
                SetBoidPowerDown();
                ResetBoidsMovingToTagetFlag(boids_);
            }

        }

        #region イベント処理

        /// 障害物 ------------------------------------------------

        /// <summary>
        /// 障害物を検知したBoidが立てたフラグを下げる
        /// </summary>
        void ResetBoidsDetectedObstacleFlag(List<Boid> boids)
        {
            foreach (Boid boid in boids)
            {
                boid.DetectedObstacle = false;
            }
        }

        /// <summary>
        /// Boidsを群れさせる
        /// </summary>
        void SetFlocking()
        {
            // 群れを形成するようにパラメータを変更
            param.minSpeed = 3f;
            param.maxSpeed = 7f;
            param.isPowerful = true;
            timer_powerful = Time.realtimeSinceStartup;
            param.neighborFov = 120f;
            param.neighborDistance = 2.5f;
            param.isFlocking = true;
            timer_flocking = Time.realtimeSinceStartup;
            detectedCount = 0;
        }

        /// <summary>
        /// Boidsを離散させる
        /// </summary>
        void SetDeFlocking()
        {
            // 群れが離散するようにパラメータを変更
            param.neighborFov = 1f;
            param.neighborDistance = 0;
            param.isFlocking = false;
            detectedCount = 0;
        }

        /// 餌-------------------------------------------------

        /// <summary>
        /// 餌に向かって移動するフラグをBoids全員立てる
        /// </summary>
        void SetBoidsMovingToTargetFlag(List<Boid> boids)
        {
            foreach (Boid boid in boids)
            {
                boid.movingToTaget = true;
            }
        }

        /// <summary>
        /// 餌に向かって移動するフラグをBoids全員さげる
        /// </summary>
        void ResetBoidsMovingToTagetFlag(List<Boid> boids)
        {
            foreach (Boid boid in boids)
            {
                boid.movingToTaget = false;
            }
        }

        /// <summary>
        /// 餌を出したら皆早くなる(体力はここでは回復しない)
        /// </summary>
        void SetBoidPowerUp()
        {
            // 餌やり開始
            param.minSpeed = 3f;
            param.maxSpeed = 7f;
            param.isPowerful = true;
            timer_powerful = Time.realtimeSinceStartup;
            isFeeded = false;

        }

        /// <summary>
        /// 餌を与え一定時間後元気なくなる
        /// </summary>
        void SetBoidPowerDown()
        {
            //餌やり終了
            param.minSpeed = 2f;
            param.maxSpeed = 5f;
            param.isPowerful = false;
        }

        /// <summary>
        /// 餌の座標をすべてのBoidsに教える
        /// </summary>
        /// <param name="targetPos">餌が生成される座標</param>
        public void SetBoidTargetPos(Vector3 targetPos)
        {
            // Boid に集まる先の座標を指定
            foreach (Boid boid in boids)
            {
                boid.TargetPos = targetPos;
            }
        }

        #endregion
    }
}
