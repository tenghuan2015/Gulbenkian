using System.Collections;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject leafPrefab; // 落叶的预制体
    public Transform spawnPoint; // 落叶的生成点
  public  TMP_Text number;  // 显示随机数字的Text组件
   public int targetNumber;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {   
        GetNumber();
        GenerateNewLeaf();
        InvokeRepeating("GenerateNewLeaf", 10.0f, 10.0f);
    }
void Update()
    {  
      GetNumber();
    }  
  
    // 生成新的落叶
    public void GenerateNewLeaf()
    {
        Instantiate(leafPrefab, spawnPoint.position, Quaternion.identity);
    }

    // 销毁落叶的方法
 public void DestroyLeaf(GameObject leaf)
    {
        if (leaf != null)
        {
            StartCoroutine(DestroyLeafAfterDelay(leaf, 5f)); // 停留2秒后销毁叶子
        }
    }

    // 协程：在指定的延迟后销毁叶子
    private IEnumerator DestroyLeafAfterDelay(GameObject leaf, float delay)
    {
        // 等待指定的时间
        yield return new WaitForSeconds(delay);
        Debug.Log("等待中");
        
        // 销毁叶子
        Destroy(leaf);
    }
      public void GetNumber()//在start调用记得 然后碰到pond+1 碰到别的-1
        {
       
        number.text = targetNumber.ToString(); 
        Debug.Log("目标数字: " + targetNumber);
        }
}




// using System.Collections;
// using UnityEngine;
// using TMPro;

// public class GameManager : MonoBehaviour
// {
//     public static GameManager Instance;

//     public GameObject leafPrefab;  // 落叶的预制体
//     public Transform spawnPoint;   // 落叶的生成点
//         public GameObject pond;         // 池塘对象
//     public float leafSpawnInterval = 0.2f; // 落叶生成间隔
//     public TMP_Text randomNumberText;  // 显示随机数字的Text组件

//     public int pondScore = 0;
//     public int groundScore = 0;
//     private int totalScore = 0;
//     private int _leafCount = 0;
    
//     private int targetNumber;

//     void Awake()
//     {
//         if (Instance == null)
//         {
//             Instance = this;
//         }
//     }

//     void Start()
//     {
//         GenerateRandomNumber(); // 生成初始随机数
//         StartCoroutine(SpawnLeafCoroutine()); // 开始生成落叶的协程
//     }

//     // 协程：定期生成新的落叶
//     private IEnumerator SpawnLeafCoroutine()
// {
//    while (true)
//     {
//         for (int i = 0; i < 20 && _leafCount < 20; i++)
//         {
//             GenerateNewLeaf();
//             yield return new WaitForSeconds(leafSpawnInterval);
//         }
//         if (_leafCount >= 20) break; // 当达到20片叶子时退出循环
       
//     }
// }

// // 生成新的落叶
// public void GenerateNewLeaf()
// {
//     Instantiate(leafPrefab, spawnPoint.position, Quaternion.identity);
//     _leafCount++;
// }
//     // 增加池塘分数
//    // 生成一个小于10的随机整数，并显示在UI上
    //  void GenerateRandomNumber()
    // {
    //     targetNumber = Random.Range(1, 10);
    //     randomNumberText.text = targetNumber.ToString();
    //     Debug.Log("目标数字: " + targetNumber);
    // }

//    public void CheckScore()
//     {
        
//        if (totalScore >= 20)
//     {
//         OnScoreReached20();
//     }
//     }

//     // 当总分数达到 20 时的处理逻辑
//   private void OnScoreReached20()
//     {
//         if (totalScore == targetNumber)
//         {
//             Debug.Log("赢了！");
//         }
//         else
//         {
//             Debug.Log("失败了！目标数字是 " + targetNumber + "，而总分数是 " + totalScore);
//         }

//         // 重置分数和叶子
//         ClearAllLeaves();
//         pondScore = 0;
//         groundScore = 0;
//         totalScore = 0;
//         _leafCount = 0;

//         // 生成新的随机数
//         GenerateRandomNumber();

//     }



// public void AddScoreToPond()
// {
//     pondScore += 1;
//     totalScore += 1;
// }

// public void AddScoreToGround()
// {
//     groundScore += 1;
//     totalScore += 1;
// }

// void ClearAllLeaves()
// {
//     GameObject[] leaves = GameObject.FindGameObjectsWithTag("Leaf");
//     foreach (GameObject leaf in leaves)
//     {
//         Destroy(leaf);
//     }
// }
// }
