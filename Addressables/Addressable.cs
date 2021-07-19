using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class Addressable : MonoBehaviour
{
    public bool useRemote = false;
    public bool loadOnStart = false;

    int DownloadCount = 0;
    int LoadDoneCount = 0;

    enum Mode
    {       
        Wait,
        DownloadAsset,
        LoadAsset,
        End
    }

    private Mode UpdateMode = Mode.Wait;

    //address
    readonly string[] stringKeys = new string[]
    {

    };

    private void Update()
    {
        switch (UpdateMode)
        {
            case Mode.Wait:
                break;

            case Mode.DownloadAsset:
                {
                    for (int i = -1; ++i < stringKeys.Length;)
                    {
                        string key = stringKeys[i];

                        /// <summary>
                        /// bundle size�޾ƿ��� opSize.Result�� 0���� ũ�� �ٿ���� bundle�� �ִ� ��
                        /// + ��� LoadAssetAsync�� �Ҷ� bundle�� Remote���� �޾ƿ��� �Ǹ� ����������
                        /// memory�� asset�� �ø��� ���� �˾Ƽ� �ٿ�ε带 �����ϰ� �ε���� �ϱ� ��
                        /// (Local�̸� �翬�� �׳� ���ÿ� �ִ°��� �ٷ� Load)
                        /// �׷��� �ϳ��� ���鿡 ����ִ� Asset���� ������ ���� �����ϸ� �� ���鿡 �ִ�
                        /// Asset�ϳ��� �ٷ� Load�ع����� �� ������ ���Ե� ���� ��ü�� �ٿ�ε� �ϱ� ������ 
                        /// Remote���� �޾ƿö��� ���� �׷��� ����ϸ� �ȵ�
                        /// -> �̸� �ٿ� �ؼ� �޸𸮿� �ø��� �ε��ϴ°� ����
                        /// + ������ �� ����� �����ϰ� ���� asset���̾� �ٷ� Load�ϰų� Instantiate�ص� ��
                        /// opSize.PercentComplete - download percent
                        /// </summary>
                        Addressables.GetDownloadSizeAsync(key).Completed += (opSize) =>
                        {
                            if (opSize.Status == AsyncOperationStatus.Succeeded && opSize.Result > 0)
                                Addressables.DownloadDependenciesAsync(key, true).Completed += (opDownload) =>
                                {
                                    if (opDownload.Status != AsyncOperationStatus.Succeeded) return;
                                    
                                    OnDownloadDone();
                                };
                            else
                            {
                                // Result ���� 0�̸� �̹� �ٿ� �Ϸ� ����
                                OnDownloadDone();
                            }

                            Debug.Log("Download: " + opSize.PercentComplete * 100f + "%");
                        };
                    }
                    UpdateMode = Mode.Wait;
                }
                break;

                //�޸𸮿� �ε�
            case Mode.LoadAsset:
                {
                    for (int i = -1; i < stringKeys.Length;)
                    {
                        Addressables.LoadAssetAsync<GameObject>(stringKeys[i]).Completed += (op) =>
                        {
                            if (op.Status != AsyncOperationStatus.Succeeded) return;
                            
                            OnLoadDone(op);
                        };
                    }
                    UpdateMode = Mode.Wait;
                }
                break;

            case Mode.End:
                break;
        }
    }

    private void OnDownloadDone()
    {
        ++DownloadCount;

        if (DownloadCount == stringKeys.Length)
        {
            UpdateMode = Mode.LoadAsset;
            Debug.Log("Download complete");
        }
    }

    private void OnLoadDone(AsyncOperationHandle<GameObject> op)
    {
        ++LoadDoneCount;

        if (LoadDoneCount == stringKeys.Length)
        {
            UpdateMode = Mode.End;
            Debug.Log("Load complete");
        }
    }

    public void _Spawn(string obj_address)
    {
        Addressables.InstantiateAsync(obj_address).Completed += (op) =>
        {
            // Instantiate�ϰ��� �� �۾�
        };
    }

    public void _RelaseInstance(GameObject obj)
    {
        Addressables.ReleaseInstance(obj);
    }    

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

            if (loadOnStart)
            {
                if (useRemote) UpdateMode = Mode.DownloadAsset;
                else UpdateMode = Mode.LoadAsset;
            }
            else
            {
                UpdateMode = Mode.End;
            }

            // �Ϸ� �� �޸� ���� ������ ����Addressables handle�� �޸𸮿��� ���������
            Addressables.Release(async);
        };
    }

    /// <summary>
    /// Update�� �Ҷ� �������� bundle�� �ѹ� �����ְ� ������Ʈ �� ������ �ޱ� ���� ���� ���� �ʱ�ȭ
    /// C:\Users\user\AppData\LocalLow �ȿ� project Company name���� ������ ���� ���� ĳ��(����)�� ���� ��
    /// -> ĳ���� ������ Ǯ���� �ٿ�� ������
    /// �̰� �������� ��
    /// </summary>
    public void BundleUpdateClear()
    {
        /// <summary>
        /// ���ǰ��ִ� asset�� �޸𸮿��� ���� �������� 
        /// - �ȱ׷��� ������� Asset�� ������ �� �������� ���� �����        
        /// </summary> 
        AssetBundle.UnloadAllAssetBundles(true);

        for (int i = -1; ++i < stringKeys.Length;)
        {
            Addressables.ClearDependencyCacheAsync(stringKeys[i]);
        }

        Debug.Log("Cleared old bundles");
    }
}
