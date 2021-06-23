using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
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

    void Start()
    {
        rb = obj_player.GetComponent<Rigidbody>();
        player_anim = obj_player.GetComponent<Animator>();
        enemy1 = GameObject.FindGameObjectsWithTag("Enemy1");//임시 공격 코드 위한 Search문.
    }

    private void Update()
    {
        //임시 공격 코드.
        if (Input.GetKeyDown(KeyCode.A))
        {
            for (int i = 0; i < enemy1.Length; i++)
            {
                enemy1[i].GetComponent<EnemyCtrl>().TakeDamage(5);
            }
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
}