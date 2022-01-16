using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupSceneConfirm : Popup
{
    [SerializeField] private Text text_content;
    [SerializeField] private Button button_yes;
    [SerializeField] private Button button_no;
    [SerializeField] private Button button_confirm;

    private Action sceneCallback;
 
    public void GoSceneConfirmShow(string _type, Action _action = null) //씬 
    {
        Show();
        switch (_type)
        {
            case "Main":
                text_content.text = "정말 메인화면으로 돌아가겠습니까?";
                sceneCallback = _action;
                break;
            default:
                text_content.text = "Error..";
                break;
        }
        button_yes.gameObject.SetActive(true);
        button_no.gameObject.SetActive(true);
        button_confirm.gameObject.SetActive(false);
    }

    public void OnClick_Scene_Yes() //씬
    {
        if (sceneCallback != null)
            HideSceneCallback();
        else
            base.HideTimeScale();
    }

    public void HideSceneCallback() //씬 
    {
        base.HideTimeScale();
        sceneCallback();

        sceneCallback = null;
    }
}

