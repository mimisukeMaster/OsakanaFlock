using UnityEngine;
using System.Reflection;

namespace Boid
{

    [CreateAssetMenu(menuName = "Assets/BoidsScripts/Common/Param")]
    public class Param : ScriptableObject
    {
        [Tooltip("発射速度")]
        public float initSpeed = 0.5f;
        [Tooltip("移動中の最小速度")]
        public float minSpeed = 3f;
        [Tooltip("移動中の最大速度")]
        public float maxSpeed = 7f;
        [Tooltip("近隣の個体として認識する距離")]
        public float neighborDistance = 2.5f;
        [Tooltip("近隣の個体として認識する前方角")]
        public float neighborFov = 120f;
        [Tooltip("分離のウエイト、大きいほどばらばらに")]
        public float separationWeight = 6.5f;
        [Tooltip("仮想の壁のサイズ")]
        public float wallScale = 30f;
        [Tooltip("壁から離れようとする残りの壁との距離")]
        public float wallDistance = 3f;
        [Tooltip("壁からよける力のウエイト")]
        public float wallWeight = 1f;
        [Tooltip("整列のウエイト、大きいほどみな同じ方向に")]
        public float alignmentWeight = 2f;
        [Tooltip("結合のウエイト、大きいほど集まる")]
        public float cohesionWeight = 3f;



        [Header("My param")]
        [Tooltip("目標の点に向かうスピード\n遅いほど3ルールが勝り目標点に中々到達できないので周りを徘徊する")]
        public float targetSpeed = 1f;
        [Tooltip("目標の点への移動をやめる距離のしきい値\n遅いほど3ルールが勝り" +
        "目標点に中々到達できないので周りを徘徊する\ntargetSpeedより顕著に現れる")]
        public float proximityThr = 1f;
        [Tooltip("障害物を避け始める残りの障害物との距離")]
        public float avoidDistance = 9.0f;
        [Tooltip("障害物を避けるウエイト、大きいほど忌避")]
        public float avoidWeight = 1.0f;
        [Tooltip("障害物を検知したBoidの数")]
        public int detectedObstacleBoids = 15;
        [Tooltip("群れてる期間(purfulと並立)")]
        public float Duration_flocking = 20.0f;
        [Tooltip("群れているか")]
        public bool isFlocking = true;
        [Tooltip("びんびん速度がある期間(flockingと並立)")]
        public float DurationPowerful = 10.0f;
        [Tooltip("びんびん速度があるか")]
        public bool isPowerful = true;

        public void Reset()
        {
            // 変更されたパラメータをリセット
            initSpeed = 2f;
            minSpeed = 3f;
            maxSpeed = 7f;
            neighborDistance = 2.5f;
            neighborFov = 120f;
            separationWeight = 6.5f;
            wallScale = 30f;
            wallDistance = 3f;
            wallWeight = 1f;
            alignmentWeight = 2f;
            cohesionWeight = 3f;

            /// --my param--
            targetSpeed = 1f;
            proximityThr = 1f;
            avoidDistance = 9.0f;
            avoidWeight = 1.0f;
            detectedObstacleBoids = 15;
            Duration_flocking = 20.0f;
            DurationPowerful = 10.0f;
            isFlocking = true;
            isPowerful = true;
        }
    }
}
