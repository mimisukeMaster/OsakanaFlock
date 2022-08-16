using UnityEngine;
using System.Reflection;

namespace Boid
{

    [CreateAssetMenu(menuName = "Assets/BoidsScripts/Common/Param")]
    public class Param : ScriptableObject
    {
        [Tooltip("発射速度")]
        public float initSpeed = 2f;
        [Tooltip("移動中の最小速度")]
        public float minSpeed = 2f;
        [Tooltip("移動中の最大速度")]
        public float maxSpeed = 5f;
        [Tooltip("近隣の個体として認識する距離")]
        public float neighborDistance = 2f;
        [Tooltip("近隣の個体として認識する前方角")]
        public float neighborFov = 90f;
        [Tooltip("分離のウエイト、大きいほどばらばらに")]
        public float separationWeight = 6f;
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
        public float proximityThr = 1.3f;
        [Tooltip("時間とともに減速する速さの変化率\nTime.deltatimeにこの値をかけたもので毎フレームデクリメント")]
        public float speedDampingRatio = 0.035f;
        [Tooltip("時間とともに近隣の認識角が小さくなっていく、その下降比率\n" +
        "Time.deltatimeにこの値をかけたもので毎フレームデクリメント")]
        public float neighborFovRatio = 3.0f;
        [Tooltip("集団の動きが乱れるパラメータ設定に変わっていく間隔")]
        public float disarrangedInverval = 20.0f;
        [Tooltip("障害物を避け始める残りの障害物との距離")]
        public float avoidDistance = 9.0f;
        [Tooltip("障害物を避けるウエイト、大きいほど忌避")]
        public float avoidWeight = 1.0f;


        #region disarranged params
        [Header("--Disarranged params--")]
        [Tooltip("押しても意味ないよ")]
        public bool o;
        [Header("Step1")]
        [Tooltip("一回目の変化で変わる値")]
        public float neighborFov_d1 = 5.0f;
        [Header("Step2")]
        [Tooltip("二回目の変化で変わる値")]
        public float minSpeed_d2 = 1.0f;
        [Tooltip("二回目の変化で変わる値")]
        public float maxSpeed_d2 = 1.5f;
        [Tooltip("二回目の変化で変わる値")]
        public float neighborFov_d2 = 10.0f;
        #endregion
        public void Reset()
        {
            // 変更されたパラメータをリセット
            initSpeed = 2f;
            minSpeed = 2f;
            maxSpeed = 5f;
            neighborDistance = 2f;
            neighborFov = 90f;
            separationWeight = 6f;
            wallScale = 30f;
            wallDistance = 3f;
            wallWeight = 1f;
            alignmentWeight = 2f;
            cohesionWeight = 3f;
            targetSpeed = 1f;
            proximityThr = 1.3f;
            speedDampingRatio = 0.035f;
            neighborFovRatio = 3.0f;
            disarrangedInverval = 20.0f;
            avoidDistance = 9.0f;
            avoidWeight = 1.0f;

        }
    }
}
