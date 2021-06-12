using BackEnd;
using UnityEngine;

public class BackEndInitialize : MonoBehaviour
{
    void Start()
    {
        Backend.Initialize(BRO =>
        {
            Debug.Log("뒤끝 초기화 진행 " + BRO);

            // 성공
            if (BRO.IsSuccess())
            {
                // 해쉬키 
                Debug.Log(Backend.Utils.GetGoogleHash());
            }

            // 실패
            else
            {
                Debug.LogError("초기화 실패: " + BRO.GetErrorCode());
            }
        });
    }
}