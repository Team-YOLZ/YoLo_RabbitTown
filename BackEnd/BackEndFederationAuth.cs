using BackEnd;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

public class BackEndFederationAuth : MonoBehaviour
{
    void Start()
    {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration
            .Builder()
            .RequestServerAuthCode(false) //뒤끝 파이어베이스 등 구글 토큰 사용하려면 flase로.
            .RequestEmail()     //이메일 요청.
            .RequestIdToken()   //토큰 요청.
            .Build();

        //커스텀된 정보로 GPS 초기화.
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = false;

        //GPGS 시작.
        PlayGamesPlatform.Activate();
        GoogleAuth();
    }

    //구글 로그인.
    private void GoogleAuth()
    {
        if (PlayGamesPlatform.Instance.localUser.authenticated == false)
        {
            Social.localUser.Authenticate(success =>
            {
                if (success == false)
                {
                    Debug.Log("구글 로그인 실패");
                    return;
                }

                // 로그인이 성공되었습니다.
                Debug.Log("GetIdToken - " + PlayGamesPlatform.Instance.GetIdToken());
                Debug.Log("Email - " + ((PlayGamesLocalUser)Social.localUser).Email);
                Debug.Log("GoogleId - " + Social.localUser.id);
                Debug.Log("UserName - " + Social.localUser.userName);
                Debug.Log("UserName - " + PlayGamesPlatform.Instance.GetUserDisplayName());
            });
        }
    }

    //구글 토큰 받아오기.
    private string GetTokens()
    {
        if (PlayGamesPlatform.Instance.localUser.authenticated) //구글 로그인이 되어있는 상태라면.
        {
            //유저 토큰 받기 첫번째 방법.
            string _IDToken = PlayGamesPlatform.Instance.GetIdToken();
            //두번째 방법.
            //string _IDToken = ((PlayGamesLocalUser)Social.localUser).GetIdToken();

            return _IDToken;
        }
        else
        {
            Debug.Log("접속되어 있지 않습니다. 잠시 후 다시 시도하여 주세요.");
            GoogleAuth();
            return null;
        }
    }

    //구글 토큰으로 뒤끝서버 로그인하기 - 동기 방식.
    //public void GPGSLogin()
    //{
    //    // 이미 로그인 된 경우
    //    if (Social.localUser.authenticated == true)
    //    {
    //        BackendReturnObject BRO = Backend.BMember.AuthorizeFederation(GetTokens(), FederationType.Google, "gpgs");
    //    }
    //    else
    //    {
    //        Social.localUser.Authenticate((bool success) => {
    //            if (success)
    //            {
    //                // 로그인 성공 -> 뒤끝 서버에 획득한 구글 토큰으로 가입 요청
    //                BackendReturnObject BRO = Backend.BMember.AuthorizeFederation(GetTokens(), FederationType.Google, "gpgs");
    //            }
    //            else
    //            {
    //                // 로그인 실패
    //                Debug.Log("Login failed for some reason");
    //            }
    //        });
    //    }
    //}

    public void OnClcikGPGSLogin()
    {
        BackendReturnObject BRO = Backend.BMember.AuthorizeFederation(GetTokens(), FederationType.Google, "GPGS로 만든 계정.");
        if (BRO.IsSuccess()) //로그인 성공.
        {
            Debug.Log("구글 토큰으로 뒤끝 서버 로그인 성공.(동기 방식)");
        }
        else //로그인 실패.
        {
            switch (BRO.GetStatusCode()) //실패 사유.
            {
                case "200":
                    Debug.Log("이미 회원가입 완료.");
                    break;

                case "403":
                    Debug.Log("차단 유저입니다. 차단 사유 : " + BRO.GetErrorCode());
                    break;

                default:
                    Debug.Log("서버 공통 에러 발생. " + BRO.GetMessage());
                    break;
            }
        }
    }

    //이미 가입한 회원의 이메일 정보 저장.
    public void OnClickUpdateEmail()
    {
        BackendReturnObject BRO = Backend.BMember.UpdateFederationEmail(GetTokens(), FederationType.Google);
        if (BRO.IsSuccess())
        {
            Debug.Log("이메일 저장 완료.");
        }
        else
        {
            if (BRO.GetStatusCode() == "404") Debug.Log("federation ID를 찾을 수 없습니다.");
        }
    }

    //이미 가입한 유저인지 확인 => (이 때 이미 가입한 유저라면 바로 로그인 로직으로 넘어가야 하나 고민중.)
    public void OnClickCheckUserAuthenticate()
    {
        BackendReturnObject BRO = Backend.BMember.CheckUserInBackend(GetTokens(), FederationType.Google);
        if (BRO.GetStatusCode() == "200")
        {
            Debug.Log("가입중인 계정입니다.");

            //해당 계정 정보.
            Debug.Log(BRO.GetReturnValue());
        }
        else
        {
            Debug.Log("가입된 계정이 아닙니다.");
        }
    }

    //커스텀 계정을 패더레이션 계정으로 변경.
    public void OnClickChangeCustomToFederation()
    {
        BackendReturnObject BRO = Backend.BMember.ChangeCustomToFederation(GetTokens(), FederationType.Google);
        if (BRO.IsSuccess())
        {
            Debug.Log("패더레이션 계정으로 변경 완료.");
        }
        else
        {
            switch (BRO.GetStatusCode())
            {
                case "400":
                    if (BRO.GetErrorCode() == "BadParameterException")
                    {
                        Debug.Log("이미 변경 완료 되었는데 다시 시도한 경우.");
                    }

                    else if (BRO.GetErrorCode() == "UndefinedParameterException")
                    {
                        Debug.Log("커스텀 로그인 하지 않고 시도한 경우.");
                    }
                    break;

                case "409":
                    //이미 가입되어 있는 경우.
                    Debug.Log("중복된 fedetationId 입니다.");
                    break;

                default:
                    Debug.Log("서버 공통 에러 발생. " + BRO.GetMessage());
                    break;
            }
        }
    }
}
