using UnityEngine;
using BackEnd;

public class BackEndGetTable : MonoBehaviour
{
    public PlayerStatData playerStat;
    public PlayerAssetData playerAsset;
    public PlayerKillData playerKillData;

    private string StatTableKey;
    private string AssetTableKey;
    private string CountTableKey;

    private int needspoil1;
    private int needspoil2;

    private void Awake()
    {
        InitUserStatTable(); //스탯 테이블 입력.
        InitUserAssetTable(); //자산 테이블 입력.
        InitCaptureCountTable(); //킬카운트 테이블 입력.

        Debug.Log(StatTableKey);
        BackendReturnObject bro = Backend.BMember.GetUserInfo();
        playerStat.OwnerIndate = bro.GetReturnValuetoJSON()["row"]["inDate"].ToString();
    }

    //유저 스탯 테이블 init.
    public void InitUserStatTable()
    {
        var bro = Backend.GameData.GetMyData("UserStatTable", new Where(), 10);
        string OwnerIndate = "";
        if (bro.IsSuccess() == false)
        {
            // 요청 실패 처리
            Debug.Log(bro);
            return;
        }
        if (bro.GetReturnValuetoJSON()["rows"].Count <= 0)
        {
            // 요청이 성공해도 where 조건에 부합하는 데이터가 없을 수 있기 때문에
            // 데이터가 존재하는지 확인
            // 위와 같은 new Where() 조건의 경우 테이블에 row가 하나도 없으면 Count가 0 이하 일 수 있다.
            Debug.Log(bro);
            return;
        }
        //내 테이블의 rowindate 할당.
        for (int i = 0; i < bro.Rows().Count; ++i)
        {
            OwnerIndate = bro.Rows()[i]["inDate"]["S"].ToString();
        }
        string[] select = { "Attack", "AttackSpeed", "MovingSpeed", "Hp", "Leadership", "Appeal", "owner_inDate" };

        // 테이블 내 해당 rowIndate를 지닌 row를 조회
        // select에 존재하는 컬럼만 리턴
        var BRO = Backend.GameData.Get("UserStatTable", OwnerIndate, select); //BackEnd Return 데이터 Get.
        var json = BRO.GetReturnValuetoJSON(); //BackEnd Return 데이터 => JSON 데이터로 변환
        playerStat = new PlayerStatData(json); //PlayerData 클래스에 할당.
        StatTableKey = OwnerIndate;
    }

    //유저 자산 테이블 init.
    public void InitUserAssetTable()
    {
        var bro = Backend.GameData.GetMyData("UserAssetTable", new Where(), 10);
        string OwnerIndate = "";
        if (bro.IsSuccess() == false)
        {
            // 요청 실패 처리
            Debug.Log(bro);
            return;
        }
        if (bro.GetReturnValuetoJSON()["rows"].Count <= 0)
        {
            // 요청이 성공해도 where 조건에 부합하는 데이터가 없을 수 있기 때문에
            // 데이터가 존재하는지 확인
            // 위와 같은 new Where() 조건의 경우 테이블에 row가 하나도 없으면 Count가 0 이하 일 수 있다.
            Debug.Log(bro);
            return;
        }
        //내 테이블의 rowindate 할당.
        for (int i = 0; i < bro.Rows().Count; ++i)
        {
            OwnerIndate = bro.Rows()[i]["inDate"]["S"].ToString();
        }
        string[] select = { "Coin", "Spoil1", "Spoil2" };

        // 테이블 내 해당 rowIndate를 지닌 row를 조회
        // select에 존재하는 컬럼만 리턴
        var BRO = Backend.GameData.Get("UserAssetTable", OwnerIndate, select); //BackEnd Return 데이터 Get.
        var json = BRO.GetReturnValuetoJSON(); //BackEnd Return 데이터 => JSON 데이터로 변환
        playerAsset = new PlayerAssetData(json); //PlayerData 클래스에 할당.
        AssetTableKey = OwnerIndate;
    }

    //유저 킬카운트 테이블 init.
    public void InitCaptureCountTable()
    {
        var bro = Backend.GameData.GetMyData("UnitCaptureCount", new Where(), 10);
        string OwnerIndate = "";
        if (bro.IsSuccess() == false)
        {
            // 요청 실패 처리
            Debug.Log(bro);
            return;
        }
        if (bro.GetReturnValuetoJSON()["rows"].Count <= 0)
        {
            // 요청이 성공해도 where 조건에 부합하는 데이터가 없을 수 있기 때문에
            // 데이터가 존재하는지 확인
            // 위와 같은 new Where() 조건의 경우 테이블에 row가 하나도 없으면 Count가 0 이하 일 수 있다.
            Debug.Log(bro);
            return;
        }
        //내 테이블의 rowindate 할당.
        for (int i = 0; i < bro.Rows().Count; ++i)
        {
            OwnerIndate = bro.Rows()[i]["inDate"]["S"].ToString();
        }
        string[] select = { "Chicken1", "Chicken2", "Chicken3", "Cow1", "Cow2", "Cow3",
             "Horse1", "Horse2", "Horse3", "Goat1", "Goat2","Duck1", "Duck2", "Duck3", "Sheep1", "Sheep2" };

        // 테이블 내 해당 rowIndate를 지닌 row를 조회
        // select에 존재하는 컬럼만 리턴
        var BRO = Backend.GameData.Get("UnitCaptureCount", OwnerIndate, select); //BackEnd Return 데이터 Get.
        var json = BRO.GetReturnValuetoJSON(); //BackEnd Return 데이터 => JSON 데이터로 변환
        playerKillData = new PlayerKillData(json); //PlayerData 클래스에 할당.
        CountTableKey = OwnerIndate;
    }

    //공격력 증가 버튼.
    public void UpAttackButton()
    {
        if (ConsumptionCoin())
            MainSceneUIManager.Instance.popup_confirm.ConfirmShow(true, 10, "Coin", UpAttackStat);
        else
            MainSceneUIManager.Instance.popup_confirm.ConfirmShow(false, 10, "Coin", null, 10 - playerAsset.Coin);
    }

    public void UpAttackSpeedButton()
    {
        if (ConsumptionCoin())
            MainSceneUIManager.Instance.popup_confirm.ConfirmShow(true, 10, "Coin", UpAttackSpeedStat);
        else
            MainSceneUIManager.Instance.popup_confirm.ConfirmShow(false, 10, "Coin", null, 10 - playerAsset.Coin);
    }

    public void UpMoveSpeedButton()
    {
        if (ConsumptionCoin())
            MainSceneUIManager.Instance.popup_confirm.ConfirmShow(true, 10, "Coin", UpMoveSpeedStat);
        else
            MainSceneUIManager.Instance.popup_confirm.ConfirmShow(false, 10, "Coin", null, 10 - playerAsset.Coin);
    }

    public void UpHPButton()
    {
        if (ConsumptionCoin())
            MainSceneUIManager.Instance.popup_confirm.ConfirmShow(true, 10, "Coin", UpHPStat);
        else
            MainSceneUIManager.Instance.popup_confirm.ConfirmShow(false, 10, "Coin", null, 10 - playerAsset.Coin);
    }

    public void UpLeadershipButton()
    {
        if (ConsumptionSpoil1())
            MainSceneUIManager.Instance.popup_confirm.ConfirmShow(true, needspoil1, "전리품1", UpLeaderShipStat);
        else
            MainSceneUIManager.Instance.popup_confirm.ConfirmShow(false, needspoil1, "전리품1", null, needspoil1 - playerAsset.Spoil1);
    }

    public void UpAppealButton()
    {
        if (ConsumptionSpoil2())
            MainSceneUIManager.Instance.popup_confirm.ConfirmShow(true, needspoil2, "전리품2", UpAppealStat);
        else
            MainSceneUIManager.Instance.popup_confirm.ConfirmShow(false, needspoil2, "전리품2", null, needspoil2 - playerAsset.Spoil2);
    }


    //임시 공격력 증가 함수.
    private void UpAttackStat()
    {
        if (playerAsset.Coin >= 10) //콜백 두번 넘어갈 경우 방어코드
        {
            Param updateParam = new Param();
            updateParam.AddCalculation("Attack", GameInfoOperator.addition, 10); // 기존 데이터 Attack에서 10만큼 더 추가

            Backend.GameData.UpdateWithCalculation("UserStatTable", new Where(), updateParam); //백엔드 UserTable에 적용.

            InitUserStatTable();
            InitUserAssetTable();
        }
        return;
    }

    private void UpAttackSpeedStat()
    {
        if (playerAsset.Coin >= 10)
        {
            Param updateParam = new Param();
            updateParam.AddCalculation("AttackSpeed", GameInfoOperator.multiplication, 1.1f); // 기존 데이터 Attack에서 10만큼 더 추가

            Backend.GameData.UpdateWithCalculation("UserStatTable", new Where(), updateParam); //백엔드 UserTable에 적용.


            InitUserStatTable();
            InitUserAssetTable();
        }
        return;
    }

    private void UpMoveSpeedStat()
    {
        if (playerAsset.Coin >= 10)
        {
            Param updateParam = new Param();
            updateParam.AddCalculation("MovingSpeed", GameInfoOperator.multiplication, 1.1f); // 기존 데이터 Attack에서 10만큼 더 추가

            Backend.GameData.UpdateWithCalculation("UserStatTable", new Where(), updateParam); //백엔드 UserTable에 적용.


            InitUserStatTable();
            InitUserAssetTable();
        }
        return;
    }

    private void UpHPStat()
    {
        if (playerAsset.Coin >= 10)
        {
            Param updateParam = new Param();
            updateParam.AddCalculation("Hp", GameInfoOperator.addition, 20); // 기존 데이터 Attack에서 10만큼 더 추가

            Backend.GameData.UpdateWithCalculation("UserStatTable", new Where(), updateParam); //백엔드 UserTable에 적용.


            InitUserStatTable();
            InitUserAssetTable();
        }
        return;
    }

    private void UpLeaderShipStat()
    {
        if (playerAsset.Spoil1 >= needspoil1)
        {
            Param updateParam = new Param();
            updateParam.AddCalculation("Leadership", GameInfoOperator.addition, 1); // 기존 데이터 Attack에서 10만큼 더 추가

            Backend.GameData.UpdateWithCalculation("UserStatTable", new Where(), updateParam); //백엔드 UserTable에 적용.


            InitUserStatTable();
            InitUserAssetTable();
        }
        return;
    }

    private void UpAppealStat()
    {
        if (playerAsset.Spoil2 >= needspoil2)
        {
            Param updateParam = new Param();
            updateParam.AddCalculation("Appeal", GameInfoOperator.addition, 1); // 기존 데이터 Attack에서 10만큼 더 추가

            Backend.GameData.UpdateWithCalculation("UserStatTable", new Where(), updateParam); //백엔드 UserTable에 적용.


            InitUserStatTable();
            InitUserAssetTable();
        }
        return;
    }

    //임시 스탯 증가 비용 지불.
    private bool ConsumptionCoin()
    {
        Param updateParam = new Param();

        if (playerAsset.Coin >= 10)
        {
            updateParam.AddCalculation("Coin", GameInfoOperator.subtraction, 10); // 기존 데이터 Attack에서 10만큼 더 추가
            Backend.GameData.UpdateWithCalculation("UserAssetTable", new Where(), updateParam); //백엔드 UserTable에 적용.
            return true;
        }
        return false;
    }

    private bool ConsumptionSpoil1()
    {
        Param updateParam = new Param();

        needspoil1 = (playerStat.Leadership - 1) * 2;
        if (playerAsset.Spoil1 >= needspoil1)
        {
            updateParam.AddCalculation("Spoil1", GameInfoOperator.subtraction, needspoil1);
            Backend.GameData.UpdateWithCalculation("UserAssetTable", new Where(), updateParam); //백엔드 UserTable에 적용.
            return true;
        }
        return false;
    }

    private bool ConsumptionSpoil2()
    {
        Param updateParam = new Param();

        needspoil2 = (playerStat.Appeal + 1) * 2;
        if (playerAsset.Spoil2 >= needspoil2)
        {
            updateParam.AddCalculation("Spoil2", GameInfoOperator.subtraction, needspoil2);
            Backend.GameData.UpdateWithCalculation("UserAssetTable", new Where(), updateParam); //백엔드 UserTable에 적용.
            return true;
        }
        return false;
    }

    public void AssetUpdate()
    {
        Param updateParam = new Param();
        updateParam.Add("Coin", playerAsset.Coin);
        updateParam.Add("Spoil1", playerAsset.Spoil1);
        updateParam.Add("Spoil2", playerAsset.Spoil2);

        Backend.GameData.Update("UserAssetTable", AssetTableKey, updateParam);
    }

    public void KillCountUpdate()
    {
        Param updateParam = new Param();
        updateParam.Add("Chicken1", playerKillData.Chicken1);
        updateParam.Add("Chicken2", playerKillData.Chicken2);
        updateParam.Add("Chicken3", playerKillData.Chicken3);
        updateParam.Add("Cow1", playerKillData.Cow1);
        updateParam.Add("Cow2", playerKillData.Cow2);
        updateParam.Add("Cow3", playerKillData.Cow1);
        updateParam.Add("Goat1", playerKillData.Goat1);
        updateParam.Add("Goat2", playerKillData.Goat2);
        updateParam.Add("Horse1", playerKillData.Horse1);
        updateParam.Add("Horse2", playerKillData.Horse2);
        updateParam.Add("Horse3", playerKillData.Horse3);


        Backend.GameData.Update("UnitCaptureCount", CountTableKey, updateParam);
    }
}
