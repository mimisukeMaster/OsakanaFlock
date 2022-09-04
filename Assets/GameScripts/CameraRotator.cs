using UnityEngine;

public class CameraRotator : MonoBehaviour
{
    public Camera mainCamera;
    public Vector2 rotationSpeed;

    Vector2 lastMousePosition;
    Vector2 newAngle = new Vector2(0, 0);

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            // カメラの角度
            newAngle = mainCamera.transform.localEulerAngles;
            // マウス座標
            lastMousePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButton(1))
        {
            // Y軸の回転　クリック時の座標と現在値の差分
            newAngle.y -= (lastMousePosition.x - Input.mousePosition.x) * rotationSpeed.y;

            // X軸の回転　クリック時の座標と現在値の差分
            newAngle.x -= (Input.mousePosition.y - lastMousePosition.y) * rotationSpeed.x;

            mainCamera.transform.localEulerAngles = newAngle;

            // マウス座標更新
            lastMousePosition = Input.mousePosition;
        }
    }
}
