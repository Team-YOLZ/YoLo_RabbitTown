using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


public class Popup : MonoBehaviour
{
    public Vector2 startPosition = Vector2.zero;
    private RectTransform rectTransform;
    public RectTransform GetRectTransform
    {
        get
        {
            if (rectTransform == null)
                rectTransform = GetComponent<RectTransform>();

            return rectTransform;
        }
    }
    private Animator ani;
    private Animator GetAni
    {
        get
        {
            if (ani == null)
                ani = GetComponent<Animator>();

            return ani;
        }
    }

    public bool IsVisible { get { return gameObject.activeSelf; } }


    virtual protected void Awake()
    {
    }

    public virtual void Show()
    {
        transform.SetAsLastSibling();
        gameObject.SetActive(true);
        GetRectTransform.anchoredPosition = startPosition;
        GetAni.Play("in");
    }

    public virtual void Hide()
    {
        if (gameObject.activeSelf)
            GetAni.Play("out");
    }

    public virtual void HideTimeScale()
    {
        if (gameObject.activeSelf)
            GetAni.Play("out");

        Time.timeScale = 1;
    }

    public virtual void InActive()
    {
        GetAni.Rebind();
        gameObject.SetActive(false);
    }

    public void InitInActive()
    {
        GetAni.Rebind();
        transform.localPosition = startPosition;
        transform.localScale = Vector3.one;
        gameObject.SetActive(false);
    }
}

