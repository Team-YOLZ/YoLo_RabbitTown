using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using static Define;
using System.Reflection;
using UnityEngine.UI;

public class EnemyCtrl : CreatureCtrl
{
    private NavMeshAgent _agent;
    private Transform _playertr;
    private GameObject player;
    private PlayerCtrl PlayerCtrl;
    private Vector3 Mine;// 자신 위치.
    private LayerMask _whatIsGround, _whatIsPlayer;

    //순찰.
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;
 
    //State
    public float sightRange; //순찰범위
    public float attackRange; //공격범위
    public bool playerInSightRange; //아마 불값 다른코드에서 가져다 쓸 것 같아서 public
    public bool playerInAttackRange;
    bool isDead; //죽었는지 ?

    protected int _level = 1; //유닛의 레벨
    protected int _canCapturePercent; //유닛 포획 확률.
    protected MeadowUnit meadowUnit = MeadowUnit.Null;// 어떤 유닛인지
    protected int _dropcoin; //죽으면 주는 돈.
    protected string _enemyname; //Enemy Name

    public int _spoilnumber; //죽으면 주는 전리품 넘버(Player가 호출해야 하기 때문에 public). 
    public int _spoilamount; //죽으면 주는 전리품 양(Player가 호출해야 하기 때문에 public).

    public UnitData unitData;
    BackEndGetTable GetPlayerStatData;

    protected override void Init()
    {
        base.Init();
        _agent = GetComponent<NavMeshAgent>();
        _playertr = GameObject.Find("player").transform;
        _whatIsGround = 1 << LayerMask.NameToLayer("Section");
        _whatIsPlayer = (1 << LayerMask.NameToLayer("Player")) + (1 << LayerMask.NameToLayer("Ally"));

    }

    private void OnEnable()//활성화시 객체 초기화 로직.
    {
        StartCoroutine(EnableAfterWarkPoint()); //활성화시 0.2초 뒤 warkpoint를 다시 자기자신으로 설정
       
    }

    protected override void Init2()
    {
        
        base.Init2();
        if (_creature.layer == LayerMask.NameToLayer("Boss"))
        {
            _creature.tag = "Enemy";
            _creature.layer = LayerMask.NameToLayer("Boss");
        }
        else
        {
            _creature.tag = "Enemy";
            _creature.layer = LayerMask.NameToLayer("Enemy");
        }
        //공격 사거리 NavMeshAgent의 stoppingDistance에 적용
        _agent.enabled = true;  // NavMeshAgent가 Bake된 NavMesh에 이미 종속돼 활동을 시작했기 때문에 임의로 현재 오브젝트 위치를 변경 허용 x
        _agent.stoppingDistance = _attackRange;
        _agent.speed = _speed;

        GetPlayerStatData = GameObject.Find("UserTableInformation").GetComponent<BackEndGetTable>();
        player = GameObject.Find("player");
        PlayerCtrl = player.GetComponent<PlayerCtrl>();
        render = gameObject.GetComponentInChildren<Renderer>();

        //hpbar 생성
        GameObject go = GameObject.Find("GameScene_Canvas");
        _hpBar = Managers.Resource.Instantiate("UI/EnemyHpBar", go.transform.Find("HpBar").gameObject.transform);
        //_hpBar = _hpBarCanvas.transform.Find("HpBar").gameObject;
        _sliderHp = _hpBar.GetComponent<Slider>();
        _sliderHp.maxValue = _hp; //피통 maxValue = 전체체력
        _sliderHp.value = _hp;
        _hpBar.SetActive(false); //생성,초기화 후 비활성

        isDead = false;
    }

    private void OnDisable()
    {
        _agent.enabled = false;
    }

    protected override void UpdateController()
    {
        //보스는 idle 상태 지속, 추격, 공격만
        if(_enemyname.Substring(0,_enemyname.Length-1) == "Boss")
        {
            if(State!= CreatureState.Dead)
            {
                playerInSightRange = Physics.CheckSphere(transform.position, sightRange, _whatIsPlayer);
                playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, _whatIsPlayer);
                if (playerInSightRange && !playerInAttackRange) ChasePlayer(); //순찰범위엔 포함 공격범위엔 벗어나있을 때
                if (playerInAttackRange && playerInSightRange) State = CreatureState.Skill; //순찰범위,공격범위 모두 포함되어있을 때
            }
        }
        else if (State != CreatureState.Dead)
        {
            //Check 순찰범위,공격범위에 따른 State Change
            playerInSightRange = Physics.CheckSphere(transform.position, sightRange, _whatIsPlayer);
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, _whatIsPlayer);
            if (!playerInSightRange && !playerInAttackRange) Patroling(); //순찰범위,공격범위 벗어나있을 때
            if (playerInSightRange && !playerInAttackRange) ChasePlayer(); //순찰범위엔 포함 공격범위엔 벗어나있을 때
            if (playerInAttackRange && playerInSightRange) State = CreatureState.Skill; //순찰범위,공격범위 모두 포함되어있을 때
        }
        base.UpdateController();
    }

    protected override void UpdateSkill()
    {
        _agent.SetDestination(transform.position);
        base.UpdateSkill();
    }

    protected override void UpdateDead()
    {
        //죽었을 때 로직 (코인, 전리품, 동료로 변환..)
        //Collider[] cols = Physics.OverlapSphere(gameObject.transform.position, 8);
        //foreach (Collider col in cols)
        //{
        //    if (col.gameObject.layer == LayerMask.NameToLayer("Player") && gameObject.layer == LayerMask.NameToLayer("Neutrality"))
        //    {
        //        render.material.SetFloat("_OutlineWidth", 1.15f);
        //        if (!PlayerCtrl._getSpoilButton.activeSelf)
        //        {
        //            PlayerCtrl.OnCaptureButton(); //capture 버튼 On
        //            PlayerCtrl.OnGetSpoilButton(); //Spoil 버튼 On
        //        }
        //    }
        //    else
        //    {
        //        render.material.SetFloat("_OutlineWidth", 1.0f);
        //        if (PlayerCtrl._getSpoilButton.activeSelf)
        //        {
        //            PlayerCtrl.OffCaptureButton(); //capture 버튼 Off
        //            PlayerCtrl.OffGetSpoilButton(); //Spoil 버튼 Off
        //        }
        //    }
        //}
        float dis = Vector3.Distance(player.transform.position, transform.position); //플레이어와의 거리
        if (gameObject.layer == LayerMask.NameToLayer("Neutrality"))
        {
            if (dis < 8f)
            {
                render.material.SetFloat("_OutlineWidth", 1.15f);
                if (!PlayerCtrl._getSpoilButton.activeSelf)
                {
                    PlayerCtrl.OnCaptureButton(); //capture 버튼 On
                    PlayerCtrl.OnGetSpoilButton(); //Spoil 버튼 On
                }
            }
            else if (dis > 8f && dis < 12f)
            {
                render.material.SetFloat("_OutlineWidth", 1.0f);
                if (PlayerCtrl._getSpoilButton.activeSelf)
                {
                    PlayerCtrl.OffCaptureButton(); //capture 버튼 Off
                    PlayerCtrl.OffGetSpoilButton(); //Spoil 버튼 Off
                }
            }
        }

        if (!isDead)
        {
            EnemySpawning._despawnEnemyCount++; // 디스폰enemy 카운트 증가
            isDead = true;
        }
    }

    private void Patroling() //순찰 코드
    {
        State = CreatureState.Moving;
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet && _agent.enabled == true) 
            _agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 10f)
            walkPointSet = false;
    }

    private void SearchWalkPoint() //순찰 포인트 지정 함수.
    {
        //지정 범위 랜덤 포인트 순찰.
        float randomZ = Random.Range(-walkPointRange, walkPointRange); //랜덤범위(-지정 범위 ~ 지정범위) z축.
        float randomX = Random.Range(-walkPointRange, walkPointRange); //랜덤범위(-지정 범위 ~ 지정범위) x축.
        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, _whatIsGround))
            walkPointSet = true;
    }

    private void ChasePlayer() //추격함수.
    {
        State = CreatureState.Moving;
        GameObject go = FindNearestObjectByTag("Team");

        if(go !=null && _agent.enabled == true)
            _agent.SetDestination(go.transform.position);
    }

    protected override void DefaultStatDBConnection() //초기 스탯 할당.
    {
        _atk = unitData.Level[0].Attack;
        _atkSpeed = unitData.Level[0].AttackSpeed;
        _speed = unitData.Level[0].MovingSpeed;
        _hp = unitData.Level[0].Hp;
        _attackRange = unitData.Level[0].range;
        _dropcoin = unitData.Level[0].DropCoin;
        _spoilnumber = unitData.Level[0].Spoilnumber;
        _spoilamount = unitData.Level[0].SpoilAmount;
        _canCapturePercent = unitData.Level[0].CanCapturePercent;
        _enemyname = unitData.Level[0].Name;
    }

    public void RandomCapture() //적 죽었을때 포획 or go pool ,  Coin 획득 , KillCount 증가.
    {
        gameObject.transform.localScale -= new Vector3(0f, 0.5f, 0f); //죽은 효과를 더 살리게 위해 크기 변경.
        GetPlayerStatData.playerAsset.Coin += _dropcoin; //몬스터 지정 코인 획득.
        KillCount(); //킬 카운트 증가.
        int RandomPercent = Random.Range(0, 100);
        int Percent = GetPlayerStatData.playerStat.Appeal + _canCapturePercent; //기존 몬스터 포획 확률 + 플레이어 매력수치 = 포획 확률
        if (RandomPercent <= Percent) // 포획가능
        {
            gameObject.tag = "IsDeadEnemy";
            gameObject.layer = LayerMask.NameToLayer("Neutrality");
        }
        else //포획불가능
        {
            gameObject.tag = "GoPoolEnemy"; 
            //gameObject.layer = LayerMask.NameToLayer("Neutrality");
            StartCoroutine(GoPool());
        }
    }

    private void KillCount() //몬스터에 따른 킬카운트 증가.
    {
        switch (_enemyname)
        {
            case "Chicken1":
                GetPlayerStatData.playerKillData.Chicken1 += 1;
                break;
            case "Chicken2":
                GetPlayerStatData.playerKillData.Chicken2 += 1;
                break;
            case "Chicken3":
                GetPlayerStatData.playerKillData.Chicken3 += 1;
                break;
            case "Cow1":
                GetPlayerStatData.playerKillData.Cow1 += 1;
                break;
            case "Cow2":
                GetPlayerStatData.playerKillData.Cow2 += 1;
                break;
            case "Cow3":
                GetPlayerStatData.playerKillData.Cow3 += 1;
                break;
            case "Goat1":
                GetPlayerStatData.playerKillData.Goat1 += 1;
                break;
            case "Goat2":
                GetPlayerStatData.playerKillData.Goat2 += 1;
                break;
            case "Horse1":
                GetPlayerStatData.playerKillData.Horse1 += 1;
                break;
            case "Horse2":
                GetPlayerStatData.playerKillData.Horse2 += 1;
                break;
            case "Horse3":
                GetPlayerStatData.playerKillData.Horse3 += 1;
                break;
            case "Duck1":
                GetPlayerStatData.playerKillData.Duck1 += 1;
                break;
            case "Duck2":
                GetPlayerStatData.playerKillData.Duck2 += 1;
                break;
            case "Duck3":
                GetPlayerStatData.playerKillData.Duck3 += 1;
                break;
            case "Sheep1":
                GetPlayerStatData.playerKillData.Sheep1 += 1;
                break;
            case "Sheep2":
                GetPlayerStatData.playerKillData.Sheep2 += 1;
                break;
            default:
                break;
        }
    }

    IEnumerator EnableAfterWarkPoint()
    {
        yield return new WaitForSeconds(0.2f);
        walkPoint = _creature.transform.position;
        _agent.enabled = true;
    }

    IEnumerator GoPool()
    {
        render.material.SetFloat("_OutlineWidth", 1.25f);
        yield return new WaitForSeconds(0.2f);
        render.material.SetFloat("_OutlineWidth", 1f);
        yield return new WaitForSeconds(0.2f);
        render.material.SetFloat("_OutlineWidth", 1.25f);
        yield return new WaitForSeconds(0.2f);
        render.material.SetFloat("_OutlineWidth", 1f);
        yield return new WaitForSeconds(0.2f);

        EnemyPool.ReturnObject(this);
    }
}
