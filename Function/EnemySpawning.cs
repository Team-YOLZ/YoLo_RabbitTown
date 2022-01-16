using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

//Player 컴포넌트에 붙이자
public class EnemySpawning : MonoBehaviour
{
    [Header("Player's Range")]
    [SerializeField]
    GameObject _player;
    [SerializeField]
    float _spawnRange;
    [SerializeField]
    float _destroyMinRange;
    [SerializeField]
    float _destroyMaxRange; //이 범위에 넘어갈 시 디스폰

    [Header("Enemy Count")]
    [SerializeField]
    int _enemyCountSectonStart; // StaringZone
    [SerializeField]
    int _enemyCountSecton1; //플레이어가 1구역에 있을 때 몬스터의 수
    [SerializeField]
    int _enemyCountSecton2;
    [SerializeField]
    int _enemyCountSecton3;

    [Header("Time")]
    [SerializeField]
    float _spawnTime;
    [SerializeField]
    float _deSpawnTime;

    static public int _despawnEnemyCount; //디스폰된 몬스터의 수

    // LayerMask
    int _layerMaskGround;
    int _layerMaskSectionStart;
    int _layerMaskSection1;
    int _layerMaskLevel1; // section1 + sectionStart 레이어 합침 (startingZone과 section1에서 같은 종류의 몬스터가 나오기 때문에)
    int _layerMaskSection2;
    int _layerMaskSection3;
    int _layerMaskEnemy;

    // 몬스터 생성 좌표가 어떤 구역에 있는지 확인하기 위함
    bool _sectionStart;
    bool _section1;
    bool _section2;
    bool _section3;

    int _curSection; //플레이어의 현재 위치
    int _previousSection; //플레이어의 지난 위치


    void Awake()
    {
        _layerMaskGround = 1 << LayerMask.NameToLayer("Section");
        _layerMaskSectionStart = 1 << LayerMask.NameToLayer("SectionStarting");
        _layerMaskSection1 = 1 << LayerMask.NameToLayer("Section1");
        _layerMaskLevel1 = (1 << LayerMask.NameToLayer("Section1")) + (1 << LayerMask.NameToLayer("SectionStarting"));
        _layerMaskSection2 = 1 << LayerMask.NameToLayer("Section2");
        _layerMaskSection3 = 1 << LayerMask.NameToLayer("Section3");
        _layerMaskEnemy = 1 << LayerMask.NameToLayer("Enemy");

        Init();
    }

    void Init()
    {
        for (int i = 0; i < _enemyCountSectonStart; i++)// 게임 시작 시 10마리만 소환
        {
            Spawn(RandomCoordinate());
        }
        StartCoroutine(Despawn());
        StartCoroutine(Respawn());
    }

    Vector3 RandomCoordinate()
    {
        //적이 생성 될 랜덤 좌표 구하기 (좌표는 필드 내에 있어야 함)
        Vector3 randPos;
        while (true)
        {
            Vector3 randSphere = Random.insideUnitSphere * _spawnRange;
            randSphere.y = 3f;
            randPos = _player.transform.position + randSphere;
            if (Physics.Raycast(randPos, -transform.up, Mathf.Infinity, _layerMaskGround))//필드 내에 좌표가 생성 됐으면
            {
                break;
            }
        }
        //몬스터 스폰 위치 y값 고정
        randPos.y = 0.8f;

        return randPos;
    }

    void Spawn(Vector3 spawnPos)
    {
        // 랜덤 좌표가 어떤 구역에 있는지
        Vector3 linecastEnd = spawnPos;
        linecastEnd.y = -7.0f;
        _section1 = Physics.Linecast(spawnPos, linecastEnd, _layerMaskLevel1);
        _section2 = Physics.Linecast(spawnPos, linecastEnd, _layerMaskSection2);
        _section3 = Physics.Linecast(spawnPos, linecastEnd, _layerMaskSection3);

        //구역에 맞게 적 생성
        int randArea = 1;
        if (_section1) { randArea = Random.Range(1, 6 + 1); } //Define.cs에서 meadowUint 순서 참고
        else if (_section2) { randArea = Random.Range(7, 12 + 1); }
        else if (_section3) { randArea = Random.Range(13, 16 + 1); }

        var enemy = EnemyPool.GetObject(((MeadowUnit)randArea).ToString());
        if (enemy != null)
            enemy.gameObject.transform.position = spawnPos;
    }

    //6초 마다 범위에 벗어난 몬스터 디스폰
    IEnumerator Despawn()
    {
        yield return new WaitForSeconds(_deSpawnTime);
        while (true)
        {
            //if (_despawnEnemyCount <= 0)
            //{
            int outMaxColliders = 127;
            int inMaxColliders = 127;
            Collider[] outHitColliders = new Collider[outMaxColliders];
            Collider[] inHitColliders = new Collider[inMaxColliders];
            int outColl = Physics.OverlapSphereNonAlloc(_player.transform.position, _destroyMaxRange, outHitColliders, _layerMaskEnemy);
            int inColl = Physics.OverlapSphereNonAlloc(_player.transform.position, _destroyMinRange, inHitColliders, _layerMaskEnemy);
            for (int i = 0; i < outColl; i++)
            {
                bool isInside = false; //범위내에 몬스터가 있는지?
                for (int j = 0; j < inColl; j++)
                {
                    if (outHitColliders[i] == inHitColliders[j])
                    {
                        isInside = true;
                        break;
                    }
                }
                if (isInside == false)
                {
                    var bye = outHitColliders[i].gameObject.GetComponent<EnemyCtrl>();
                    if (bye.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                    {
                        EnemyPool.ReturnObject(bye);
                        _despawnEnemyCount++;
                    }
                }
            }
            //}
            yield return new WaitForSeconds(_deSpawnTime);
        }
    }

    //10초 마다 디스폰된 수 만큼 리스폰
    IEnumerator Respawn()
    {

        while (true)
        {
            //지난 위치
            Vector3 PosEnd = transform.position;
            PosEnd.y = -7.0f;
            _sectionStart = Physics.Linecast(transform.position, PosEnd, _layerMaskSectionStart);
            _section1 = Physics.Linecast(transform.position, PosEnd, _layerMaskSection1);
            _section2 = Physics.Linecast(transform.position, PosEnd, _layerMaskSection2);
            _section3 = Physics.Linecast(transform.position, PosEnd, _layerMaskSection3);
            if (_sectionStart) _previousSection = 0;
            else if (_section1) _previousSection = 1;
            else if (_section2) _previousSection = 2;
            else if (_section3) _previousSection = 3;

            yield return new WaitForSeconds(_spawnTime);

            //현재위치
            Vector3 PosEnd1 = transform.position;
            PosEnd1.y = -7.0f;
            _sectionStart = Physics.Linecast(transform.position, PosEnd, _layerMaskSectionStart); //10
            _section1 = Physics.Linecast(transform.position, PosEnd1, _layerMaskSection1); //25
            _section2 = Physics.Linecast(transform.position, PosEnd1, _layerMaskSection2); //50
            _section3 = Physics.Linecast(transform.position, PosEnd1, _layerMaskSection3); //60
            if (_sectionStart) _curSection = 0;
            else if (_section1) _curSection = 1;
            else if (_section2) _curSection = 2;
            else if (_section3) _curSection = 3;

            if (_curSection == 0)
            {
                if (_previousSection == 1) _despawnEnemyCount += (_enemyCountSectonStart - _enemyCountSecton1); //1->0  ) -10
                else if (_previousSection == 2) _despawnEnemyCount += (_enemyCountSectonStart - _enemyCountSecton2); //2->0  ) -40
            }
            else if (_curSection == 1)
            {
                if (_previousSection == 0) _despawnEnemyCount -= (_enemyCountSectonStart - _enemyCountSecton1); //0->1  ) +15
                else if (_previousSection == 2) _despawnEnemyCount += (_enemyCountSecton1 - _enemyCountSecton2); //2 -> 1  )-15
                else if (_previousSection  ==3) _despawnEnemyCount += (_enemyCountSecton1 - _enemyCountSecton3); //3 -> 1  )-35
            }
            else if (_curSection == 2)
            {
                if(_previousSection ==0) _despawnEnemyCount -= (_enemyCountSectonStart - _enemyCountSecton2); //0->2  ) +40
                else if (_previousSection == 1) _despawnEnemyCount -= (_enemyCountSecton1 - _enemyCountSecton2); // 1-> 2  )+15
                else if (_previousSection == 3) _despawnEnemyCount += (_enemyCountSecton2 - _enemyCountSecton3); // 3 -> 2  )-10

            }
            else if (_curSection == 3)
            {
                if (_previousSection == 0) _despawnEnemyCount -= (_enemyCountSectonStart - _enemyCountSecton3); //0->3  ) +50
                else if (_previousSection == 1) _despawnEnemyCount -= (_enemyCountSecton1 - _enemyCountSecton3); // 1-> 3  )+35
                else if (_previousSection == 2) _despawnEnemyCount -= (_enemyCountSecton2 - _enemyCountSecton3); //2 -> 3   )+10
            }

            if (_despawnEnemyCount > 0)
            {
                for (int i = 0; i < _despawnEnemyCount; i++)
                    Spawn(RandomCoordinate());

                _despawnEnemyCount = 0;
            }
        }
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(_player.transform.position, _spawnRange);
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(_player.transform.position, _destroyMinRange);
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(_player.transform.position, _destroyMaxRange);

    //}
}   