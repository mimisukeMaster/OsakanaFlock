using UnityEngine;
using System.Reflection;

namespace Boid
{

    [CreateAssetMenu(menuName = "Assets/BoidsScripts/Common/Param"), HideInInspector]
    public class Param : ScriptableObject
    {
        [Tooltip("発射速度"), SerializeField]
        public float initSpeed = 2f;
        [Tooltip("移動中の最小速度"), SerializeField]
        public float minSpeed = 2f;
        [Tooltip("移動中の最大速度"), SerializeField]
        public float maxSpeed = 5f;
        [Tooltip("近隣の個体として認識する距離"), SerializeField]
        public float neighborDistance = 2f;
        [Tooltip("近隣の個体として認識する前方角"), SerializeField]
        public float neighborFov = 90f;
        [Tooltip("分離のウエイト、大きいほどばらばらに"), SerializeField]
        public float separationWeight = 5f;
        [Tooltip("仮想の壁のサイズ"), SerializeField]
        public float wallScale = 30f;
        [Tooltip("壁から離れようとする残りの壁との距離"), SerializeField]
        public float wallDistance = 3f;
        [Tooltip("壁からよける力のウエイト"), SerializeField]
        public float wallWeight = 1f;
        [Tooltip("整列のウエイト、大きいほどみな同じ方向に"), SerializeField]
        public float alignmentWeight = 2f;
        [Tooltip("結合のウエイト、大きいほど集まる"), SerializeField]
        public float cohesionWeight = 3f;

        [Header("My param"), SerializeField]
        [Tooltip("目標の点に向かうスピード\n遅いほど3ルールが勝り目標点に中々到達できないので周りを徘徊する")]
        public float targetSpeed = 1f;
        [Tooltip("目標の点への移動をやめる距離のしきい値\n遅いほど3ルールが勝り" +
        "目標点に中々到達できないので周りを徘徊する\ntargetSpeedより顕著に現れる"), SerializeField]
        public float proximityThr = 1.3f;
        [Tooltip("時間とともに減速する速さの変化率\nTime.deltatimeにこの値をかけたもので毎フレームデクリメント"), SerializeField]
        public float speedDampingRatio = 0.035f;
        [Tooltip("時間とともに近隣の認識角が小さくなっていく、その下降比率\n" +
        "Time.deltatimeにこの値をかけたもので毎フレームデクリメント")]
        public float neighborFovRatio = 3.0f;

        public void Reset()
        {
            // 変更されたパラメータをリセット
            initSpeed = 2f;
            minSpeed = 2f;
            maxSpeed = 5f;
            neighborDistance = 2f;
            neighborFov = 90f;
            separationWeight = 5f;
            wallScale = 30f;
            wallDistance = 3f;
            wallWeight = 1f;
            alignmentWeight = 2f;
            cohesionWeight = 3f;
            targetSpeed = 1f;
            proximityThr = 1.3f;
            speedDampingRatio = 0.035f;
            neighborFovRatio = 3.0f;
        }
    }
}
