using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pusher : MonoBehaviour
{

	enum PushState
	{
		eRetract, ePushWarning, ePush
	}

	//its currentState.
	PushState pushState;

	//makes sure they run one at a time.
	bool _coroutineRunning;

	//pushing out
	public float pushSpeed = 10;
	public float pushTime = 4;
	public float warningTime = 1;

	//pulling back in
	public float retractSpeed = 2;
	public float retractTime = 2;

	//transforms and such for pusher
	public Transform pusher, pushPoint, retractPoint;
	Rigidbody _pusherRb;
	Vector3 targetPoint;
	public MeshRenderer pusherMat;

	//sets the color to be pusher material's color at start.
	Color defaultColor;
	//the color it changes to right before pushing
	public Color pushWarningColor = Color.yellow;

	//how close it gets to a point
	const float CLOSE_TO_POINT = 0.1f;
	

	private void Start()
	{
		VariableChecks();
		if (pusherMat != null)
		{
			defaultColor = pusherMat.material.color;
		}
		if (retractPoint == null)
		{
			retractPoint = transform;
		}
		if (pushPoint == null)
		{
			pushPoint = transform;
		}
		if (pusher == null)
		{
			pusher = transform.Find("pusher");
		}
		//if I can't find the pusher I just dont let this pusher run.
		if (pusher != null)
		{
			_pusherRb = pusher.GetComponent<Rigidbody>();
			if (_pusherRb == null)
			{
				_pusherRb = pusher.gameObject.AddComponent<Rigidbody>();
			}

			StartCoroutine("StartPush");
		}
		else
		{
			Debug.LogError("could not set pusher.. Set in inspector before start or name the child pusher.");
		}
		
	}

	
	//my update
	IEnumerator StartPush()
	{
		while (1 == 1)
		{
			VariableChecks();
			if (!_coroutineRunning)
			{
				switch (pushState)
				{
					case PushState.ePushWarning:
						StartCoroutine("PushWarning");
						break;
					case PushState.eRetract:
						StartCoroutine("Retract");
						break;
					case PushState.ePush:
						StartCoroutine("Push");
						break;
					default:
						break;
				}
			}
			yield return null;
		}

	}
	

	//pushing out method
	IEnumerator Push()
	{
		_coroutineRunning = true;
		//go to push point
		while (Vector3.Distance(pusher.position, pushPoint.position) > CLOSE_TO_POINT)
		{
			_pusherRb.MovePosition(Vector3.MoveTowards(pusher.position, pushPoint.position, pushSpeed * Time.deltaTime));
			yield return null;

		}
		//wait to retract back
		yield return new WaitForSeconds(retractTime);
		pushState = PushState.eRetract;
		_coroutineRunning = false;
	}

	//pulling in method
	IEnumerator Retract()
	{
		_coroutineRunning = true;

		//go to retract point
		while (Vector3.Distance(pusher.position, retractPoint.position) > CLOSE_TO_POINT)
		{
			_pusherRb.MovePosition(Vector3.MoveTowards(pusher.position, retractPoint.position, retractSpeed * Time.deltaTime));
			yield return null;

		}
		
		//wait until its time to warn about the incoming push
		yield return new WaitForSeconds(pushTime - warningTime);
		pushState = PushState.ePushWarning;
		_coroutineRunning = false;
	}

	//warning before push method
	IEnumerator PushWarning()
	{
		_coroutineRunning = true;
		if (pusherMat != null)
		{
			//changes material so player can tell its about to push
			pusherMat.material.color = pushWarningColor;
		}

		//wait to push
		yield return new WaitForSeconds(pushTime - (pushTime - warningTime));
		pusherMat.material.color = defaultColor;
		pushState = PushState.ePush;
		_coroutineRunning = false;
	}

	//makes sure certain values aren't wrong
	void VariableChecks()
	{
		if (pushTime < warningTime)
		{
			pushTime = warningTime;
		}
		if (pushSpeed < 0)
		{
			pushSpeed = 0;
		}
		if(pushTime < 0)
		{
			pushTime = 0;
		}
		if (warningTime < 0)
		{
			warningTime = 0;
		}
		if (retractSpeed < 0)
		{
			retractSpeed = 0;
		}
		if (retractTime < 0)
		{
			retractTime = 0;
		}
}

}
