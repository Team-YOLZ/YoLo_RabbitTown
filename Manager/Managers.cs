using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    static Managers s_instance; //유일성이 보장된다.
    static Managers Instance { get { Init(); return s_instance; } }

    InputManager _input = new InputManager();
    public static InputManager Input { get { return Instance._input; } }

    ResourceManager _resource = new ResourceManager();
    public static ResourceManager Resource { get { return Instance._resource; } }

    //UIManager _ui = new UIManager();
    //public static UIManager UI { get { return Instance._ui; } }

    SceneManagerEx _scene = new SceneManagerEx();
    public static SceneManagerEx Scene { get { return Instance._scene; } }

    SoundManager _sound = new SoundManager();
    public static SoundManager Sound { get { return Instance._sound; } }

    PoolManager _pool = new PoolManager();
    public static PoolManager Pool { get { return Instance._pool; } }

    GameManager _game = new GameManager();
    public static GameManager Game { get { return Instance._game; } }

    NetworkManger _network =  new NetworkManger();
    public static NetworkManger Network { get { return Instance._network; } }

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        _input.OnUpdate();
    }
    static void Init()
    {
        if (s_instance == null)
        {
            GameObject go = GameObject.Find("@Managers");
            if (go == null)
            {
                go = new GameObject { name = "@Managers" };
                go.AddComponent<Managers>();
            }
            DontDestroyOnLoad(go);
            s_instance = go.GetComponent<Managers>();

            s_instance._pool.Init();
            s_instance._sound.Init();
        }
    }

    public static void Clear() //날려줘야할 것들
    {
        Sound.Clear();
        Input.Clear();
        Scene.Clear();
        //UI.Clear();

        Pool.Clear();
    }

}
