using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class Addressable : MonoBehaviour
{
    public List<GameObject> _pools = new List<GameObject>();

    //Test�� opHandle
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
    /// ���� ���� �ٿ�
    /// -> Addressable������ ���۹���� ��� �񵿱�� �۾��϶�� ���� but ���� �ٿ�ε� ��Ȳ�� Ȯ���Ϸ��� ���� ������� �ؾ���
    /// + ��� �񵿱� ����� �� ���� �� ���� �ƴϱ⵵ ��
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
    /// ���� �񵿱� �ٿ�
    /// bundle size�޾ƿ��� opSize.Result�� 0���� ũ�� �ٿ���� bundle�� �ִ� ��
    /// + ��� LoadAssetAsync�� �Ҷ� bundle�� Remote���� �޾ƿ��� �Ǹ� ����������
    /// memory�� asset�� �ø��� ���� �˾Ƽ� �ٿ�ε带 �����ϰ� �ε���� �ϱ� ��
    /// (Local�̸� �翬�� �׳� ���ÿ� �ִ°��� �ٷ� Load)
    /// �׷��� �ϳ��� ���鿡 ����ִ� Asset���� ������ ���� �����ϸ� �� ���鿡 �ִ�
    /// Asset�ϳ��� �ٷ� Load�ع����� �� ������ ���Ե� ���� ��ü�� �ٿ�ε� �ϱ� ������ 
    /// Remote���� �޾ƿö��� ���� �׷��� ����ϸ� �ȵ�
    /// -> �̸� �ٿ� �ؼ� �޸𸮿� �ø��� �ε��ϴ°� ����
    /// + ������ �� ����� �����ϰ� ���� asset���̾� �ٷ� Load�ϰų� Instantiate�ص� ��
    /// + �츰 Load -> Instantiate �������� �ٷ� Instantiate �� ��
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
                    //DownloadDependenciesAsync 2��° �Ű����� true -> ����� Handle�� �Ϸ� �� �ڵ����� �޸𸮿��� �����ֱ� ����
                    Addressables.DownloadDependenciesAsync(key, true).Completed += (opDownload) =>
                    {
                        if (opDownload.Status != AsyncOperationStatus.Succeeded) return;

                        OnDownloadDone();
                    };
                }
                else
                {
                    // Result ���� 0�̸� �̹� �ٿ� �Ϸ� ����
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
    /// �޸𸮿� �ε�
    /// Label�� ���� ���� ������Ʈ�� �ѹ��� �޾� ����� �����̶� ��ǻ� �Ⱦ� �ڵ���
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

    //�Ʒ� 2 �Լ��� Test�غ�����
    /// <summary>
    /// InitializeAsync���� addressable�� �ʱ�ȭ - ��� addressable�� �޼ҵ带 ����ϴ� ����
    /// InitializeAsync�� �ȵǾ� ������ �ڵ����� ���ο��� �����ϴµ� �׷��� ��� addressable�� �޼ҵ��
    /// InitializeAsync�� ȣ������ �ʱ� ������ ������ �̿��� �����ϱ� ����
    /// </summary>
    public void _ClearBundles()
    {
        var async = Addressables.InitializeAsync();

        async.Completed += (op) =>
        {
            // �����ϴ� cache ����, Unity���� �����ϴ� ��
            Caching.ClearCache();
            Debug.Log("ClearCache All");

            // �Ϸ� �� �޸� ���� ������ ����Addressables handle�� �޸𸮿��� ���������
            Addressables.Release(async);
        };
    }

    /// <summary>
    /// Update�� �Ҷ� �������� bundle�� �ѹ� �����ְ� ������Ʈ �� ������ �ޱ� ���� ���� ���� �ʱ�ȭ
    /// * C:\Users\user\AppData\LocalLow �ȿ� project Company name���� ������ ���� ���� ĳ��(����)�� ���� ��
    /// -> ĳ���� ������ Ǯ���� �ٿ�� ������
    /// �̰� �������� ��
    /// </summary>
    public void _BundleUpdateClear()
    {
        /// <summary>
        /// ���ǰ��ִ� asset�� �޸𸮿��� ���� �������� 
        /// - �ȱ׷��� ������� Asset�� ������ �� �������� ���� �����        
        /// </summary> 
        AssetBundle.UnloadAllAssetBundles(true);

        for (int i = -1; ++i < stringKeys_Labels.Length;)
        {
            Addressables.ClearDependencyCacheAsync(stringKeys_Labels[i]);
        }
        Debug.Log("Cleared old bundles");
    }
}
