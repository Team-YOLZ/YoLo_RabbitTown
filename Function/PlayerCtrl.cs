using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class PlayerCtrl : CreatureCtrl 
{
    public static PlayerCtrl Instance //sigleton
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<PlayerCtrl>();
                if (Instance == null)
                {
                    var instanceContainer = new GameObject("PlayerCtrl");
                    instance = instanceContainer.AddComponent<PlayerCtrl>();
                }
            }
            return instance;
        }
    }

    private static PlayerCtrl instance;
    Rigidbody rb;

    public bool _enemyInAttackRange; //공격 사거리 내에 적이 있나요?
    public LayerMask _whatIsEnemy; //enemy 레이어

    public GameObject ObstacleMinHeight;
    [SerializeField] List<Renderer> list_Obstacle = new List<Renderer>(); //플레이어를 가리는 오브젝트의 Renderer

    protected override void Init()
    {
        _creature = gameObject;
        _whatIsEnemy = 1 << LayerMask.NameToLayer("Enemy");

        CustomPlayerDBConnection(); //플레이어 능력치
    }
    protected override void Init2()
    {
        rb = _creature.GetComponent<Rigidbody>();
        base.Init2();
    }

    protected override void UpdateAnimation() //공격과 죽음만 애니메이션 구현
    {
        if (_state == CreatureState.Skill)
        {
            _animator.Play("Attack");
        }
        else if(_state == CreatureState.Dead)
        {
            _animator.Play("Die");
        }
    }
    protected override void UpdateController()
    {
        _enemyInAttackRange = Physics.CheckSphere(transform.position, _attackRange, _whatIsEnemy);

        if (_enemyInAttackRange)
            State = CreatureState.Skill;
        base.UpdateController();

        float Dis = Vector3.Distance(Camera.main.transform.position, ObstacleMinHeight.transform.position);
        Vector3 Dir = (ObstacleMinHeight.transform.position - Camera.main.transform.position).normalized;

        if (Physics.Raycast(Camera.main.transform.position, Dir, out RaycastHit hit, Dis))
        {
            if (hit.collider.CompareTag("Obstacle"))
            {
                Renderer check = hit.collider.gameObject.GetComponent<Renderer>();
                if (!list_Obstacle.Contains(check)) list_Obstacle.Add(check);

                ObstacleCollision();
            }
            else
            {
                if (list_Obstacle.Count != 0) ReleaseAlpha();
            }
        }
    }
    protected override void UpdateDead()
    {
       //플레이어 죽었을 때 로직
    }

    private void CustomPlayerDBConnection()
    {
        //플레이어 능력치 디비랑 연결 
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, _attackRange); //black : 공격 사거리
    }

    void ObstacleCollision()
    {
        foreach (Renderer index in list_Obstacle)
        {
            index.material.SetFloat("_Alpha", 0.5f);
        }
    }

    void ReleaseAlpha()
    {
        foreach (Renderer index in list_Obstacle)
        {
            index.material.SetFloat("_Alpha", 1f);
        }

        list_Obstacle.Clear();
    }
}

//공격사거리 근처에만 있으면 공격. 

