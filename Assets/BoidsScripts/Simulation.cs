using UnityEngine;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Boid.OOP
{

    public class Simulation : MonoBehaviour
    {
        [SerializeField]
        public int boidCount = 450;

        [SerializeField]
        GameObject boidPrefab;

        [SerializeField]
        Param param;
        public GameManager gameManager;

        List<Boid> boids_ = new List<Boid>();
        public ReadOnlyCollection<Boid> boids
        {
            get { return boids_.AsReadOnly(); }
        }
        public Transform[] Obstacles;
        public Vector3 ColliderSize;
        public int detectedCount = 0;
        public bool isFeeded;


        float timer_powerful = 0;
        float timer_flocking = 0;

        void Awake()
        {
            ColliderSize = GetComponent<BoxCollider>().size;

            // パラメータをリセット
            param.Reset();

            //timer_disarraged = Time.realtimeSinceStartup;
            timer_powerful = Time.realtimeSinceStartup;
            timer_flocking = Time.realtimeSinceStartup;
        }
        void Start()
        {
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
            if (boids_.Count == 0)
            {
                gameManager.BoidsAreAllDead = true;
                return;
            }
            // var lastIndex = boids_.Count - 1;
            // var boid = boids_[lastIndex];

            // gameObjectを削除
            Destroy(rip_boid.gameObject);

            // Boid listからも削除
            boids_.RemoveAt(boids_.IndexOf(rip_boid));
        }
        /// 現在のBoidsの数を取得する<seealso cref="UIManager.Update()">で呼び出し
        public int GetNowAliveBoids() => boids_.Count;

        #endregion 


        void Update()
        {
            // while (boids_.Count < boidCount)
            // {
            //     AddBoid();
            // }
            // while (boids_.Count > boidCount)
            // {
            //     RemoveBoid();
            // }

            // 時間経過によるBoidsの変化


            // 障害物処理---
            if (detectedCount >= param.detectedObstacleBoids)
            {
                SetFlocking();
                ResetBoidsDetectedObstacleFlag(boids_);
            }
            // 一定時間むれたあと
            if (Time.realtimeSinceStartup - timer_flocking > param.Duration_flocking && param.isFlocking)
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
            Debug.Log("群れさせる");
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
                // 現在見えていない(レンダリングされていない)ならそのBoidのFlagは上げない
                //if (!boid.myRenderer.isVisible) continue;

                boid.movingToTaget = true;
                Debug.Log("俺指名された餌食える");
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
            //param.neighborFov = 90f;
            //param.isFlocking = true;
            //timer_flocking = Time.realtimeSinceStartup;

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
