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
        public Transform[] Obstacles;
        public Vector3 ColliderSize;
        public int detectedCount = 0;
        public bool isFeeded;
        public bool isTitleNow;
        public bool isFirstGameFrame;
        public bool isGameStarted;
        public float timer_powerful = 0;
        public float timer_flocking = 0;

        void Start()
        {
            ColliderSize = GetComponent<BoxCollider>().size;
            param.Reset();

            ParticleSystem.SetMaximumPreMappedBufferCounts(1, 1);

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

        // HP0のBoidを消す
        public void RemoveBoid(Boid rip_boid)
        {
            // main camera を含んでいる場合はCameraを次のインデックスのBoidに移ってもらう
            if (rip_boid.transform.childCount == 5)
            {
                try
                {
                    Camera.main.transform.SetParent(boids_[boids_.IndexOf(rip_boid) + 1].transform);
                }
                catch (System.ArgumentException)
                {
                    Camera.main.transform.parent = null;
                    Camera.main.transform.position = Vector3.zero;
                    Camera.main.transform.eulerAngles = Vector3.zero;
                }
            }
            // Boid listからも削除
            boids_.RemoveAt(boids_.IndexOf(rip_boid));

            // 削除した結果、Boid list 空なら
            if (boids_.Count == 0)
            {
                gameManager.BoidsAreAllDead = true;

                // 残り一匹をどうするか
                Destroy(rip_boid.gameObject);

                return;
            }


            // gameObjectを削除
            Destroy(rip_boid.gameObject);

        }
        #endregion 


        void Update()
        {
            // シーン再ロード時生成する、ゲーム中の死、再ロード前に生成されないようにフラグ
            while (boids_.Count < boidCount && isGameStarted)
            {
                AddBoid();
            }
            isGameStarted = false;

            // タイトルではきれいな姿で泳がせたいのでReset()、HP維持フラグ、タイマー更新
            if (SceneManager.GetActiveScene().name == "TitleScene")
            {
                isTitleNow = true;
                isFirstGameFrame = true;
                param.Reset();
            }
            // ゲーム始まって最初のフレームで入る
            else if (isFirstGameFrame && gameManager.isGaming)
            {
                //スタート直後のparam.Reset() はい　いいえの選択時間減った場合の値のリカバー
                param.Reset();
                isFirstGameFrame = false;
                isTitleNow = false;
            }


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
                SetBoidPowerDown();
                ResetBoidsMovingToTagetFlag(boids_);
            }

        }

        #region イベント処理

        /// 障害物-------------------------------------------------
        void ResetBoidsDetectedObstacleFlag(List<Boid> boids)
        {
            foreach (Boid boid in boids)
            {
                boid.DetectedObstacle = false;
            }
        }
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
        void SetDeFlocking()
        {
            // 群れが離散するようにパラメータを変更
            param.neighborFov = 1f;
            param.neighborDistance = 0;
            param.isFlocking = false;
            detectedCount = 0;
        }

        /// 餌-------------------------------------------------
        void SetBoidsMovingToTargetFlag(List<Boid> boids)
        {
            foreach (Boid boid in boids)
            {
                boid.movingToTaget = true;
            }
        }
        void ResetBoidsMovingToTagetFlag(List<Boid> boids)
        {
            foreach (Boid boid in boids)
            {
                boid.movingToTaget = false;
            }
        }
        void SetBoidPowerUp()
        {
            // 餌やり開始
            param.minSpeed = 3f;
            param.maxSpeed = 7f;
            param.isPowerful = true;
            timer_powerful = Time.realtimeSinceStartup;
            isFeeded = false;

        }
        void SetBoidPowerDown()
        {
            //餌やり終了処理（与えた後一定時間後元気なくなる処理）
            param.minSpeed = 2f;
            param.maxSpeed = 5f;
            param.isPowerful = false;
        }

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
