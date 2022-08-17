using UnityEngine;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Boid.OOP
{

    public class Simulation : MonoBehaviour
    {
        [SerializeField]
        int boidCount = 100;

        [SerializeField]
        GameObject boidPrefab;

        [SerializeField]
        Param param;

        List<Boid> boids_ = new List<Boid>();
        public ReadOnlyCollection<Boid> boids
        {
            get { return boids_.AsReadOnly(); }
        }
        public Transform[] Obstacles;
        public Vector3 ColliderSize;
        public int detectedCount = 0;
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

        void AddBoid()
        {
            var go = Instantiate(boidPrefab, Random.insideUnitSphere, Random.rotation);
            go.transform.SetParent(transform);
            var boid = go.GetComponent<Boid>();
            boid.simulation = this;
            boid.param = param;
            boids_.Add(boid);
        }

        void RemoveBoid()
        {
            if (boids_.Count == 0) return;

            var lastIndex = boids_.Count - 1;
            var boid = boids_[lastIndex];
            Destroy(boid.gameObject);
            boids_.RemoveAt(lastIndex);
        }

        void Update()
        {
            while (boids_.Count < boidCount)
            {
                AddBoid();
            }
            while (boids_.Count > boidCount)
            {
                RemoveBoid();
            }

            // 時間経過によるBoidsの変化


            // 障害物処理
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

            // 餌やり処理
            if (Input.GetMouseButtonDown(0))
            {
                SetBoidPowerUp();
            }
            // 餌与えてから一定期間たった後
            if (Time.realtimeSinceStartup - timer_powerful > param.DurationPowerful && param.isPoweful)
            {
                SetBoidPowerDown();
                ResetBoidsMovingToTagetFlag(boids_);

            }

        }


        void ResetBoidsDetectedObstacleFlag(List<Boid> boids)
        {
            Debug.Log("群れさせる");
            foreach (Boid boid in boids)
            {
                boid.DetectedObstacle = false;
            }
        }
        void ResetBoidsMovingToTagetFlag(List<Boid> boids)
        {
            Debug.Log("餌はもうつきた");
            foreach (Boid boid in boids)
            {
                boid.movingToTaget = false;
            }
        }

        public void SetFlocking()
        {
            // 群れを形成するようにパラメータを変更
            param.minSpeed = 2f;
            param.maxSpeed = 5f;
            param.isPoweful = true;
            timer_powerful = Time.realtimeSinceStartup;
            param.neighborFov = 90f;
            param.isFlocking = true;
            timer_flocking = Time.realtimeSinceStartup;
            detectedCount = 0;
        }
        public void SetDeFlocking()
        {
            // 群れが離散するようにパラメータを変更
            param.neighborFov = 1f;
            param.isFlocking = false;
            detectedCount = 0;
        }
        void SetBoidPowerUp()
        {
            // 餌やり開始
            param.minSpeed = 2;
            param.maxSpeed = 5;
            param.isPoweful = true;
            timer_powerful = Time.realtimeSinceStartup;
            //param.neighborFov = 90f;
            //param.isFlocking = true;
            //timer_flocking = Time.realtimeSinceStartup;

        }
        void SetBoidPowerDown()
        {
            //餌やり終了処理（与えた後一定時間後元気なくなる処理）
            param.minSpeed = 1.0f;
            param.maxSpeed = 1.5f;
            param.isPoweful = false;
        }

    }
}
