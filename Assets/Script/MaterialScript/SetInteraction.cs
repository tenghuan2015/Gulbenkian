using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 该类用于在Unity引擎中设置相机与着色器之间的交互。
/// 它允许相机动态更新着色器参数以实现实时渲染效果。
/// </summary>
public class SetInteraction : MonoBehaviour
{
    // 用于着色器的渲染纹理，在Unity编辑器中可赋值
    [SerializeField] RenderTexture rt;
    // 要跟踪位置的目标对象，在Unity编辑器中可赋值
    [SerializeField] Transform target;

    /// <summary>
    /// 初始化全局着色器设置。
    /// 该方法在MonoBehaviour创建后首次执行Update之前调用一次。
    /// </summary>
    void Awake()
    {
        // 为着色器设置全局渲染纹理
        Shader.SetGlobalTexture("_RenderTexture", rt);
        // 从附加到同一对象的相机组件获取正交大小，并为着色器设置全局正交大小
        Shader.SetGlobalFloat("_OrthographicSize", GetComponent<Camera>().orthographicSize);
    }

    /// <summary>
    /// 更新全局着色器位置参数。
    /// 该方法每帧调用一次。
    /// </summary>
    void Update()
    {
        // 更新此对象的变换位置，使其匹配目标对象的x和z坐标
        transform.position = new Vector3(target.position.x, transform.position.y, target.position.z);
        // 使用此对象更新后的变换位置，为着色器设置全局位置
        Shader.SetGlobalVector("_Position", transform.position);

    }
}