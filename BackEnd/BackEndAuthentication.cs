using UnityEngine;
using UnityEngine.UI;
using BackEnd;

public class BackEndAuthentication : MonoBehaviour
{
    public InputField idInput;
    public InputField paInput;

    // 회원가입1 - 동기 방식
    public void OnClickSignUp()
    {
        // 회원 가입을 한뒤 결과를 BackEndReturnObject 타입으로 반환한다.
        string error = Backend.BMember.CustomSignUp(idInput.text, paInput.text, "Test1").GetErrorCode();

        // 회원 가입 실패 처리
        switch (error)
        {
            case "DuplicatedParameterException":
                Debug.Log("중복된 customId 가 존재하는 경우");
                break;

            default:
                Debug.Log("회원 가입 완료");
                break;
        }

        Debug.Log("동기 방식============================================= ");
    }

    public void OnClickLogin1()
    {
        string error = Backend.BMember.CustomLogin(idInput.text, paInput.text).GetErrorCode();

        // 로그인 실패 처리
        switch (error)
        {
            // 아이디 또는 비밀번호가 틀렸을 경우
            case "BadUnauthorizedException":
                Debug.Log("아이디 또는 비밀번호가 틀렸다.");
                break;


            case "BadPlayer":  //  이 경우 콘솔에서 입력한 차단된 사유가 에러코드가 된다.
                Debug.Log("차단된 유저");
                break;

            default:
                Debug.Log("로그인 완료");
                break;
        }
        Debug.Log("동기 방식============================================= ");
    }
    //자동 로그인 기능.
    public void AutoLogin()
    {
        string error = Backend.BMember.LoginWithTheBackendToken().GetErrorCode();

        switch (error)
        {
            // 토근 기간 만료
            case "GoneResourceException":
                Debug.Log("1년뒤 refresh_token이 만료된 경우");
                break;

            // 토근 조건부 만료
            case "BadUnauthorizedException":
                Debug.Log("다른 기기로 로그인 하여 refresh_token이 만료된 경우");
                break;

            case "BadPlayer":  //  이 경우 콘솔에서 입력한 차단된 사유가 에러코드가 된다.
                Debug.Log("차단된 유저");
                break;

            default:
                Debug.Log("로그인 완료");
                break;
        }

        Debug.Log("동기 방식============================================= ");
    }
}

