using UnityEngine;
using System;
using LitJson;

[Serializable]
public class PlayerStatData
{
    [Header("UserStatTable_column")]
    public int Attack;
    public int AttackSpeed;
    public int MovingSpeed;
    public int Hp;
    public int Leadership;
    public int Appeal;

    public PlayerStatData(JsonData json) //JSON Data 할당 생성자.  
    {
        this.Attack = int.Parse(json["row"]["Attack"]["N"].ToString());
        this.AttackSpeed = int.Parse(json["row"]["AttackSpeed"]["N"].ToString());
        this.MovingSpeed = int.Parse(json["row"]["MovingSpeed"]["N"].ToString());
        this.Hp = int.Parse(json["row"]["Hp"]["N"].ToString());
        this.Leadership = int.Parse(json["row"]["Leadership"]["N"].ToString());
        this.Appeal = int.Parse(json["row"]["Appeal"]["N"].ToString());
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
        this.Spoil3 = int.Parse(json["row"]["Spoil3"]["N"].ToString());
        this.Spoil4 = int.Parse(json["row"]["Spoil4"]["N"].ToString());
    }
}
