using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using LitJson;

[Serializable]
public class PlayerData
{
    public int Attack;
    public int AttackSpeed;
    public int MovingSpeed;
    public int Hp;
    public int Leadership;
    public int Appeal;

    public PlayerData(JsonData json) //JSON Data 할당 생성자.  
    {
        this.Attack = int.Parse(json["row"]["Attack"]["N"].ToString());
        this.AttackSpeed = int.Parse(json["row"]["AttackSpeed"]["N"].ToString());
        this.MovingSpeed = int.Parse(json["row"]["MovingSpeed"]["N"].ToString());
        this.Hp = int.Parse(json["row"]["Hp"]["N"].ToString());
        this.Leadership = int.Parse(json["row"]["Leadership"]["N"].ToString());
        this.Appeal = int.Parse(json["row"]["Appeal"]["N"].ToString());
    }
}
