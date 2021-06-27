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
    public float _speed = 10f;
    public GameObject obj_player;        
    public GameObject ObstacleMinHeight;        
    Animator player_anim;
    Rigidbody rb;
    public GameObject[] enemy1;
    [SerializeField] List<Renderer> list_Obstacle = new List<Renderer>(); //플레이어를 가리는 오브젝트의 Renderer


    public bool _enemyInAttackRange; //怨듦꺽 ш굅由 댁 � ?
    public LayerMask _whatIsEnemy; //enemy �댁


    protected override void Init()
    {
        _creature = gameObject;
        _whatIsEnemy = 1 << LayerMask.NameToLayer("Enemy");

        CustomPlayerDBConnection(); //�댁 λμ
    }
    protected override void Init2()
    {
        rb = _creature.GetComponent<Rigidbody>();
        base.Init2();
    }

    protected override void UpdateAnimation() //怨듦꺽怨 二쎌留 硫댁 援ы
    {
        if (_state == CreatureState.Skill)
        {
            _animator.Play("Attack");
        }
        else if(_state == CreatureState.Dead)
        {
            _animator.Play("Die");
        }

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
    protected override void UpdateController()
    {
        _enemyInAttackRange = Physics.CheckSphere(transform.position, _attackRange, _whatIsEnemy);

        if (_enemyInAttackRange)
            State = CreatureState.Skill;
        base.UpdateController();
    }
    protected override void UpdateDead()
    {
       //�댁 二쎌  濡吏
    }

    private void CustomPlayerDBConnection()
    {
        //�댁 λμ 鍮 곌껐 
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, _attackRange); //black : 怨듦꺽 ш굅由
    }

}

//怨듦꺽ш굅由 洹쇱留 쇰㈃ 怨듦꺽. 