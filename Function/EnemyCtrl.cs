using UnityEngine;
using UnityEngine.AI;

public class EnemyCtrl : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform player;
    public Vector3 Mine;// 자신 위치.

    public LayerMask whatIsGround, whatIsPlayer;

    public float Maxhealth; //전체 피통.
    public float health; //현재 피통.

    //순찰.
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //공격.
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public GameObject projectile; //원거리 공격 적 위

    //State
    public float sightRange; //순찰범위
    public float attackRange; //공격범위
    public bool playerInSightRange; //아마 불값 다른코드에서 가져다 쓸 것 같아서 public
    public bool playerInAttackRange;


    private void Awake()
    {
        player = GameObject.Find("player").transform;
        Mine = transform.position;
        agent = GetComponent<NavMeshAgent>();
        health = Maxhealth;
    }

    private void Update()
    {
        //Check 순찰범위,공격범위에 따른 State Change
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange) Patroling(); //순찰범위,공격범위 벗어나있을 때
        if (playerInSightRange && !playerInAttackRange) ChasePlayer(); //순찰범위엔 포함 공격범위엔 벗어나있을 때
        if (playerInAttackRange && playerInSightRange) AttackPlayer(); //순찰범위,공격범위 모두 포함되어있을 때
    }

    private void Patroling() //순찰 코드
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

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

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void ChasePlayer() //추격함수.
    {
        agent.SetDestination(player.position);
    }

    private void AttackPlayer() //공격함수.
    {
        //nevmesh set destination을 player로.
        agent.SetDestination(transform.position);

        //transform.LookAt(player,Vector3.up);//y포스는 고정할지 고민중. Rotation 코드.

        if (!alreadyAttacked)
        {
            //공격코드 (원거리,근거리 등등 여기서 적 구분해서 지정할 예정.)

            //공격코드 끝.

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks); //공격 사이의 설정(공격 딜레이 혹은 효과 넣으면 되)
        }
    }
    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0) Invoke(nameof(DeactiveDelay), 0.5f); //Death Animation 실행위해.
    }
    //object Pooler Control
    private void OnEnable()//활성화시 객체 초기화 로직.
    {
        gameObject.transform.position = Mine;
        health = Maxhealth;
    }
    private void OnDisable()
    {
        //ObjectPooler.ReturnToPool(gameObject);
        CancelInvoke();
        Invoke(nameof(ReSpawn),5f);
    }

    private void DeactiveDelay() => gameObject.SetActive(false);
    private void ReSpawn() => gameObject.SetActive(true); //Respawn

    private void OnDrawGizmosSelected() //공격 범위 순찰 범위 영역 보기 위해.
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
