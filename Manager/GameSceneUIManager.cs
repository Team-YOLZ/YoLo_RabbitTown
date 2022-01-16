using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSceneUIManager : Singleton<GameSceneUIManager>
{
    [SerializeField] private RectTransform recttransformCanvas;
    [SerializeField] private Camera camera_Minimap;

    public PopupMinimap popup_minimap;
    public PopupSceneConfirm popup_scene_confirm;

    [SerializeField] private Text textCoin;
    [SerializeField] private Text textSpoil1;
    [SerializeField] private Text textSpoi2;

    BackEndGetTable tableData;

    protected override void Init()
    {
        recttransformCanvas = GameObject.Find("GameScene_Canvas").GetComponent<RectTransform>();
        tableData = FindObjectOfType<BackEndGetTable>();

        GameObject go = Resources.Load<GameObject>("Prefabs/Prefabs_YJ/Popup_Minimap");
        popup_minimap = Instantiate(go, recttransformCanvas).GetComponent<PopupMinimap>();
        popup_minimap.gameObject.SetActive(false);

        GameObject go1 = Resources.Load<GameObject>("Prefabs/Prefabs_YJ/Popup_Scene_Confirm");
        popup_scene_confirm = Instantiate(go1, recttransformCanvas).GetComponent<PopupSceneConfirm>();
        popup_scene_confirm.gameObject.SetActive(false);
    }

    public void Update()
    {
        textCoin.text = $": {tableData.playerAsset.Coin}";
        textSpoil1.text = $": {tableData.playerAsset.Spoil1}";
        textSpoi2.text = $": {tableData.playerAsset.Spoil2}";
    }
}
