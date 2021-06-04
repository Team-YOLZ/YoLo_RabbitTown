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
    public GameObject obj_playerCameraArm; //cinemachine Fallow Target
    Animator player_anim;
    Rigidbody rb;

    void Start()
    {
        rb = obj_player.GetComponent<Rigidbody>();
        player_anim = obj_player.GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        if (Application.platform == RuntimePlatform.WindowsEditor) //Editor에서 테스트하기 편하도록
        {
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");

            player_anim.SetFloat("Move", Mathf.Abs(moveHorizontal) + Mathf.Abs(moveVertical)); //애니메이션 SetTrigger -> Float으로 변경

            rb.velocity = new Vector3(moveHorizontal * _speed, 0, moveVertical * _speed);            
        }

        //CameraArm의 경우 Player보다 살짝 위로 두고 Player와 Rotation은 공유하지 않음
        obj_playerCameraArm.transform.position = new Vector3(player_anim.transform.position.x, player_anim.transform.position.y + 3.5f, player_anim.transform.position.z);
    }
}