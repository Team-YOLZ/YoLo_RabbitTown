using UnityEngine;
using UnityEngine.UI;
using BackEnd;

public class MainScene : BaseScene
{
    //BackEndGetTable 싱글톤 보장 받기위한 Static 변수.
    static BackEndGetTable s_instance; //유일성이 보장된다.
    static BackEndGetTable Instance { get { TableInit(); return s_instance; } }

    [Header ("UserStatText")]
    [SerializeField] private Text Attack;
    [SerializeField] private Text AttackSpeed;
    [SerializeField] private Text MovingSpeed;
    [SerializeField] private Text Hp;
    [SerializeField] private Text Leadership;
    [SerializeField] private Text Appeal;

    [Header("UserAssetText")]
    [SerializeField] private Text Coin;
    [SerializeField] private Text Spoil1;
    [SerializeField] private Text Spoil2;
    [SerializeField] private Text Spoil3;
    [SerializeField] private Text Spoil4;

    void Start()
    {
        Init();
        TableInit();
        EditPage();
    }
    static void TableInit() // 유저 DB정보 싱글톤 할당.
    {
        if (s_instance == null)
        {
            GameObject go = GameObject.Find("UserTableInformation");
            if (go == null)
            {
                go = new GameObject { name = "UserTableInformation" };
                go.AddComponent<BackEndGetTable>();
            }
            DontDestroyOnLoad(go);
            s_instance = go.GetComponent<BackEndGetTable>();
        }
    }

    protected override void Init()
    {
        base.Init();
        SceneType = Define.Scene.Main;
    }

    public override void Clear()
    {
        Debug.Log("MainScene Clear");
    }

    public void OnClickGoGameScene()
    {
        Managers.Scene.LoadScene(Define.Scene.Game);
    }
    public void OnClickUPAttack()
    {
        s_instance.UpAttackButton(); // 싱글톤 테이블 instance.공격력Up 함수.
        EditPage(); //페이지 재할당.
    }

    public void EditPage()
    {
        //Stat Text 할당.
        Attack.text = s_instance.playerStat.Attack.ToString();
        AttackSpeed.text = s_instance.playerStat.AttackSpeed.ToString();
        MovingSpeed.text = s_instance.playerStat.MovingSpeed.ToString();
        Hp.text = s_instance.playerStat.Hp.ToString();
        Leadership.text = s_instance.playerStat.Leadership.ToString();
        Appeal.text = s_instance.playerStat.Appeal.ToString();

        //Asset Text 할당.
        Coin.text = s_instance.playerAsset.Coin.ToString();
        Spoil1.text = s_instance.playerAsset.Spoil1.ToString();
        Spoil2.text = s_instance.playerAsset.Spoil2.ToString();
        Spoil3.text = s_instance.playerAsset.Spoil3.ToString();
        Spoil4.text = s_instance.playerAsset.Spoil4.ToString();
    }
}
