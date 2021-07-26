using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawning : MonoBehaviour
{
    [Header("Player's Range")]
    [SerializeField]
    GameObject _player;
    [SerializeField]
    float _spawnRange; //250
    [SerializeField]
    float _destroyRange; //400

    /* 맵을 크게 9구간으로 나뉨
     *  ㅁㅁㅁ     1 2 3
     *  ㅁㅁㅁ ->  4 5 6
     *  ㅁㅁㅁ     7 8 9 
     */
    [Header("Section1")] //1성급 몬스터가 나오는 구간
    [SerializeField]
    GameObject _section1;

    [Header("Section2")] //2성급 몬스터가 나오는 구간
    [SerializeField]
    GameObject _section2;
    [SerializeField]
    GameObject _section4;
    [SerializeField]
    GameObject _section5;

    [Header("Section3")] //3성급 몬스터가 나오는 구간
    [SerializeField]
    GameObject _section3;
    [SerializeField]
    GameObject _section6;
    [SerializeField]
    GameObject _section7;
    [SerializeField]
    GameObject _section8;
    [SerializeField]
    GameObject _section9;

    int _maxEnemyCount; //구간 별 최대 나올 수 있는 몬스터 그룹 수
    int _randomEnemyCount; //뭉쳐다니는 적들의 수
    int _layerMask; //필드레이어
    RaycastHit _hit;


    //최대 나올 수 있는 몬스터 그룹 수 어떻게 ??
    //_spawnRange의 범위가 1개 ~ 4개의 section을 포함할 수도 있잖아?
    
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
