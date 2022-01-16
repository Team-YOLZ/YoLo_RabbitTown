using UnityEngine;
using UnityEngine.UI;
using BackEnd;
using DG.Tweening;

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

    [Header("UserUnitKillCountText")]
    [SerializeField] private Text Chicken1_Kill;
    [SerializeField] private Text Chicken2_Kill;
    [SerializeField] private Text Chicken3_Kill;
    [SerializeField] private Text Cow1_Kill;
    [SerializeField] private Text Cow2_Kill;
    [SerializeField] private Text Cow3_Kill;
    [SerializeField] private Text Duck1_Kill;
    [SerializeField] private Text Duck2_Kill;
    [SerializeField] private Text Duck3_Kill;
    [SerializeField] private Text Horse1_Kill;
    [SerializeField] private Text Horse2_Kill;
    [SerializeField] private Text Horse3_Kill;
    [SerializeField] private Text Goat1_Kill;
    [SerializeField] private Text Goat2_Kill;
    [SerializeField] private Text Sheep1_Kill;
    [SerializeField] private Text Sheep2_Kill;

    [Header("UserUnitLevelText")]
    [SerializeField] private Text Chicken1_Level;
    [SerializeField] private Text Chicken2_Level;
    [SerializeField] private Text Chicken3_Level;
    [SerializeField] private Text Cow1_Level;
    [SerializeField] private Text Cow2_Level;
    [SerializeField] private Text Cow3_Level;
    [SerializeField] private Text Duck1_Level;
    [SerializeField] private Text Duck2_Level;
    [SerializeField] private Text Duck3_Level;
    [SerializeField] private Text Horse1_Level;
    [SerializeField] private Text Horse2_Level;
    [SerializeField] private Text Horse3_Level;
    [SerializeField] private Text Goat1_Level;
    [SerializeField] private Text Goat2_Level;
    [SerializeField] private Text Sheep1_Level;
    [SerializeField] private Text Sheep2_Level;

    [Header("UnitPage")]
    [SerializeField] private GameObject UnitPage1;
    [SerializeField] private GameObject UnitPage2;
    [SerializeField] private GameObject UnitPage3;
    [SerializeField] private GameObject UnitPage1_Button;
    [SerializeField] private GameObject UnitPage2_Button;
    [SerializeField] private GameObject UnitPage3_Button;

    [Header("PageMask")]
    [SerializeField] private RectTransform StatPage;
    [SerializeField] private RectTransform AssetPage;
    [SerializeField] private RectTransform UnitPage;
    private bool isStatPageOpen;
    private bool isAssetPageOpen;
    private bool isUnitPageOpen;

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

    public void OnClickUpAttackSpeed()
    {
        s_instance.UpAttackSpeedButton();
        EditPage();
    }

    public void OnClickUPMoveSpeed()
    {
        s_instance.UpMoveSpeedButton();
        EditPage();
    }

    public void OnClickUPHP()
    {
        s_instance.UpHPButton();
        EditPage();
    }

    public void OnClickUPLeaderShip()
    {
        s_instance.UpLeadershipButton();
        EditPage();
    }

    public void OnClickUPAppeal()
    {
        s_instance.UpAppealButton();
        EditPage();
    }

    public void EditPage()
    {
        //Stat Text 할당.
        Attack.text = s_instance.playerStat.Attack.ToString();
        AttackSpeed.text = string.Format("{0:F1}", s_instance.playerStat.AttackSpeed);  //s_instance.playerStat.AttackSpeed.ToString();
        MovingSpeed.text = string.Format("{0:F1}", s_instance.playerStat.MovingSpeed);  //s_instance.playerStat.MovingSpeed.ToString();
        Hp.text = s_instance.playerStat.Hp.ToString();
        Leadership.text = s_instance.playerStat.Leadership.ToString();
        Appeal.text = s_instance.playerStat.Appeal.ToString();

        //Asset Text 할당.
        Coin.text = s_instance.playerAsset.Coin.ToString();
        Spoil1.text = s_instance.playerAsset.Spoil1.ToString();
        Spoil2.text = s_instance.playerAsset.Spoil2.ToString();

        //Unit Kill Count Text 할당.
        Chicken1_Kill.text = s_instance.playerKillData.Chicken1.ToString();
        Chicken2_Kill.text = s_instance.playerKillData.Chicken2.ToString();
        Chicken3_Kill.text = s_instance.playerKillData.Chicken3.ToString();
        Cow1_Kill.text = s_instance.playerKillData.Cow1.ToString();
        Cow2_Kill.text = s_instance.playerKillData.Cow2.ToString();
        Cow3_Kill.text = s_instance.playerKillData.Cow3.ToString();
        Duck1_Kill.text = s_instance.playerKillData.Duck1.ToString();
        Duck2_Kill.text = s_instance.playerKillData.Duck2.ToString();
        Duck3_Kill.text = s_instance.playerKillData.Duck3.ToString();
        Horse1_Kill.text = s_instance.playerKillData.Horse1.ToString();
        Horse2_Kill.text = s_instance.playerKillData.Horse2.ToString();
        Horse3_Kill.text = s_instance.playerKillData.Horse3.ToString();
        Goat1_Kill.text = s_instance.playerKillData.Goat1.ToString();
        Goat2_Kill.text = s_instance.playerKillData.Goat2.ToString();
        Sheep1_Kill.text = s_instance.playerKillData.Sheep1.ToString();
        Sheep2_Kill.text = s_instance.playerKillData.Sheep2.ToString();

        //Unit Level Text 할당.
        Chicken1_Level.text = Leveling(s_instance.playerKillData.Chicken1).ToString();
        Chicken2_Level.text = Leveling(s_instance.playerKillData.Chicken2).ToString();
        Chicken3_Level.text = Leveling(s_instance.playerKillData.Chicken3).ToString();
        Cow1_Level.text = Leveling(s_instance.playerKillData.Cow1).ToString();
        Cow2_Level.text = Leveling(s_instance.playerKillData.Cow2).ToString();
        Cow3_Level.text = Leveling(s_instance.playerKillData.Cow3).ToString();
        Duck1_Level.text = Leveling(s_instance.playerKillData.Duck1).ToString();
        Duck2_Level.text = Leveling(s_instance.playerKillData.Duck2).ToString();
        Duck3_Level.text = Leveling(s_instance.playerKillData.Duck3).ToString();
        Horse1_Level.text = Leveling(s_instance.playerKillData.Horse1).ToString();
        Horse2_Level.text = Leveling(s_instance.playerKillData.Horse2).ToString();
        Horse3_Level.text = Leveling(s_instance.playerKillData.Horse3).ToString();
        Goat1_Level.text = Leveling(s_instance.playerKillData.Goat1).ToString();
        Goat2_Level.text = Leveling(s_instance.playerKillData.Goat2).ToString();
        Sheep1_Level.text = Leveling(s_instance.playerKillData.Sheep1).ToString();
        Sheep2_Level.text = Leveling(s_instance.playerKillData.Sheep2).ToString();


    }

    public void OpenStatPage()
    {
        if (!isStatPageOpen)
        {
            StatPage.DOAnchorPosX(300f, 0.2f);
            isStatPageOpen = true;
        }
        else
        {
            StatPage.DOAnchorPosX(-300f, 0.2f);
            isStatPageOpen = false;
        }
    }

    public void OpenAssetPage()
    {
        if (!isAssetPageOpen)
        {
            if (isUnitPageOpen)
            {
                UnitPage1_Button.SetActive(false);
                UnitPage2_Button.SetActive(false);
                UnitPage3_Button.SetActive(false);
                UnitPage.DOAnchorPosX(600f, 0.2f);
                isUnitPageOpen = false;
            }
            AssetPage.DOAnchorPosX(0f, 0.2f);
            isAssetPageOpen = true;
        }
        else
        {
            AssetPage.DOAnchorPosX(300f, 0.2f);
            isAssetPageOpen = false;
        }
    }

    public void OpenUnitPage()
    {
        if (!isUnitPageOpen)
        {
            if (isAssetPageOpen)
            {
                AssetPage.DOAnchorPosX(300f, 0.2f);
                isAssetPageOpen = false;
            }
            SwitchUnitPage1();
            UnitPage1_Button.SetActive(true);
            UnitPage2_Button.SetActive(true);
            UnitPage3_Button.SetActive(true);

            UnitPage.DOAnchorPosX(0f, 0.2f);
            isUnitPageOpen = true;
        }
        else
        {
            UnitPage1_Button.SetActive(false);
            UnitPage2_Button.SetActive(false);
            UnitPage3_Button.SetActive(false);
            UnitPage.DOAnchorPosX(600f, 0.2f);
            isUnitPageOpen = false;
        }
    }

    public void SwitchUnitPage1()
    {
        UnitPage1.SetActive(true);
        UnitPage2.SetActive(false);
        UnitPage3.SetActive(false);
    }
    public void SwitchUnitPage2()
    {
        UnitPage1.SetActive(false);
        UnitPage2.SetActive(true);
        UnitPage3.SetActive(false);
    }
    public void SwitchUnitPage3()
    {
        UnitPage1.SetActive(false);
        UnitPage2.SetActive(false);
        UnitPage3.SetActive(true);
    }

    //30 60 90 120 
    private int Leveling(int kill)
    {
        if (kill > 119)
            return 5;
        else if (kill > 89)
            return 4;
        else if (kill > 59)
            return 3;
        else if (kill > 29)
            return 2;

        return 1;
    }

}
