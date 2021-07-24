using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class Addressable : MonoBehaviour
{
    public List<GameObject> _pools = new List<GameObject>();

    //Test용 opHandle
    AsyncOperationHandle _downHandle;

    bool _downloadBundle;

    //addressableLabels
    readonly string[] stringKeys_Labels = new string[]
    {
        "Test_01",
    };

    private void Start()
    {
        _GetDownloadSize();
    }

    private void Update()
    {
        if (_downloadBundle)
            Debug.Log("BundlesDownloadPercent: " + _downHandle.GetDownloadStatus().Percent * 100f + "%");
    }

    /// <summary>
    /// 번들 동기 다운
    /// -> Addressable에서는 동작방식을 모두 비동기로 작업하라고 권장 but 번들 다운로드 상황을 확인하려면 동기 방식으로 해야함
    /// + 사실 비동기 방식이 썩 좋은 것 만은 아니기도 함
    /// </summary>    
    public void _GetDownloadSize()
    {
        _downloadBundle = true;

        for (int i = -1; ++i < stringKeys_Labels.Length;)
        {            
            _downHandle = Addressables.DownloadDependencies(stringKeys_Labels[i]);
            Debug.Log("BundlesDownloadPercent_for: " + _downHandle.GetDownloadStatus().Percent * 100f + "%");
        }            

        _downHandle.Completed += (op) => { OnDownloadDone(); };
    }

    /// <summary>
    /// 번들 비동기 다운
    /// bundle size받아오고 opSize.Result가 0보다 크면 다운받을 bundle이 있는 것
    /// + 사실 LoadAssetAsync을 할때 bundle을 Remote에서 받아오게 되면 내부적으로
    /// memory에 asset을 올리기 위해 알아서 다운로드를 진행하고 로드까지 하긴 함
    /// (Local이면 당연히 그냥 로컬에 있는것을 바로 Load)
    /// 그러나 하나의 번들에 들어있는 Asset들의 정보가 많다 가정하면 그 번들에 있는
    /// Asset하나를 바로 Load해버리면 그 에셋이 포함된 번들 자체를 다운로드 하기 때문에 
    /// Remote에서 받아올때는 절대 그렇게 사용하면 안됨
    /// -> 미리 다운 해서 메모리에 올리고 로드하는게 좋음
    /// + 번들을 잘 나누어서 간단하게 작은 asset들이야 바로 Load하거나 Instantiate해도 됨
    /// + 우린 Load -> Instantiate 하지말고 바로 Instantiate 할 것
    /// </summary>
    public void _GetDownloadSizeAsync()
    {
        for (int i = -1; ++i < stringKeys_Labels.Length;)
        {
            string key = stringKeys_Labels[i];
          
            Addressables.GetDownloadSizeAsync(stringKeys_Labels[i]).Completed += (opSize) =>
            {
                if (opSize.Status == AsyncOperationStatus.Succeeded && opSize.Result > 0)
                {
                    //DownloadDependenciesAsync 2번째 매개변수 true -> 사용한 Handle을 완료 후 자동으로 메모리에서 내려주기 위함
                    Addressables.DownloadDependenciesAsync(key, true).Completed += (opDownload) =>
                    {
                        if (opDownload.Status != AsyncOperationStatus.Succeeded) return;

                        OnDownloadDone();
                    };
                }
                else
                {
                    // Result 값이 0이면 이미 다운 완료 상태
                    OnDownloadDone();
                }
            };
        }

    }

    private void OnDownloadDone()
    {
        _downloadBundle = false;
        Debug.Log("Download complete");
    }

    public void _Spawn(string obj_address)
    {
        Addressables.InstantiateAsync(obj_address).Completed += (op) =>
        {
            _pools.Add(op.Result);            
        };

        //Addressables.InstantiateAsync(obj_address).Completed += (AsyncOperationHandle<GameObject> obj) =>
        //{
        //    _pools.Add(obj.Result);
        //};
    }

    public void _RelaseInstance()
    {
        for (int i = -1; ++i < _pools.Count;)
        {
            Addressables.Release<GameObject>(_pools[i]);
        }

        _pools.Clear();
    }

    /// <summary>
    /// 메모리에 로드
    /// Label이 같은 게임 오브젝트를 한번에 받아 사용할 예정이라 사실상 안쓸 코드임
    /// </summary>
    public void _LoadAssetAsync()
    {
        //for (int i = -1; ++i < stringKeys.Length;)
        //{            
        //    Addressables.LoadAssetAsync<GameObject>(key).Completed += (op) =>
        //    {
        //        if (op.Status != AsyncOperationStatus.Succeeded) return;

        //        OnLoadDone(op);
        //    };
        //}
    }

    private void OnLoadDone(AsyncOperationHandle<GameObject> op)
    {
        Debug.Log("Load complete -> " + op.Result);
    }    

    //아래 2 함수는 Test해봐야함
    /// <summary>
    /// InitializeAsync통해 addressable을 초기화 - 사실 addressable의 메소드를 사용하다 보면
    /// InitializeAsync가 안되어 있으면 자동으로 내부에서 동작하는데 그러나 몇몇 addressable의 메소드는
    /// InitializeAsync를 호출하지 않기 때문에 오류를 미연에 방지하기 위함
    /// </summary>
    public void _ClearBundles()
    {
        var async = Addressables.InitializeAsync();

        async.Completed += (op) =>
        {
            // 존재하는 cache 삭제, Unity에서 제공하는 것
            Caching.ClearCache();
            Debug.Log("ClearCache All");

            // 완료 후 메모리 누수 방지를 위해Addressables handle을 메모리에서 내려줘야함
            Addressables.Release(async);
        };
    }

    /// <summary>
    /// Update를 할때 구버전의 bundle을 한번 지워주고 업데이트 된 번들을 받기 위해 기존 번들 초기화
    /// * C:\Users\user\AppData\LocalLow 안에 project Company name으로 지정된 폴더 내에 캐쉬(번들)이 생성 됨
    /// -> 캐쉬는 번들이 풀려서 다운된 파일임
    /// 이게 지워지는 것
    /// </summary>
    public void _BundleUpdateClear()
    {
        /// <summary>
        /// 사용되고있는 asset을 메모리에서 먼저 내려야함 
        /// - 안그러면 사용중인 Asset의 번들은 안 지워지고 오류 뜰거임        
        /// </summary> 
        AssetBundle.UnloadAllAssetBundles(true);

        for (int i = -1; ++i < stringKeys_Labels.Length;)
        {
            Addressables.ClearDependencyCacheAsync(stringKeys_Labels[i]);
        }
        Debug.Log("Cleared old bundles");
    }
}
