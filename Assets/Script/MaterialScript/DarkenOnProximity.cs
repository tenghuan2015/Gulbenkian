using UnityEngine;

public class DarkenOnProximity : MonoBehaviour
{
    public GameObject targetObject; // 目标物体
    public float maxDistance = 5f; // 最大影响距离
    public Color minColor = Color.white; // 最亮的颜色
    public Color maxColor = Color.black; // 最暗的颜色

    private Renderer objectRenderer;

    void Start()
    {
        // 获取物体的渲染器组件
        objectRenderer = GetComponent<Renderer>();
    }

    void Update()
    {
        if (targetObject != null)
        {
            // 计算当前物体与目标物体之间的距离
            float distance = Vector3.Distance(transform.position, targetObject.transform.position);

            // 根据距离计算插值
            float t = Mathf.Clamp01(distance / maxDistance); // t 的值范围在 0 到 1 之间

            // 根据 t 值混合颜色
            Color currentColor = Color.Lerp(maxColor, minColor, t);

            // 更新物体的颜色
            objectRenderer.material.color = currentColor;
        }
    }
}
