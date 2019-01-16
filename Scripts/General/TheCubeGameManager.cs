using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TheCubeGameManager : MonoBehaviour
{
	private static TheCubeGameManager _S;
	private int _currentLevel;
	private int _finalLevel;
	AsyncOperation asyncManager;
	const int WAIT_TIME = 8;
	bool _startOverOneShot;
	//if this wasn't singleplayer I'd only put the player in the 
	//CubeController and not make it static. 
	//might use function instead later
	public static GameObject player;
	public static GameObject playerMesh;
	public static Vector3 startScale;

	private void Awake()
	{
		
		if (_S == null)
		{
			_S = this;
		}
		
		_finalLevel = SceneManager.sceneCountInBuildSettings - 1;
		print(_finalLevel);
	}

	private void OnEnable()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	private void OnDisable()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		print("sceneLoaded");
		player = GameObject.FindGameObjectWithTag("Player");
		if (player != null)
		{
			playerMesh = player.transform.Find("playerMesh").gameObject;

			startScale = playerMesh.transform.localScale;
		}
		_currentLevel = SceneManager.GetActiveScene().buildIndex;
		if (_currentLevel == _finalLevel)
		{
			if (!_startOverOneShot)
			{
				StartCoroutine("StartOver");
				_startOverOneShot = true;
			}
		}
		else
		{
			_startOverOneShot = false;
		}
	}

	IEnumerator StartOver()
	{
		yield return new WaitForSeconds(5);
		LoadLevel(0);
	}

	public static void LoadNextLevel()
	{
		if (LoadComplete())
		{
			_S.asyncManager = SceneManager.LoadSceneAsync(_S._currentLevel + 1);
			CheckpointManager.RemoveAllCheckpoints();
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
		_S._currentLevel = SceneManager.GetActiveScene().buildIndex;
		if (_S.asyncManager != null)
			return _S.asyncManager.isDone;
		return true;
	}
}
