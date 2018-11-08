using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TheCubeGameManager : MonoBehaviour
{
	private static TheCubeGameManager _S;
	public int currentLevel;
	AsyncOperation asyncManager;
	static bool created;

	private void Awake()
	{
		if (_S == null)
		{
			_S = this;
		}
		if (!created)
		{
			DontDestroyOnLoad(gameObject);
			created = true;
		}
		if (this != _S)
		{
			Destroy(this);
		}
		currentLevel = SceneManager.GetActiveScene().buildIndex;
	}
	public static void LoadNextLevel()
	{
		if (LoadComplete())
		{
			_S.asyncManager = SceneManager.LoadSceneAsync(_S.currentLevel + 1);
		}
	}
	public static void LoadLevel(int level)
	{
		if (LoadComplete())
		{
			_S.asyncManager = SceneManager.LoadSceneAsync(level);
		}
	}
	public static bool LoadComplete()
	{
		if(_S.asyncManager != null)
			return _S.asyncManager.isDone;
		return true;
	}
}
