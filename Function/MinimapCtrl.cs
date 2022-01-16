using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapCtrl : MonoBehaviour
{
    [SerializeField] Button button_Viewminimap;

    public void Click_minimap()
    {
        Debug.Log("Click");
        GameSceneUIManager.Instance.popup_minimap.Show();
    }
}
