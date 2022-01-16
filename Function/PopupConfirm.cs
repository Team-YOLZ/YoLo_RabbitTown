using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupConfirm : Popup
{
    [SerializeField] private Text text_content;
    [SerializeField] private Button button_yes;
    [SerializeField] private Button button_no;
    [SerializeField] private Button button_confirm;

    BackEndGetTable tableData;
    MainScene mainSceneData;

    private void Start()
    {
        tableData = FindObjectOfType<BackEndGetTable>();
        mainSceneData = FindObjectOfType<MainScene>();
    }

    private Action upgradeCallback;

    public void ConfirmShow(bool _canUpgrade, int _need, string _type, Action _action = null, int _needamount = 0)
    {
        Show();

        if (_canUpgrade)
        {
            if (_type == "전리품2")
                text_content.text = $"정말 {_type}를 {_need}만큼 사용하시겠습니까?";
            else
                text_content.text = $"정말 {_type}을 {_need}만큼 사용하시겠습니까?";

            upgradeCallback = _action;

            //팝업 두개 쓰기 싫어서 재사용.
            button_yes.gameObject.SetActive(true);
            button_no.gameObject.SetActive(true);
            button_confirm.gameObject.SetActive(false);

        }
        else
        {
            if(_type == "전리품2")
                text_content.text = $"{_type}가 {_needamount}만큼 부족합니다.";
            else
                text_content.text = $"{_type}이 {_needamount}만큼 부족합니다.";

            button_yes.gameObject.SetActive(false);
            button_no.gameObject.SetActive(false);
            button_confirm.gameObject.SetActive(true);
        }
    }

    public void OnClick_Yes()
    {
        if (upgradeCallback != null)
            HideCallback();
        else
            base.Hide();
    }

    public void HideCallback()
    {
        base.Hide();
        upgradeCallback(); //콜백 실행
        mainSceneData.EditPage();

        upgradeCallback = null;
    }
}

