using UnityEngine;
using System;
using static Define;

[CreateAssetMenu]
public class UnitData : ScriptableObject
{
    public GameObject UnitPrefab;
    public GameObject Projecttile;
    public Level_UnitStat[] Level;

    [Serializable]
    public struct Level_UnitStat
    {
        public string Name;
        public MeadowUnit meadowUnit;
        public int Attack;  // 공격력
        public int AttackSpeed;    // 공격속도
        public int MovingSpeed; //이동속도
        public int Hp; //체력
        public int Level; //레벨
        public int range;   // 사거리
        public int DropCoin;    //죽으면 죽는 돈.
        public int Spoilnumber; // 죽으면 주는 전리품 넘버.
        public int SpoilAmount; //죽으면 주는 전리품 양.
        public int CanCapturePercent; //죽었을 시 포획 가능 확률.
        public int DetectionRange; //적 발견 사거리
        public int PlayerRange; //플레이어와 동료 사거리
        public int PlayerRangeWellOff; // 적 공격 시 플레이어와 동료 거리 널널하게 하기 위한
    }
}
