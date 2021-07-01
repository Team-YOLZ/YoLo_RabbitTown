using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BackEnd;

public class MainScene : BaseScene
{
    public BackEndGetTable GetTable;

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
        GetTable = GameObject.Find("UserTableInformation").GetComponent<BackEndGetTable>();
        EditPage();
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

    public void EditPage()
    {
        //Stat Text 할당.
        Attack.text = GetTable.playerStat.Attack.ToString();
        AttackSpeed.text = GetTable.playerStat.AttackSpeed.ToString();
        MovingSpeed.text = GetTable.playerStat.MovingSpeed.ToString();
        Hp.text = GetTable.playerStat.Hp.ToString();
        Leadership.text = GetTable.playerStat.Leadership.ToString();
        Appeal.text = GetTable.playerStat.Appeal.ToString();

        //Asset Text 할당.
        Coin.text = GetTable.playerAsset.Coin.ToString();
        Spoil1.text = GetTable.playerAsset.Spoil1.ToString();
        Spoil2.text = GetTable.playerAsset.Spoil2.ToString();
        Spoil3.text = GetTable.playerAsset.Spoil3.ToString();
        Spoil4.text = GetTable.playerAsset.Spoil4.ToString();
    }
}
