using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Define;

// 에니메이션 플레이, 기본적인 요소 초기화 , 공격로직, 받은데미지 로직, 

public class CreatureCtrl : MonoBehaviour
{
    //TODO: 나중에 SerializeField 지워야 함.

    //디비에서 할당받아야하는 공통 정보.
    [SerializeField] protected int _hp;          // 전체체력 
    [SerializeField] protected float _speed;     //이동속도
    [SerializeField] protected float _atk;       //공격력
    [SerializeField] protected float _atkSpeed;  //공격속도

    //공통 고유 정보.
    [SerializeField] protected int _attackRange;//공격사거리 <-agent. stopping distance
    public bool _rangeAttacktype = false; //근거리(false) or 원거리(true)
    protected Animator _animator;
    [SerializeField] protected CreatureState _state = CreatureState.Idle;
    [SerializeField] protected GameObject _creature;// 자기자신

    //변경 정보.
    [SerializeField] protected float _currentHp;  //현재 체력
    [SerializeField] private BoxCollider boxCollider;

    //Creature 공통 정보.
    public Renderer render;
    //공격
    Coroutine _coSkill;

    public virtual CreatureState State
    {
        get { return _state; }
        set
        {
            if (_state == value)
                return;

            _state = value;
            UpdateAnimation();
        }
    }

    void Awake()
    {
        Init();
    }

    private void Start()
    {
        Init2();
        boxCollider = gameObject.GetComponent<BoxCollider>();
    }
    void Update()
    {
        UpdateController();
    }

    protected virtual void Init() //초기화 부분(Awake)
    {
        _creature = gameObject;
    }
    protected virtual void Init2() //초기화 부분 (Start)
    {
        DefaultStatDBConnection(); //디폴트능력치수치 디비와 연결
        _animator = _creature.GetComponent<Animator>();
        _currentHp = _hp;
    }

    protected virtual void UpdateAnimation() //애니메이션 처리
    {
        if (_state == CreatureState.Dead)
        {
            StopAllCoroutines();
            _animator.SetTrigger("Dead");
        }
        else if (_state == CreatureState.Moving)
        {
            _animator.Play("Run");
        }
        else if (_state == CreatureState.Skill)
        {
            _animator.Play("Attack");
        }
        else 
        {
            _animator.Play("Idle");
        }
    }

    protected virtual void UpdateController() //상황에 따른 처리(ex.. Idle 상태이면 UpdateIdle()에서 처리)
    {
        switch (State)
        {
            case CreatureState.Idle:
                UpdateIdle();
                break;
            case CreatureState.Moving:
                UpdateMoving();
                break;
            case CreatureState.Skill:
                UpdateSkill();
                break;
            case CreatureState.Dead:
                UpdateDead();
                break;
        }
    }

    protected virtual void UpdateIdle()
    {
    }

    protected virtual void UpdateMoving()
    {
    }

    protected virtual void UpdateSkill() //공격
    {
        if (_coSkill == null)
        {
            _coSkill = StartCoroutine(CoAttack());
        }
    }
    protected virtual void UpdateDead()
    {

    }

    IEnumerator CoAttack()
    {
        State = CreatureState.Skill;
        GameObject go;

        // 피격 판정
        if (_creature.CompareTag("Team"))
        {
            go = FindNearestObjectByTag("Enemy");
        }
        else go = FindNearestObjectByTag("Team");

        if (go != null)
        {
            transform.LookAt(go.transform); //공격대상 바라보기
            if (_rangeAttacktype) //원거리 유닛이면 발사체 생성
            {
                var arrowPool = _creature.GetComponent<ArrowPool>();//ArrowPool컴포넌트를 가지고 와서
                var arrow = arrowPool.GetObject(); //큐에 들어 있는 화살 사용
                if (arrow != null)
                {
                    arrow.transform.position = _creature.transform.position + Vector3.up;
                    arrow.fire(go, _creature);
                }
            }
            CreatureCtrl cc = go.GetComponent<CreatureCtrl>();
            if (cc != null)
                cc.OnDamaged(_atk);
            Debug.Log("On Hit !");
        }
        // 대기 시간
        yield return new WaitForSeconds(1/_atkSpeed);
        _coSkill = null;
        State = CreatureState.Idle; // <- 공격하는게 어색하다 싶으면 Moving으로 바꿔보기
    }

    public virtual void OnDamaged(float damage)
    {
        if (_currentHp > 0)
        {
            _currentHp -= damage;
        }

        else
        {
            State = CreatureState.Dead;
        }
    }

    protected virtual void DefaultStatDBConnection() //유닛 DefaultStatDBConnection Enemy,Ally => o Player => x
    {
    }

    protected void Buff(MeadowUnit mu, bool unBuff) //버프 적용 or 해제
    {
        int num = 1;
        if (!unBuff) //유닛이 죽으면 버프 해제하기 위한 것
            num = -1;

        //버프를 주기 위해 "Team"이라는 태그를 이용해 게임 오브젝트 찾기
        GameObject[] Team = GameObject.FindGameObjectsWithTag("Team");
        if (Team != null)
        {
            foreach (GameObject go in Team)
            {
                CreatureCtrl cc = go.GetComponent<CreatureCtrl>();
                if (cc != null)
                {
                    switch (mu)
                    {
                        case MeadowUnit.Chicken2: //전리품
                            break;
                        case MeadowUnit.Chicken3:
                            break;
                        case MeadowUnit.Cow2:
                            cc.BuffHP(10 * num);
                            break;
                        case MeadowUnit.Cow3:
                            cc.BuffHP(30 * num);
                            break;
                        case MeadowUnit.Duck2: //코인
                            break;
                        case MeadowUnit.Duck3:
                            break;
                        case MeadowUnit.Horse2:
                            cc.buffSpeed(0.1f * num);
                            break;
                        case MeadowUnit.Horse3:
                            cc.buffSpeed(0.3f * num);
                            break;
                        case MeadowUnit.Sheep2:
                            cc.buffAttack(3 * num);
                            break;
                        case MeadowUnit.Goat2:
                            cc.buffAttackSpeed(0.1f * num);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }

    public void BuffHP(int bHP) //체력 버프 
    {
        if (_currentHp + bHP > 0) 
        {
            _hp += bHP;
            _currentHp += bHP;
        }
    }

    public void buffSpeed(float bSpeed) //이속 버프
    {
        _speed += bSpeed;
        //_speed += (1 / 100 * bSpeed);
    }

    public void buffAttack(float bAttack) //공격력 버프
    {
        _atk += bAttack;
    }

    public void buffAttackSpeed(float bAttackSpeed) //공속 버프
    {
        _atkSpeed += bAttackSpeed;
        //_atkSpeed += (1 / 100 * bAttackSpeed);
    }

    /*
     * OrderBy 메소드는 넘겨받은 람다식으로 List를 정렬.
     * 정렬 기준은 Vector3.Distance 메소드로 내 위치와 적 위치의 거리를 계산한 결과.
     * 덕분에 가장 가까운 오브젝트가 첫 번째 요소로 정렬.
     * FirstOrDefault 메소드는 List 의 첫 번째 요소를 반환. 만약 List 가 비어있다면 null을 반환.
     * 최종적으로 neareastEnemy 변수에 가장 가까운 오브젝트가 저장.
     */
    protected GameObject FindNearestObjectByTag(string tag)
    {
        // 탐색할 오브젝트 목록을 List 로 저장
        var objects = GameObject.FindGameObjectsWithTag(tag).ToList();

        // LINQ 메소드를 이용해 가장 가까운 오브젝트를 찾음. <- 빠르다!
        var neareastObject = objects
            .OrderBy(obj =>
            {
                return Vector3.Distance(transform.position, obj.transform.position);
            })
        .FirstOrDefault();

        return neareastObject;
    }
}
