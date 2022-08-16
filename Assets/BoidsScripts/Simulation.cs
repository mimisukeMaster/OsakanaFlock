﻿using UnityEngine;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine.SceneManagement;

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
        float timer;
        int disarrangedStep = 1;

        void Awake()
        {
            ColliderSize = GetComponent<BoxCollider>().size;

            // パラメータをリセット
            param.Reset();

            timer = Time.realtimeSinceStartup;
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
            UpdateParam();



            // Debug.Log(Obstacles[0].transform.position);
            // if (Input.GetMouseButtonDown(1))
            // {
            //     Vector3 launchPos = Camera.main.transform.position;
            //     Transform obs = Instantiate(Obstacles[0], launchPos, Quaternion.identity);
            //     // シーンのものを登録
            //     Obstacles[0] = GameObject.Find("Obstacle_01(Clone)").transform;
            //     Rigidbody obs_rb = Obstacles[0].GetComponent<Rigidbody>();
            //     obs_rb.AddForce(new Vector3(-4000, -2000f, 4000));
            // }
            // if (Input.GetKeyDown(KeyCode.Space)) SceneManager.LoadScene("FlockingScene");
        }

        void UpdateParam()
        {

            // 指定秒たつごとに乱れる
            if (Time.realtimeSinceStartup - timer > param.disarrangedInverval)
            {
                if (disarrangedStep == 1)
                {
                    param.neighborFov = param.neighborFov_d1;
                    Debug.Log("step１");
                }
                else if (disarrangedStep == 2)
                {
                    param.minSpeed = param.minSpeed_d2;
                    param.maxSpeed = param.maxSpeed_d2;
                    param.neighborFov = param.neighborFov_d2;
                    Debug.Log("step2");
                }
                // タイマー更新
                timer = Time.realtimeSinceStartup;

                Debug.Log("ガーン");
                disarrangedStep++;
            }
        }
    }
}