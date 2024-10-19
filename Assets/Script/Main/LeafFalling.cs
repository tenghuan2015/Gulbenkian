using System.Collections;
using TMPro;
using UnityEngine;
// using TMPro;

public class LeafController : MonoBehaviour
{
    // 单例实例
   public static LeafController Instance;

    public float windForce = 2f; // 基础风力
    public float maxWindForce = 5f; // 最大风力（由鼠标拖拽决定）
    public float fallSpeed = 0.5f; // 叶子下落速度
    public float rotationSpeed = 30f; // 叶子旋转速度
    public float horizontalWobbleAmount = 2f; // 水平摆动幅度
    public float verticalWobbleAmount = 0.5f; // 垂直摆动幅度
    public float windRandomness = 2f; // 风力的随机性
    // public  TMP_Text number;  // 显示随机数字的Text组件
    public GameObject windEffect;
    public GameObject blackEffect;
    public GameObject whiteEffect;
    
    private Vector3 windDirection; // 风的方向
    private Vector3 dragStartPos; // 鼠标拖拽起始点
    private bool isDragging = false; // 标记是否正在拖拽
    private float lifeTime = 0f; // 控制叶子生命周期的时间变量
    private Rigidbody rb; // 叶子的刚体组件
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // 初始随机风向
        windDirection = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized; // 随机初始化风向
         windEffect.SetActive(true);
         blackEffect.SetActive(false);
         whiteEffect.SetActive(false);
    }

    void Update()
    {
        // 更新风力方向和运动
        UpdateWind();
        // 更新叶子的运动轨迹
        UpdateLeafMovement();
        // 更新叶子的摆动和旋转
        UpdateLeafWobbleAndRotation();
    }

    // 更新风力，根据鼠标拖拽调整风向
    void UpdateWind()
    {
        if (Input.GetMouseButtonDown(0)) // 鼠标左键按下时，开始拖拽
        {
            dragStartPos = Input.mousePosition; // 记录鼠标拖拽的起点
            isDragging = true;
        }

        if (Input.GetMouseButton(0) && isDragging) // 当鼠标按住并拖拽时
        {
            Vector3 dragEndPos = Input.mousePosition; // 获取拖拽的终点
            Vector3 dragVector = dragEndPos - dragStartPos; // 计算拖拽向量（表示风向）

            // 根据拖拽向量确定风向，仅在X和Z轴上
            windDirection = new Vector3(dragVector.x, 0, dragVector.y).normalized; // Y轴保持为0

            // 根据拖拽的距离计算风力大小，越长距离风力越大
            float dragDistance = dragVector.magnitude;
            windForce = Mathf.Clamp(dragDistance / 5f, 0f, maxWindForce); // 控制风力最大值
        }

        if (Input.GetMouseButtonUp(0)) // 松开鼠标左键，停止拖拽
        {
            isDragging = false; // 停止拖拽
            windForce = 2f; // 恢复到默认的基础风力
        }
    }

    // 更新叶子的运动
    void UpdateLeafMovement()
    {
        // Perlin噪声模拟随机风力变化
        float windX = Mathf.PerlinNoise(Time.time, lifeTime) * 2 - 1;
        float windZ = Mathf.PerlinNoise(Time.time + 1, lifeTime) * 2 - 1; // 使用 Perlin 噪声更新 Z轴风力

        Vector3 windVariation = new Vector3(windX, 0, windZ) * windRandomness;

        // 叶子在风力和随机风力的作用下运动
        Vector3 movement = Vector3.down * fallSpeed + (windDirection + windVariation) * windForce;
        transform.position += new Vector3(movement.x, 0, movement.z) * Time.deltaTime; // 确保只在X和Z轴移动
    }

    // 模拟叶子摆动和旋转
    void UpdateLeafWobbleAndRotation()
    {
        // 水平摆动和垂直摆动
        float wobbleX = Mathf.Sin(lifeTime * 2f) * horizontalWobbleAmount;
        float wobbleY = Mathf.Cos(lifeTime * 3f) * verticalWobbleAmount;
        transform.position += new Vector3(wobbleX, 0, wobbleY) * Time.deltaTime; // 只在X和Z轴上摆动

        // 叶子旋转
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime); // 旋转
        // 更新生命周期时间
        lifeTime += Time.deltaTime;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Pond"))
        {
            StopMotion();
            FromWindToWhite();
            GameManager.Instance.targetNumber += 1;
            
            GameManager.Instance.DestroyLeaf(gameObject);

           
            
            Debug.Log(" collision with pond" );
        }
        else if (collision.gameObject.CompareTag("Ground"))
        {
            StopMotion();
            FromWindToBlack();
            GameManager.Instance.targetNumber -= 1;
           
            GameManager.Instance.DestroyLeaf(gameObject);
          
           
            Debug.Log(" collision with Ground");
        }
        // GameManager.Instance.CheckScore();
        
         

    }
       public void FromWindToBlack()
    {   Debug.Log("运行中");
        if (windEffect != null)
        {
            // 关闭指定的子物体
            windEffect.SetActive(false);
             Debug.Log("关了");
        }

        if (blackEffect != null)
        {
            // 开启另一个子物体
            blackEffect.SetActive(true);
            Debug.Log("开了");
        }
    }
    public void FromWindToWhite()
    {   Debug.Log("运行中");
        if (windEffect != null)
        {
            // 关闭指定的子物体
            windEffect.SetActive(false);
             Debug.Log("关了");
        }

        if (blackEffect != null)
        {
            // 开启另一个子物体
            whiteEffect.SetActive(true);
            Debug.Log("开了白色");
        }
    }
  public void StopMotion()
    {
        // 停止Rigidbody的物理运动
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero; // 停止速度
            rb.angularVelocity = Vector3.zero; // 停止旋转速度
            rb.isKinematic = true; // 将物体设置为运动学模式，停止受力影响
        }

        // 停用当前脚本，防止它继续更新
        this.enabled = false; 
    }
}
