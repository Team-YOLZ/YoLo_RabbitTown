using UnityEngine;
using System;
using LitJson;

[Serializable]
public class PlayerStatData
{
    [Header("UserStatTable_column")]
    public int Attack;
    public float AttackSpeed;
    public float MovingSpeed;
    public int Hp;
    public int Leadership;
    public int Appeal;
    public string OwnerIndate;

    public PlayerStatData(JsonData json) //JSON Data 할당 생성자.  
    {
        this.Attack = int.Parse(json["row"]["Attack"]["N"].ToString());
        this.AttackSpeed = float.Parse(json["row"]["AttackSpeed"]["N"].ToString());
        this.MovingSpeed = float.Parse(json["row"]["MovingSpeed"]["N"].ToString());
        this.Hp = int.Parse(json["row"]["Hp"]["N"].ToString());
        this.Leadership = int.Parse(json["row"]["Leadership"]["N"].ToString());
        this.Appeal = int.Parse(json["row"]["Appeal"]["N"].ToString());
        this.OwnerIndate = json["row"]["owner_inDate"]["S"].ToString();
    }
}

[Serializable]
public class PlayerAssetData
{
    [Header("UserAssetTable_column")]
    public int Coin;
    public int Spoil1;
    public int Spoil2;
    public int Spoil3;
    public int Spoil4;

    public PlayerAssetData(JsonData json)
    {
        this.Coin = int.Parse(json["row"]["Coin"]["N"].ToString());
        this.Spoil1 = int.Parse(json["row"]["Spoil1"]["N"].ToString());
        this.Spoil2 = int.Parse(json["row"]["Spoil2"]["N"].ToString());
        //this.Spoil3 = int.Parse(json["row"]["Spoil3"]["N"].ToString());
        //this.Spoil4 = int.Parse(json["row"]["Spoil4"]["N"].ToString());
    }
}

[Serializable]
public class PlayerKillData
{
    [Header("UserCaputerCount_clumn")]
    public int Chicken1;
    public int Chicken2;
    public int Chicken3;
    public int Cow1;
    public int Cow2;
    public int Cow3;
    public int Horse1;
    public int Horse2;
    public int Horse3;
    public int Goat1;
    public int Goat2;
    public int Duck1;
    public int Duck2;
    public int Duck3;
    public int Sheep1;
    public int Sheep2;

    public PlayerKillData(JsonData json)
    {
        this.Chicken1 = int.Parse(json["row"]["Chicken1"]["N"].ToString());
        this.Chicken2 = int.Parse(json["row"]["Chicken2"]["N"].ToString());
        this.Chicken3 = int.Parse(json["row"]["Chicken3"]["N"].ToString());
        this.Cow1 = int.Parse(json["row"]["Cow1"]["N"].ToString());
        this.Cow2 = int.Parse(json["row"]["Cow2"]["N"].ToString());
        this.Cow3 = int.Parse(json["row"]["Cow3"]["N"].ToString());
        this.Duck1 = int.Parse(json["row"]["Duck1"]["N"].ToString());
        this.Duck2 = int.Parse(json["row"]["Duck2"]["N"].ToString());
        this.Duck3 = int.Parse(json["row"]["Duck3"]["N"].ToString());
        this.Horse1 = int.Parse(json["row"]["Horse1"]["N"].ToString());
        this.Horse2 = int.Parse(json["row"]["Horse2"]["N"].ToString());
        this.Horse3 = int.Parse(json["row"]["Horse3"]["N"].ToString());
        this.Goat1 = int.Parse(json["row"]["Goat1"]["N"].ToString());
        this.Goat2 = int.Parse(json["row"]["Goat2"]["N"].ToString());
        this.Sheep1 = int.Parse(json["row"]["Sheep1"]["N"].ToString());
        this.Sheep2 = int.Parse(json["row"]["Sheep2"]["N"].ToString());

    }
}
