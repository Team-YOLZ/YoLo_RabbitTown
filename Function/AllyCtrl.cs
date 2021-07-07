using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static Define;


public class AllyCtrl : CreatureCtrl //동료 컨트롤러
{
    private NavMeshAgent _agent;
    private Transform _player;
    private LayerMask _whatIsEnemy, _whatIsPlayer;

    //State
    public float _detectionRange; //적 발견 사거리
    public float _playerRange; //플레이어와 동료 사거리
    public bool _enemyInSightRange;//사거리내에 적을 발견 했으면 트루
    public bool _enemyInAttackRange;
    public bool _playerAndAlly; //플레이어와 동료가 사거리 내에 있는지

    public UnitData unitData;
    [SerializeField] protected int _level; //유닛의 레벨
    [SerializeField] protected MeadowUnit meadowUnit = MeadowUnit.Null;// 어떤 유닛인지
    [SerializeField] protected int _dropcoin; //죽으면 주는 돈.
    [SerializeField] protected int _spoilnumber; //죽으면 주는 전리품 넘버.

    protected override void Init()
    {
        unitData = Resources.Load<UnitData>($"UnitTemplete/{gameObject.name}");
        base.Init();
        _agent = GetComponent<NavMeshAgent>();
        _player = GameObject.Find("player").transform;
        _whatIsEnemy = 1 << LayerMask.NameToLayer("Enemy");
        _whatIsPlayer = 1 << LayerMask.NameToLayer("Player");
    }

    protected override void Init2()
    {
        _level = 1;//임시 초기화.
        base.Init2();
        _creature.tag = "Team";
        _creature.layer = LayerMask.NameToLayer("Ally");

        //공격 사거리 NavMeshAgent의 stoppingDistance에 적용
        _agent.stoppingDistance = _attackRange;
        _agent.speed = _speed;
        render = gameObject.GetComponentInChildren<Renderer>();
        render.material.SetFloat("_OutlineWidth", 1.0f);
        //AbilityByLevel();
    }

    /*
    private void OnEnable() //활성화 될 때 모든 팀에게 버프 적용
    {
        Buff(meadowUnit, true);
    }

    private void OnDisable() //비 활성화 될 때 모든 팀에게 버프 해제
    {
        Buff(meadowUnit, false);
    }
    */

    //private void AbilityByLevel() //현재 레벨에 맞는 능력치 적용
    //{
    //    switch (meadowUnit)
    //    {
    //        case MeadowUnit.Chicken1:
    //            _hp += (3 * _level);
    //            break;
    //        case MeadowUnit.Chicken2:
    //            _hp += (10 * _level);
    //            _atk += _level;
    //            break;
    //        case MeadowUnit.Chicken3:
    //            _hp += (3 * _level);
    //            _atk += (2 * _level);
    //            break;
    //        case MeadowUnit.Cow1:
    //            _hp += (10 * _level);
    //            break;
    //        case MeadowUnit.Cow2:
    //            _hp += (20 * _level);
    //            break;
    //        case MeadowUnit.Cow3:
    //            _hp += (25 * _level);
    //            break;
    //        case MeadowUnit.Duck1:
    //            _hp += (5 * _level);
    //            _atk += _level;
    //            break;
    //        case MeadowUnit.Duck2:
    //            _hp += (10 * _level);
    //            _atk += (1.5f * _level);
    //            break;
    //        case MeadowUnit.Duck3:
    //            _hp += (10 * _level);
    //            _atk += (2 * _level);
    //            break;
    //        case MeadowUnit.Horse1:
    //            _hp += (4 * _level);
    //            _atk += _level;
    //            break;
    //        case MeadowUnit.Horse2:
    //            _hp += (8 * _level);
    //            _atk += (1.5f * _level);
    //            break;
    //        case MeadowUnit.Horse3:
    //            _hp += (10 * _level);
    //            _atk += (2 * _level);
    //            break;
    //        case MeadowUnit.Sheep1:
    //            _hp += (4 * _level);
    //            _atk += (3 * _level);
    //            break;
    //        case MeadowUnit.Sheep2:
    //            _hp += (5 * _level);
    //            _atk += (5 * _level);
    //            break;
    //        case MeadowUnit.Goat1:
    //            _hp += (4 * _level);
    //            _atk += (0.5f * _level);
    //            _atkSpeed += (0.05f * _level);
    //            break;
    //        case MeadowUnit.Goat2:
    //            _hp += (3 * _level);
    //            _atk += (1 * _level);
    //            _atkSpeed += (0.05f * _level);
    //            break;
    //        default:
    //            break;
    //    }
    //}

    protected override void UpdateController()
    {
        if (State != CreatureState.Dead)
        {
            _enemyInSightRange = Physics.CheckSphere(transform.position, _detectionRange, _whatIsEnemy);
            _enemyInAttackRange = Physics.CheckSphere(transform.position, _attackRange, _whatIsEnemy);
            _playerAndAlly = Physics.CheckSphere(transform.position, _playerRange, _whatIsPlayer);
            if (!_enemyInSightRange && !_enemyInAttackRange) //순찰범위,공격범위 벗어나있을 때
            {
                if (_playerAndAlly) //플레이어와 동료가 사거리내에 있을 때
                    State = CreatureState.Idle;
                else //사거리내에 없을 때 (플레이어가 움직이고 있다는 뜻)
                    State = CreatureState.Moving;
            }
            if (_enemyInSightRange && !_enemyInAttackRange)//순찰범위엔 포함 공격범위엔 벗어나있을 때
                State = CreatureState.Moving;
            if (_enemyInSightRange && _enemyInAttackRange)//순찰범위,공격범위 모두 포함되어있을 때
            {
                if (_playerAndAlly)
                    State = CreatureState.Skill;
                else
                    State = CreatureState.Moving;
            }
        }
        base.UpdateController();
    }

    protected override void UpdateIdle()
    {
        _agent.SetDestination(transform.position);
    }

    protected override void UpdateSkill()
    {
        _agent.SetDestination(transform.position);
        base.UpdateSkill();
    }

    protected override void UpdateMoving()
    {
        if (_playerAndAlly) // 플레이어와 동료가 서로 사거리내에 있으면
            _agent.SetDestination(FindNearestObjectByTag("Enemy").transform.position); //가까운 적에게

        else
            _agent.SetDestination(_player.position);
    }

    protected override void UpdateDead()
    {
        //죽었을 때 로직
    }

    protected override void DefaultStatDBConnection() //초기 스탯 할당.
    {
        meadowUnit = unitData.Level[_level - 1].meadowUnit;
        _atk = unitData.Level[_level-1].Attack;
        _atkSpeed = unitData.Level[_level-1].AttackSpeed;
        _speed = unitData.Level[_level-1].MovingSpeed;
        _hp = unitData.Level[_level-1].Hp;
        _attackRange = unitData.Level[_level-1].range;
        _dropcoin = unitData.Level[_level-1].DropCoin;
        _spoilnumber = unitData.Level[_level-1].Spoilnumber;
        _detectionRange = unitData.Level[_level - 1].DetectionRange;
        _playerRange = unitData.Level[_level - 1].PlayerRange;

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _detectionRange); //red : 적 발견 범위
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _attackRange); //yellow : 공격 사거리
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _playerRange); //green : 플레이어와 유닛 내 허용 범위
    }
}
