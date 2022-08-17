using UnityEngine;
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

        public bool DetectedObstacle; // 障害物を検知した際のフラグ
        public bool movingToTaget;   //目標の点への移動フラグ
        Vector3 BoundingBox;  // 移動範囲のバウンディングボックス

        float HP;
        float maxHP = 10.0f;
        float HPRatio = 0.1f;
        void Start()
        {

            pos = transform.position;
            velocity = transform.forward * param.initSpeed;

            BoundingBox = simulation.ColliderSize;

            HP = maxHP;
        }

        void Update()
        {
            UpdateNeighbors();
            UpdateWalls();
            UpdateSeparation();
            UpdateAlignment();
            UpdateCohesion();
            UpdateMove();
            UpdateAvoidObstacles(simulation.Obstacles);

            if (movingToTaget) UpdateMoveToPoint(Vector3.up * 3);

            if (Input.GetMouseButtonDown(0)) movingToTaget = true;

            // HPを徐々に減らす
            HP -= Time.deltaTime;

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

            var scaleXP = BoundingBox.x * 0.5f + transform.parent.position.x;
            var scaleXM = BoundingBox.x * 0.5f - transform.parent.position.x;
            var scaleYP = BoundingBox.y * 0.5f + transform.parent.position.y;
            var scaleYM = BoundingBox.y * 0.5f - transform.parent.position.y;
            var scaleZP = BoundingBox.z * 0.5f + transform.parent.position.z;
            var scaleZM = BoundingBox.z * 0.5f - transform.parent.position.z;

            accel +=
                CalcAccelAgainstWall(-scaleXM - pos.x, Vector3.right) +
                CalcAccelAgainstWall(-scaleYM - pos.y, Vector3.up) +
                CalcAccelAgainstWall(-scaleZM - pos.z, Vector3.forward) +
                CalcAccelAgainstWall(+scaleXP - pos.x, Vector3.left) +
                CalcAccelAgainstWall(+scaleYP - pos.y, Vector3.down) +
                CalcAccelAgainstWall(+scaleZP - pos.z, Vector3.back);
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

        /// <summary>
        /// 障害物に近ければ近いほど逃げる方向のベクトルを強めaccelに加算
        /// </summary>
        void UpdateAvoidObstacles(Transform[] Obstalcles)
        {
            foreach (Transform obs in simulation.Obstacles)
            {
                // 空間の距離がまだ避ける距離でないならなにもしない
                if (Vector3.Distance(pos, obs.position) >= param.avoidDistance) break;

                // 見つけたらSimulation.detectedCountに見つけたBoidとして加算する
                if (!DetectedObstacle)
                {
                    simulation.detectedCount++;
                    DetectedObstacle = true;
                    Debug.Log("++");
                }

                // Wallと違って六面から(反対側,一つの軸に二つよける対象がある)ではないので計算量はXYZ軸一回づつのみで済む
                // Vector3.Distanceをとるとfloatが返されVector成分情報が失われてしまうので成分ごとにCalc
                accel +=
                    CalcAccelAgainstObstacle(pos.x - obs.transform.position.x, Vector3.left) +
                    CalcAccelAgainstObstacle(pos.y - obs.transform.position.y, Vector3.down) +
                    CalcAccelAgainstObstacle(pos.z - obs.transform.position.z, Vector3.back);

                Debug.DrawLine(pos, pos + accel, Color.magenta);

            }
        }

        /// <summary>
        /// 障害物から避ける計算処理
        /// </summary>
        /// <param name="distance">現在の自分と障害物との距離</param>
        /// <param name="dir">計算するベクトル成分(軸)</param>
        /// <returns name="dir">重みづけした逃げる方向のベクトル</returns>
        /// <returns name="Vector3.zero">近くない場合はなし</returns>
        Vector3 CalcAccelAgainstObstacle(float distance, Vector3 dir)
         => dir * (param.avoidWeight / Mathf.Pow((Mathf.Abs(distance / param.avoidDistance)), 2));


        /// <summary>
        /// 指定した座標までBoidを滑らかに誘導する
        /// </summary>
        /// <param name="targetPos">指定した座標</param>
        void UpdateMoveToPoint(Vector3 targetPos)
        {
            accel += (targetPos - pos) * param.targetSpeed;

            if (Vector3.Distance(targetPos, pos) <= param.proximityThr)
            {
                // hpアップ処理
                HP = maxHP;

                /// TODO
                /// 一定時間たったら諦めさせる処理

                Debug.Log("REACHED!");
                movingToTaget = false;
            }
        }
    }
}
