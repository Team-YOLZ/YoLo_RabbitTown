using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//하이라키창에서 어떻게 보이는지?
/*@EnemyPool      (최고레벨 루트) 
 *   enemy1_Root  (유닛 별 들어가게 될 빈 오브젝트)
 *      enemy1    (유닛) 
 *      enemy1    (유닛)
 *   enemy2_Root
 *      enemy2
 *      enemy2
 *      .
 *      .
 *      .
 */
public class EnemyPool : MonoBehaviour
{
    #region Pool
    class Pool
    {
        public Transform Root { get; set; } //유닛 별 들어가게 될 빈 오브젝트의 Transform

        Queue<EnemyCtrl> _queue = new Queue<EnemyCtrl>();

        public void Init(string enemyName, int initCount)
        {
            Root = new GameObject().transform;
            Root.name = $"{enemyName}_Root";  

            for (int i = 0; i < initCount; i++)
                _queue.Enqueue(CreateNewObject(enemyName));
        }

        //실질적으로 유닛을 생성하는 곳 생성과 동시에 SetActive(false), SetParent 설정
        private EnemyCtrl CreateNewObject(string enemyName)
        {
            var newObj = Managers.Resource.Instantiate($"Creature_YJ/{enemyName}").GetComponent<EnemyCtrl>();
            newObj.gameObject.SetActive(false);
            newObj.transform.SetParent(Root);
            return newObj;
        }

        public EnemyCtrl GetObject(string enemyName)
        {
            if (_queue.Count > 0)
            {
                var obj = _queue.Dequeue();
                obj.transform.SetParent(Root);
                obj.gameObject.SetActive(true);
                return obj;
            }
            else //큐 안에 더 이상 꺼낼게 없다면 새로 생성
            {
                var newObj = CreateNewObject(enemyName);
                newObj.gameObject.SetActive(true);
                newObj.transform.SetParent(Root);
                return newObj;
            }
        }

        public void ReturnObject(EnemyCtrl obj)
        {
            obj.gameObject.SetActive(false);
            obj.transform.SetParent(Root);
            _queue.Enqueue(obj);
        }
    }
    #endregion

    Dictionary<string, Pool> _pool = new Dictionary<string, Pool>();
    Transform _root;  //최상의 레벨인 @EnemyPool의 transform

    public static EnemyPool _instance; //싱글톤
    public static EnemyPool Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = GameObject.Find("@EnemyPool");
                if (go == null)
                {
                    go = new GameObject { name = "@EnemyPool" };
                    go.AddComponent<EnemyPool>();
                }
                _instance = go.GetComponent<EnemyPool>();
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_root == null)
            _root = Instance.gameObject.transform;
    }

    //pool객체 생성 후 Init()으로 count 수 만큼 enemy 생성 후, pool객체 Dictionary에 Add
    public static void CreatePool(string enemyName, int count =10)
    {
        Pool pool = new Pool();
        pool.Init(enemyName, count);
        pool.Root.SetParent(Instance._root);

        Instance._pool.Add(enemyName, pool);
    }

    public static EnemyCtrl GetObject(string enemyName)
    {
        if (Instance._pool.ContainsKey(enemyName) == false) //enemy가 처음 만들어 질 때
            CreatePool(enemyName);

        return Instance._pool[enemyName].GetObject(enemyName);
    }

    public static void ReturnObject(EnemyCtrl enemyCtrl)
    {
        string enemyName = enemyCtrl.gameObject.name;
        //혹시라도 enemy가 만들어진지않은 상태에서 ReturnObject를 하면(이럴 경우가 거의 없겠지만)
        if (Instance._pool.ContainsKey(enemyName) == false) 
        {
            Destroy(enemyCtrl.gameObject); //바로 삭제 후 리턴
            return;
        }

        Instance._pool[enemyName].ReturnObject(enemyCtrl);
    }

    public static void Clear()
    {
        foreach (Transform child in Instance._root)
            GameObject.Destroy(child.gameObject);

        Instance._pool.Clear();
    }
}
