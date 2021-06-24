using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TapManager : MonoBehaviour
{
    public Text StartText;
    void Start()
    {
        StartCoroutine(StartTextCon());
    }
    IEnumerator StartTextCon()
    {
        while (true)
        {
            StartText.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.3f);
            StartText.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.3f);
        }
    }
    private void Update()
    {
        if (Input.anyKeyDown)
        {
            Managers.Scene.LoadScene(Define.Scene.Main);
        }
    }
}