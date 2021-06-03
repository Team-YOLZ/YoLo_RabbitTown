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
    public Animator player_anim;
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player_anim = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");

            player_anim.SetFloat("Move", Mathf.Abs(moveHorizontal) + Mathf.Abs(moveVertical));

            rb.velocity = new Vector3(moveHorizontal * _speed, 0, moveVertical * _speed);
            rb.rotation = Quaternion.LookRotation(new Vector3(moveHorizontal * _speed, 0, Mathf.Abs(moveVertical) * _speed));
        }
    }
}
