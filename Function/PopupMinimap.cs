using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupMinimap : Popup
{
    public Image image_minimap;
    [SerializeField] private RectTransform image_Player;

    public GameObject _player;
    
    private void Start()
    {
        _player = GameObject.Find("player");
    }

    private void Update()
    {
        image_Player.anchoredPosition = new Vector2(_player.transform.position.x+450f, _player.transform.position.z-300f);
    }
}
