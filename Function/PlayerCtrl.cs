using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    BackEndGetTable GetPlayerStatData;

    private int _leadership;
    private int _appeal;
    //new private int _attackRange = 4;

    public GameObject _captureButton;
    public GameObject _getSpoilButton;

    protected override void Init()
    {
        _creature = gameObject;
        _whatIsEnemy = 1 << LayerMask.NameToLayer("Enemy");

        GetPlayerStatData = GameObject.Find("UserTableInformation").GetComponent<BackEndGetTable>();
        DefaultStatDBConnection(); //플레이어 능력치 적용.
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
        else if (_state == CreatureState.Dead)
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
        if (Input.GetKeyDown(KeyCode.Q)) GoMain();
    }

    protected override void UpdateDead()
    {
       //플레이어 죽었을 때 로직
    }

    protected override void DefaultStatDBConnection()
    {
        //플레이어 능력치 디비랑 연결
        _atk = GetPlayerStatData.playerStat.Attack;
        _atkSpeed = GetPlayerStatData.playerStat.AttackSpeed;
        _speed = GetPlayerStatData.playerStat.MovingSpeed;
        _hp = GetPlayerStatData.playerStat.Hp;
        _leadership = GetPlayerStatData.playerStat.Leadership;
        _appeal = GetPlayerStatData.playerStat.Appeal;
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

    public void OnCaptureButton() 
    {
        _captureButton.SetActive(true);
    }

    public void OnGetSpoilButton()
    {
        _getSpoilButton.SetActive(true);
    }

    public void OffCaptureButton()
    {
        _captureButton.SetActive(false);
    }

    public void OffGetSpoilButton()
    {
        _getSpoilButton.SetActive(false);
    }

    public void OnClickCaptureButton()
    {
        int AllyCount = GameObject.FindGameObjectsWithTag("Team").Length; //동료 숫자 파악.
        //포획 불가능.
        if (AllyCount-1 >= GetPlayerStatData.playerStat.Leadership) //동료의 숫자가 Leadership 숫자를 넘지않도록, -1은 Player 자신.
        {
            return;
        }
        //포획 가능.
        GameObject go = FindNearestObjectByTag("Enemy1"); //  죽어있는상태 가장 가까운 적.
        Destroy(go.GetComponent<EnemyCtrl>());
        AllyCtrl AC = go.AddComponent<AllyCtrl>() as AllyCtrl;
        AC.enabled = true;
        go.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f); //다시 크기 복귀.
        OffCaptureButton(); //버튼 off
        OffGetSpoilButton(); //버튼 off
    }

    public void OnClickGetSpoilButton()
    {
        GameObject go = FindNearestObjectByTag("Enemy1"); //  죽어있는상태 가장 가까운 적.
        Destroy(go);
        GetPlayerStatData.playerAsset.Spoil1 += 1;
        //Enemy pool로 돌아가는 로직 임시로 Destroy.
        OffCaptureButton(); //버튼 off
        OffGetSpoilButton(); //버튼 off
    }

    public void GoMain() // 임시로 메인씬으로 돌아가는 로직, 디비 값 조정.
    {
        GetPlayerStatData.GetComponent<BackEndGetTable>().AssetUpdate(); //자산테이블 업데이트.블

        Managers.Scene.LoadScene(Define.Scene.Main);
    }

    public void OnApplicationQuit() //게임씬에서 앱 강제 종료시 호출되는 함수, 디비 값 조정.
    {
        GetPlayerStatData.GetComponent<BackEndGetTable>().AssetUpdate(); //자산테이블 업데이트.
    }
}

//공격사거리 근처에만 있으면 공격. 

