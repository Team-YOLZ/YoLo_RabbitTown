using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
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
    public Transform playertr;

    public GameObject ObstacleMinHeight;
    [SerializeField] List<Renderer> list_Obstacle = new List<Renderer>(); //플레이어를 가리는 오브젝트의 Renderer
    BackEndGetTable GetPlayerStatData;

    private int _leadership;
    private int _appeal;

    public GameObject _captureButton;
    public GameObject _getSpoilButton;

    //BackEnd 연동 위한 Get String My ID
    private string OwnerIndateKey;

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
        //Player가 가지고 있는 Creature Name,Count 조회.
        Where where = new Where();
        where.Equal("owner_inDate", GetPlayerStatData.playerStat.OwnerIndate);
        var bro = Backend.GameData.GetMyData("OwnUnitTable", where, 50);
        Debug.Log(bro.GetReturnValuetoJSON()["rows"].Count);
        for(int i=0; i< bro.GetReturnValuetoJSON()["rows"].Count; i++) //(임시코드)가지고 있는 Creature 수까지 반복. 게임씬에는 생성.디비 테이블에선 삭제.
        {
            string Name = bro.Rows()[0]["Name"]["S"].ToString();
            GameObject go = Managers.Resource.Instantiate($"Creature_YJ/{Name}");
            Destroy(go.GetComponent<EnemyCtrl>());
            AllyCtrl AC = go.AddComponent<AllyCtrl>() as AllyCtrl;
            AC.enabled = true;
            Backend.GameData.Delete("OwnUnitTable", where);
        }
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
        if (Input.GetKeyDown(KeyCode.Q)) GoMain(); //(임시) 메인씬 가는 코드.
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
        GameObject go = FindNearestObjectByTag("IsDeadEnemy"); //  죽어있는상태 가장 가까운 적.
        Destroy(go.GetComponent<EnemyCtrl>());
        AllyCtrl AC = go.AddComponent<AllyCtrl>() as AllyCtrl;
        AC.enabled = true;
        go.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f); //다시 크기 복귀.
        OffCaptureButton(); //버튼 off
        OffGetSpoilButton(); //버튼 off
    }

    public void OnClickGetSpoilButton()
    {
        GameObject go = FindNearestObjectByTag("IsDeadEnemy"); //죽어있는상태 가장 가까운 적(Select).
        EnemyCtrl enemyCtrl = go.GetComponent<EnemyCtrl>(); // Select된 적 정보 Get.
        //Get된 정보에 따른 전리품 획득 로직.
        switch(enemyCtrl._spoilnumber)
        {
            case 1:
                GetPlayerStatData.playerAsset.Spoil1 += enemyCtrl._spoilamount;
                break;
            case 2:
                GetPlayerStatData.playerAsset.Spoil2 += enemyCtrl._spoilamount;
                break;
            case 3:
                GetPlayerStatData.playerAsset.Spoil3 += enemyCtrl._spoilamount;
                break;
            default:
                break;
        }
        //Enemy pool로 돌아가는 로직 임시로 Destroy.
        Destroy(go);
        OffCaptureButton(); //버튼 off
        OffGetSpoilButton(); //버튼 off
    }

    public void GoMain() // 임시로 메인씬으로 돌아가는 로직, 디비 값 조정.
    {
        GetPlayerStatData.GetComponent<BackEndGetTable>().AssetUpdate(); //자산 테이블 업데이트.
        GetPlayerStatData.GetComponent<BackEndGetTable>().KillCountUpdate();//킬 카운트 테이블 업데이트.
        InsertOwnUnitTable(); //OwnCreature Table 업데이트.
        Managers.Scene.LoadScene(Define.Scene.Main);
    }

    public void OnApplicationQuit() //게임씬에서 앱 강제 종료시 호출되는 함수, 디비 값 조정.
    {
        GetPlayerStatData.GetComponent<BackEndGetTable>().AssetUpdate(); //자산테이블 업데이트.
        GetPlayerStatData.GetComponent<BackEndGetTable>().KillCountUpdate();//킬 카운트 테이블 업데이트.
        InsertOwnUnitTable(); //OwnCreature Table 업데이트. 
    }

    public void InsertOwnUnitTable()
    {
        // Param은 뒤끝 서버와 통신을 할 떄 넘겨주는 파라미터 클래스 입니다.
        GameObject[] Ally = GameObject.FindGameObjectsWithTag("Team"); //동료 숫자 파악.
        for (int i = 0; i < Ally.Length; i++)
        {
            if (Ally[i].gameObject.name != "player")
            {
                Param param = new Param();
                param.Add("Name", Ally[i].gameObject.name);

                BackendReturnObject BRO = Backend.GameData.Insert("OwnUnitTable", param);
                if (BRO.IsSuccess())
                {
                    Debug.Log("indate : " + BRO.GetInDate());
                }
                else
                {
                    switch (BRO.GetStatusCode())
                    {
                        case "404":
                            Debug.Log("존재하지 않는 tableName인 경우");
                            break;

                        case "412":
                            Debug.Log("비활성화 된 tableName인 경우");
                            break;

                        case "413":
                            Debug.Log("하나의 row( column들의 집합 )이 400KB를 넘는 경우");
                            break;

                        default:
                            Debug.Log("서버 공통 에러 발생: " + BRO.GetMessage());
                            break;
                    }
                }
            }
        }
    }
}