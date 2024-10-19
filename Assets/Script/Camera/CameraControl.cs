using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float zoomSpeed = 10f;  // 控制缩放的速度
    public float minFOV = 30f;     // 最小视野
    public float maxFOV = 54f;     // 最大视野
    private Camera cam;            // 相机组件

    void Start()
    {
        // 获取相机组件
        cam = Camera.main;

        // 确保初始的FOV在规定范围内
        cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, minFOV, maxFOV);
    }

    void Update()
    {
        HandleZoom();
    }

    void HandleZoom()
    {
        // 获取鼠标滚轮的输入
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        // 根据滚轮的输入调整FOV
        cam.fieldOfView -= scroll * zoomSpeed;

        // 限制FOV的范围在minFOV和maxFOV之间
        cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, minFOV, maxFOV);
    }
}
