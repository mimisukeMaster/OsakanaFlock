using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotator : MonoBehaviour
{
    // カメラオブジェクトを格納する変数
    public Camera mainCamera;
    // カメラの回転速度を格納する変数
    public Vector2 rotationSpeed;
    // マウス移動方向とカメラ回転方向を反転する判定フラグ
    public bool reverse;
    // マウス座標を格納する変数
    private Vector2 lastMousePosition;
    // カメラの角度を格納する変数（初期値に0,0を代入）
    private Vector2 newAngle = new Vector2(0, 0);

    // ゲーム実行中の繰り返し処理
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            // カメラの角度を変数"newAngle"に格納
            newAngle = mainCamera.transform.localEulerAngles;
            // マウス座標を変数"lastMousePosition"に格納
            lastMousePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButton(1))
        {
            //カメラ回転方向の判定フラグが"true"の場合
            if (!reverse)
            {
                // Y軸の回転：マウスドラッグ方向に視点回転
                // マウスの水平移動値に変数"rotationSpeed"を掛ける
                //（クリック時の座標とマウス座標の現在値の差分値）
                newAngle.y -= (lastMousePosition.x - Input.mousePosition.x) * rotationSpeed.y;
                // X軸の回転：マウスドラッグ方向に視点回転
                // マウスの垂直移動値に変数"rotationSpeed"を掛ける
                //（クリック時の座標とマウス座標の現在値の差分値）
                newAngle.x -= (Input.mousePosition.y - lastMousePosition.y) * rotationSpeed.x;
                // "newAngle"の角度をカメラ角度に格納
                mainCamera.transform.localEulerAngles = newAngle;
                // マウス座標を変数"lastMousePosition"に格納
                lastMousePosition = Input.mousePosition;
            }
        }
    }
}
