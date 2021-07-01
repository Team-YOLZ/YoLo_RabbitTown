using UnityEngine;
using UnityEngine.AI;
using static Define;

public class EnemyCtrl : CreatureCtrl
{
    private NavMeshAgent _agent;
    private Transform _player;
    private Vector3 Mine;// 자신 위치.
    private LayerMask _whatIsGround, _whatIsPlayer;

    //순찰.
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //공격.
    //bool alreadyAttacked;
    //public GameObject projectile; //원거리 공격 적 위

    //State
    public float sightRange; //순찰범위
    public float attackRange; //공격범위
    public bool playerInSightRange; //아마 불값 다른코드에서 가져다 쓸 것 같아서 public
    public bool playerInAttackRange;

    [SerializeField] protected int _level = 1; //유닛의 레벨
    [SerializeField] protected MeadowUnit meadowUnit = MeadowUnit.Null;// 어떤 유닛인지
    [SerializeField] protected int _dropcoin; //죽으면 주는 돈.
    [SerializeField] protected int _spoilnumber; //죽으면 주는 전리품 넘버.
    public UnitData unitData;

    protected override void Init()
    {
        base.Init();
        _agent = GetComponent<NavMeshAgent>();
        _player = GameObject.Find("player").transform;
        Mine = transform.position;
        _whatIsGround = 1 << LayerMask.NameToLayer("Ground");
        _whatIsPlayer = (1 << LayerMask.NameToLayer("Player")) + (1 << LayerMask.NameToLayer("Ally"));
    }
    protected override void Init2()
    {
        base.Init2();
        _creature.tag = "Enemy";
        _creature.layer = LayerMask.NameToLayer("Enemy");

        //공격 사거리 NavMeshAgent의 stoppingDistance에 적용
        _agent.stoppingDistance = _attackRange;
    }

    protected override void UpdateController()
    {
        if (State != CreatureState.Dead)
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
        //_animator.SetTrigger("Dead");
        gameObject.transform.localScale = new Vector3(1.0f, 0.5f, 1.0f);
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
    }

    private void Patroling() //순찰 코드
    {
        State = CreatureState.Moving;
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            _agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
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
        _agent.SetDestination(go.transform.position);
    }

    //object Pooler Control
    private void OnEnable()//활성화시 객체 초기화 로직.
    {
        gameObject.transform.position = Mine;
        _currentHp = _hp;
    }
    private void OnDisable()
    {
        //ObjectPooler.ReturnToPool(gameObject);
        CancelInvoke();
        Invoke(nameof(ReSpawn),5f);
    }

    private void DeactiveDelay() => gameObject.SetActive(false);
    private void ReSpawn() => gameObject.SetActive(true); //Respawn

    //private void OnDrawGizmosSelected() //공격 범위 순찰 범위 영역 보기 위해.
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(transform.position, attackRange);
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawWireSphere(transform.position, sightRange);
    //}
}
