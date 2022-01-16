using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSceneUIManager : Singleton<MainSceneUIManager>
{
    [SerializeField] private RectTransform recttransformCanvas;

    public PopupConfirm popup_confirm;
    public PopupCredit popup_credit;

    protected override void Init()
    {
        recttransformCanvas = GameObject.Find("Main_Canvas").GetComponent<RectTransform>();

        GameObject go = Resources.Load<GameObject>("Prefabs/Prefabs_YJ/Popup_Confirm");
        popup_confirm = Instantiate(go, recttransformCanvas).GetComponent<PopupConfirm>();
        popup_confirm.gameObject.SetActive(false);

        GameObject go1 = Resources.Load<GameObject>("Prefabs/Prefabs_YJ/Popup_Credit");
        popup_credit = Instantiate(go1, recttransformCanvas).GetComponent<PopupCredit>();
        popup_credit.gameObject.SetActive(false);
    }

    public void ShowCredit()
    {
        popup_credit.Show();
    }

    public void HideCredit()
    {
        popup_credit.Hide();
    }
}
