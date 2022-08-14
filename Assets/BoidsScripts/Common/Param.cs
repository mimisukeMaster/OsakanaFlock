using UnityEngine;

namespace Boid
{

    [CreateAssetMenu(menuName = "Common/Param")]
    public class Param : ScriptableObject
    {
        [Tooltip("発射速度")]
        public float initSpeed = 2f;
        [Tooltip("移動中の最小速度")]
        public float minSpeed = 2f;
        [Tooltip("移動中の最大速度")]
        public float maxSpeed = 5f;
        [Tooltip("近隣の個体として認識する距離")]
        public float neighborDistance = 1f;
        [Tooltip("近隣の個体として認識する前方角")]
        public float neighborFov = 90f;
        [Tooltip("分離のウエイト、大きいほどばらばらに")]
        public float separationWeight = 5f;
        [Tooltip("仮想の壁のサイズ")]
        public float wallScale = 5f;
        [Tooltip("壁から離れようとする残りの壁との距離")]
        public float wallDistance = 3f;
        [Tooltip("壁からよける力のウエイト")]
        public float wallWeight = 1f;
        [Tooltip("整列のウエイト、大きいほどみな同じ方向に")]
        public float alignmentWeight = 2f;
        [Tooltip("結合のウエイト、大きいほど集まる")]
        public float cohesionWeight = 3f;


        [Header("My param")]

        [Tooltip("目標の点に向かうスピード")]
        public float targetSpeed;
        [Tooltip("目標の点への移動をやめる距離のしきい値")]
        public float proximityThr;

    }

}
