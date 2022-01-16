using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using static Define;


public class AllyCtrl : CreatureCtrl //동료 컨트롤러
{
    private NavMeshAgent _agent;
    [SerializeField] private Transform _player;
    private LayerMask _whatIsEnemy, _whatIsPlayer;

    //State
    public float _detectionRange; //적 발견 사거리
    public float _playerRange; //플레이어와 동료 사거리( 이 사거리안에서는 idle)
    public float _playerRangeWellOff; //적 공격 시 플레이어와 거리 좀 더 확장시킴
    float _playerMaxRange; // 너무 멀어 질 시 플레이어에게 순간이동하기 위한 변수
    public bool _enemyInSightRange;//사거리내에 적을 발견 했으면 트루
    public bool _enemyInAttackRange;
    public bool _playerAndAlly; //플레이어와 동료가 사거리 내에 있는지
    public bool _playerAndAllyWellOff; //플레이어와 동료가 사거리 내에 있는지(확장)
    public bool _teleportRange; //너무 멀어질 시 텔레포트
    bool _attackMoving; //움직일 시 (공격하러 움직이는 것인가/플레이어가 이동해서 이동하는 움직임인가)

    public UnitData unitData;
    BackEndGetTable tableData;

    protected int _level; //유닛의 레벨
    protected MeadowUnit meadowUnit = MeadowUnit.Null;// 어떤 유닛인지

    protected override void Init()
    {
        unitData = Resources.Load<UnitData>($"UnitTemplete/{gameObject.name}");
        tableData = FindObjectOfType<BackEndGetTable>();
        base.Init();
        _agent = GetComponent<NavMeshAgent>();
        _player = GameObject.Find("player").transform;
        _whatIsEnemy = (1 << LayerMask.NameToLayer("Enemy"))+ (1 << LayerMask.NameToLayer("Boss"));
        _whatIsPlayer = 1 << LayerMask.NameToLayer("Player");
    }

    protected override void Init2()
    {
        _level = Leveling();
        base.Init2();
        _creature.tag = "Team";
        _creature.layer = LayerMask.NameToLayer("Ally");
        _agent.enabled = true;
        //공격 사거리 NavMeshAgent의 stoppingDistance에 적용
        _agent.stoppingDistance = _attackRange;
        _agent.speed = _speed;
        //팀으로 적을 포획했을 때 아웃라인 원상복귀 코드.
        render = gameObject.GetComponentInChildren<Renderer>();
        render.material.SetFloat("_OutlineWidth", 1.0f);

        //die 애니메이션에서 빠져나오기
        _animator.Play("Idle");
        //플레이어랑 너무 멀어질 수 순간이동 하기 위한 변수 초기화
        _playerMaxRange = 45;

        //hpbar 생성
        GameObject go = GameObject.Find("GameScene_Canvas");
        _hpBar = Managers.Resource.Instantiate("UI/TeamHpBar", go.transform.Find("HpBar").gameObject.transform);
        //_hpBar = _hpBarCanvas.transform.Find("HpBar").gameObject;
        _sliderHp = _hpBar.GetComponent<Slider>();
        _sliderHp.maxValue = _hp; //피통 maxValue = 전체체력
        _sliderHp.value = _hp;
        _hpBar.SetActive(false); //생성,초기화 후 비활성

    }

    protected override void UpdateController()
    {
        if (State != CreatureState.Dead)
        {
            _enemyInSightRange = Physics.CheckSphere(transform.position, _detectionRange, _whatIsEnemy); //적 발견
            _enemyInAttackRange = Physics.CheckSphere(transform.position, _attackRange, _whatIsEnemy); //공격 사거리
            _playerAndAlly = Physics.CheckSphere(transform.position, _playerRange, _whatIsPlayer); //플레이어와의 거리
            _playerAndAllyWellOff = Physics.CheckSphere(transform.position, _playerRangeWellOff, _whatIsPlayer); //적 공격 시 플레이어와의 거리(확장)
            _teleportRange = Physics.CheckSphere(transform.position, _playerMaxRange, _whatIsPlayer);
            if (!_teleportRange)
            {
                Teleport();
            }
            //if (!_enemyInSightRange && !_enemyInAttackRange) //순찰범위,공격범위 벗어나있을 때
            //{
            //    if (_playerAndAlly) //플레이어와 동료가 사거리내에 있을 때
            //        State = CreatureState.Idle;
            //    else //사거리내에 없을 때 (플레이어가 움직이고 있다는 뜻)
            //        State = CreatureState.Moving;
            //}
            //if (_enemyInSightRange && !_enemyInAttackRange)//순찰범위엔 포함 공격범위엔 벗어나있을 때
            //    State = CreatureState.Moving;
            //if (_enemyInSightRange && _enemyInAttackRange)//순찰범위,공격범위 모두 포함되어있을 때
            //{
            //    if (_playerAndAllyWellOff)
            //        State = CreatureState.Skill;
            //    else
            //        State = CreatureState.Moving;
            //}

            if (!_enemyInSightRange) //적이 근처에 없음
            {
                if (_playerAndAlly) //플레이어와 동료가 사거리내에 있을 때
                    State = CreatureState.Idle;
                else //사거리내에 없을 때 (플레이어가 움직이고 있다는 뜻)
                {
                    State = CreatureState.Moving;
                    _attackMoving = false;
                }
            }
            else //적 발견
            {
                if (!_enemyInAttackRange) //공격 사거리에 없음
                {
                    State = CreatureState.Moving;
                    _attackMoving = true;
                }
                else //공격 사거리에 있음
                    State = CreatureState.Skill;

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
        if (_attackMoving)
        {
            if (_playerAndAllyWellOff) // 플레이어와 동료가 서로 (확장)사거리내에 있으면
                if(FindNearestObjectByTag("Enemy") != null)
                    _agent.SetDestination(FindNearestObjectByTag("Enemy").transform.position); //가까운 적에게
            else
                _agent.SetDestination(_player.position); //너무 멀어지면 플레이어에게
        }else
            _agent.SetDestination(_player.position);
    }

    protected override void UpdateDead()
    {
        //죽었을 때 로직
        Destroy(gameObject);
    }

    protected override void DefaultStatDBConnection() //초기 스탯 할당.
    {
        meadowUnit = unitData.Level[_level - 1].meadowUnit;
        _atk = unitData.Level[_level-1].Attack;
        _atkSpeed = unitData.Level[_level-1].AttackSpeed;
        _speed = unitData.Level[_level-1].MovingSpeed;
        _hp = unitData.Level[_level-1].Hp;
        _attackRange = unitData.Level[_level-1].range;
        _detectionRange = unitData.Level[_level - 1].DetectionRange;
        _playerRange = unitData.Level[_level - 1].PlayerRange;
        _playerRangeWellOff = unitData.Level[_level - 1].PlayerRangeWellOff;
    }

    void Teleport()
    {
        _agent.enabled = false;
        transform.position = _player.position + new Vector3(-1 * Random.value, 0, -2 * Random.value);

        _agent.enabled = true;
    }

    int Leveling()
    {
        string name = gameObject.name;
        int kill =0 ;
        switch (name)
        {
            case "Chicken1":
                kill = tableData.playerKillData.Chicken1;
                break;
            case "Chicken2":
                kill = tableData.playerKillData.Chicken2;
                break;
            case "Chicken3":
                kill = tableData.playerKillData.Chicken3;
                break;
            case "Cow1":
                kill = tableData.playerKillData.Cow1;
                break;
            case "Cow2":
                kill = tableData.playerKillData.Cow2;
                break;
            case "Cow3":
                kill = tableData.playerKillData.Cow3;
                break;
            case "Duck1":
                kill = tableData.playerKillData.Duck1;
                break;
            case "Duck2":
                kill = tableData.playerKillData.Duck2;
                break;
            case "Duck3":
                kill = tableData.playerKillData.Duck3;
                break;
            case "Horse1":
                kill = tableData.playerKillData.Horse1;
                break;
            case "Horse2":
                kill = tableData.playerKillData.Horse2;
                break;
            case "Horse3":
                kill = tableData.playerKillData.Horse3;
                break;
            case "Goat1":
                kill = tableData.playerKillData.Goat1;
                _rangeAttacktype = true;
                break;
            case "Goat2":
                kill = tableData.playerKillData.Goat2;
                _rangeAttacktype = true;
                break;
            case "Sheep1":
                kill = tableData.playerKillData.Sheep1;
                _rangeAttacktype = true;
                break;
            case "Sheep2":
                kill = tableData.playerKillData.Sheep2;
                _rangeAttacktype = true;
                break;
        }
        
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

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.gray;
    //    Gizmos.DrawWireSphere(transform.position, _attackRange);
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(transform.position, _detectionRange);
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(transform.position, _playerRange);
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(transform.position, _playerRangeWellOff);

    //}
}
