using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUIPositionSetter : MonoBehaviour
{
    [SerializeField]private GameObject ui1;
    [SerializeField] private GameObject ui2;
    private float m_fSpeed = 5.0f;

    void Update()
    {
        //// 테스트를 위한 키보드 이동 시작
        //float fHorizontal = Input.GetAxis("Horizontal");
        //float fVertical = Input.GetAxis("Vertical");

        //transform.Translate(Vector3.right * Time.deltaTime * m_fSpeed * fHorizontal, Space.World);
        //transform.Translate(Vector3.up * Time.deltaTime * m_fSpeed * fVertical, Space.World);
        //// 테스트를 위한 키보드 이동 끝


        // 오브젝트에 따른 HP Bar 위치 이동
        ui1.transform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(-1, 2f, 0));
        ui2.transform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(1, 2f, 0));
    }
    public void Onclick()
    {
        Debug.Log("OnClick");
    }
}
