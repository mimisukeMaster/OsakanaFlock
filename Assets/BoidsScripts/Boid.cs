﻿using UnityEngine;
using System.Collections.Generic;

namespace Boid.OOP
{

    public class Boid : MonoBehaviour
    {
        public Simulation simulation { get; set; }
        public Param param { get; set; }
        public Vector3 pos { get; private set; }
        public Vector3 velocity { get; private set; }
        Vector3 accel = Vector3.zero;
        List<Boid> neighbors = new List<Boid>();

        //目標の点への移動フラグ
        bool movingToTaget;

        void Start()
        {
            pos = transform.position;
            velocity = transform.forward * param.initSpeed;
        }

        void Update()
        {
            UpdateNeighbors();
            UpdateWalls();
            UpdateSeparation();
            UpdateAlignment();
            UpdateCohesion();
            UpdateMove();
            if (Input.GetMouseButtonDown(0) || movingToTaget) UpdateMoveToPoint();

        }

        /// <summary>
        /// 分離・整列・結合には近隣の個体の情報が必要です。
        /// Simulation クラスから Boids のリストを貰ってきて総当たりで、
        /// ある距離（neighborDistance）以内かつある角度（neighborFov）以内にいる
        /// 個体を全部集めることにします。
        /// </summary>
        void UpdateNeighbors()
        {
            neighbors.Clear();

            if (!simulation) return;

            var prodThresh = Mathf.Cos(param.neighborFov * Mathf.Deg2Rad);
            var distThresh = param.neighborDistance;

            foreach (var other in simulation.boids)
            {
                if (other == this) continue;

                var to = other.pos - pos;
                var dist = to.magnitude;
                if (dist < distThresh)
                {
                    var dir = to.normalized;
                    var fwd = velocity.normalized;
                    var prod = Vector3.Dot(fwd, dir);
                    if (prod > prodThresh)
                    {
                        neighbors.Add(other);
                    }
                }
            }
        }

        /// <summary>
        /// 範囲内に留めるために壁には近づけば近づくほど
        /// 離れる方向（壁の内側方向）の力を受けることにして accel を更新します。
        /// wallScale の立方体の内側にいる想定で、各 6 面の壁から受ける力を計算しています。
        /// </summary>
        void UpdateWalls()
        {
            if (!simulation) return;

            var scale = param.wallScale * 0.5f;
            accel +=
                CalcAccelAgainstWall(-scale - pos.x, Vector3.right) +
                CalcAccelAgainstWall(-scale - pos.y, Vector3.up) +
                CalcAccelAgainstWall(-scale - pos.z, Vector3.forward) +
                CalcAccelAgainstWall(+scale - pos.x, Vector3.left) +
                CalcAccelAgainstWall(+scale - pos.y, Vector3.down) +
                CalcAccelAgainstWall(+scale - pos.z, Vector3.back);
        }

        Vector3 CalcAccelAgainstWall(float distance, Vector3 dir)
        {
            if (distance < param.wallDistance)
            {
                return dir * (param.wallWeight / Mathf.Abs(distance / param.wallDistance));
            }
            return Vector3.zero;
        }

        /// <summary>
        /// 分離<br></br>これは近隣の個体から離れる方向に力を加えます。
        /// かかる力は雑ですが簡単のために一定とします。
        /// </summary>
        void UpdateSeparation()
        {
            if (neighbors.Count == 0) return;

            Vector3 force = Vector3.zero;
            foreach (var neighbor in neighbors)
            {
                force += (pos - neighbor.pos).normalized;
            }
            force /= neighbors.Count;

            accel += force * param.separationWeight;
        }

        /// <summary>
        /// 整列<br></br>
        /// 近隣の個体の速度平均を求め、それに近づくように accel にフィードバックをします
        /// </summary>
        void UpdateAlignment()
        {
            if (neighbors.Count == 0) return;

            var averageVelocity = Vector3.zero;
            foreach (var neighbor in neighbors)
            {
                averageVelocity += neighbor.velocity;
            }
            averageVelocity /= neighbors.Count;

            accel += (averageVelocity - velocity) * param.alignmentWeight;
        }

        /// <summary>
        /// 結合<br></br>近隣の個体の中心方向へ accel を増やすように更新します
        /// </summary>
        void UpdateCohesion()
        {
            if (neighbors.Count == 0) return;

            var averagePos = Vector3.zero;
            foreach (var neighbor in neighbors)
            {
                averagePos += neighbor.pos;
            }
            averagePos /= neighbors.Count;

            accel += (averagePos - pos) * param.cohesionWeight;
        }


        /// <summary>
        ///  accel を使って速度・位置を求めて、transform へと反映を行います。
        /// 最低速度と最高速度を決めておくのがキモで、それっぽく見えるようになります。
        /// </summary>
        void UpdateMove()
        {
            var dt = Time.deltaTime;

            velocity += accel * dt;
            var dir = velocity.normalized;
            var speed = velocity.magnitude;
            velocity = Mathf.Clamp(speed, param.minSpeed, param.maxSpeed) * dir;
            pos += velocity * dt;

            var rot = Quaternion.LookRotation(velocity);
            transform.SetPositionAndRotation(pos, rot);

            accel = Vector3.zero;
        }

        void UpdateMoveToPoint()
        {
            var targetPos = new Vector3(0, 3, 0);

            Debug.DrawRay(targetPos, targetPos + Vector3.forward, Color.magenta, 900);
            Vector3 dirVector = targetPos - transform.position;
            accel += dirVector * param.targetSpeed;

            movingToTaget = true;
            if (Vector3.Distance(targetPos, transform.position) <= param.proximityThr)
            {
                Debug.Log("もう着いたわ");
                movingToTaget = false;
            }
        }
    }

}
