using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject Spawn(Define.WorldObject type, string path, Transform parent = null)
    {
        GameObject go = Managers.Resource.Instantiate(path, parent);

        //switch (type)
        //{
        //    case Define.WorldObject.Tree:
        //        break;
        //    case Define.WorldObject.Flower:  
        //        break;
        //}

        return go;
    }
}
