using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PondGameController : MonoBehaviour
{
    public GameObject leaf; // 叶子对象
    public GameObject obstaclePrefab; // 障碍物预制体
    private Vector3 poolCenter; // 池塘中心位置
    private float windDirection; // 风的方向
    private float timeInCenter = 5f; // 叶子在中心的时间
     private Vector3 halfSize; // 池塘一半尺寸
     public List<GameObject> currentObstacles = new List<GameObject>();
      private float edgeTimer = 0f;
    private bool generatingObstacles = false;


    void Start()
    {
        // 假设池塘是一个GameObject
    GameObject pondObject = GameObject.FindGameObjectWithTag("Pond");
    if (pondObject != null)
    {
        poolCenter = pondObject.transform.position;
        halfSize = pondObject.transform.localScale / 2f;
        Debug.Log(halfSize);
    }
    else
    {
        Debug.LogError("未找到带有 'Pond' 标签的游戏对象！");
    }

    // 将叶子放在池塘边缘
    leaf.transform.position = new Vector3(1, 0, 0);
    }

    void Update()
    {
        
         // 检查叶子是否在池塘内
    var (newPosition, isOnEdge) = SnapToPondBoundary(leaf.transform.position);

    // 更新叶子位置
    leaf.transform.position = newPosition;

    // 根据是否在边缘进行其他操作
     if (isOnEdge)
        {
            // 处理叶子在边缘的情况
            Debug.Log("Leaf is on the edge.");
            edgeTimer += Time.deltaTime;

            // 达到两秒后开始生成障碍物
            if (edgeTimer >= 1f && !generatingObstacles)
            {
                generatingObstacles = true;
                StartCoroutine(GenerateObstaclesEverySecond());
            }
        }
        else
        {
            edgeTimer = 0f;
            generatingObstacles = false;
        }

        
        // 控制风向
    float windX = Input.mousePosition.x / Screen.width * 2 - 1;
    float windZ = Input.mousePosition.y / Screen.height * 2 - 1;
    Vector3 windDirection = new Vector3(windX, 0, windZ).normalized;

    // // 生成随机方向
    // Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;

    // 计算叶子的移动速度
    Vector3 moveDirection = (poolCenter - leaf.transform.position).normalized;

    // 随机方向
    Vector3 combinedDirection = (windDirection * moveDirection.magnitude).normalized;

    // 更新叶子位置
    leaf.transform.Translate(combinedDirection * Time.deltaTime * 2f);

        
        // 检测叶子是否在中心
        if (Vector3.Distance(leaf.transform.position, poolCenter) < 1)
        {
            timeInCenter += Time.deltaTime;
            if (timeInCenter > 5f)
            {
                // 触发下雨效果
                TriggerRain();
            }
        }
        else
        {
            timeInCenter = 0f;
        }

    
    
    }

    void TriggerRain()
    {
        // 实现下雨效果
        Debug.Log("It's raining!");
    }
  IEnumerator GenerateObstaclesEverySecond()
    {
        while (generatingObstacles)
        {
            GenerateObstacle();
            yield return new WaitForSeconds(0.5f);
        }
    }
      void GenerateObstacle()
    {
        // 检查当前场景内的障碍物数量
        if (currentObstacles.Count >= 20)
        {
            Debug.Log("Maximum number of obstacles reached.");
            return;
        }

        // 生成随机位置
        Vector3 spawnPosition = new Vector3(
            Random.Range(poolCenter.x - halfSize.x, poolCenter.x + halfSize.x),
            10f, // 障碍物生成的高度
            Random.Range(poolCenter.z - halfSize.z, poolCenter.z + halfSize.z)
        );

        // 确保生成位置在池塘范围内
        (Vector3 adjustedPosition, bool isValid) = SnapToPondBoundary(spawnPosition);

        if (isValid)
        {
            GameObject obstacle = Instantiate(obstaclePrefab, adjustedPosition, Quaternion.identity);
            currentObstacles.Add(obstacle);
            StartCoroutine(SinkAndDestroyObstacle(obstacle));
        }
    }


    IEnumerator SinkAndDestroyObstacle(GameObject obstacle)
    {
        // 下沉到水面高度（例如 y=0）
        while (obstacle.transform.position.y > 0)
        {
            //obstacle.transform.Translate(Vector3.down * Time.deltaTime * 10f);
            yield return null;
        }

        // 等待30秒后再销毁障碍物
        yield return new WaitForSeconds(30f);
        Destroy(obstacle);
    }

    
    bool IsInsidePond(Vector3 position)
{
    // 检查位置是否在池塘内部
    float radius = halfSize.x; // 假设池塘是正圆，半径等于 halfSize.x
    float buffer = 0.5f; // 缩小检测范围的缓冲值
    float adjustedRadius = radius - buffer;

    // 计算位置与池塘中心的距离
    float distance = Vector3.Distance(position, poolCenter);

    return distance <= adjustedRadius;
}

(Vector3, bool) SnapToPondBoundary(Vector3 position)
{
    // 检查位置是否在池塘内部
    if (!IsInsidePond(position))
    {
        // 计算位置与池塘中心的距离
        float distance = Vector3.Distance(position, poolCenter);
        float radius = halfSize.x; // 假设池塘是正圆，半径等于 halfSize.x
        float buffer = 0.5f; // 缓冲值

        // 将位置拉回到池塘边界以内
        Vector3 direction = (position - poolCenter).normalized;
        position = poolCenter + direction * (radius - buffer);

        // 返回调整后的位置和是否在边缘的标志
        return (position, true); // 表示在边缘
    }

    // 返回调整后的位置和是否在边缘的标志
    return (position, false); // 表示不在边缘
}
}