using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	[SerializeField] private bool dontDestroyOnLoad;
	public static T Instance
	{
		get
		{
			if (sInstance == null)
			{
				var obj = new GameObject("@ " + typeof(T).ToString());
				sInstance = obj.AddComponent<T>();
			}
			return sInstance;
		}
	}
	protected static T sInstance;
	protected virtual void Awake()
	{
		if (sInstance == null)
		{
			if (dontDestroyOnLoad)
				DontDestroyOnLoad(this.gameObject);
			sInstance = this.GetComponent<T>();
			Init();
		}
		else if (sInstance != this)
		{
			Debug.Log("Destroy " + gameObject.name);
			Destroy(this.gameObject);
		}
	}
	protected abstract void Init();

	protected virtual void OnDestroy()
	{
		sInstance = null;
	}
}