using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(CubeController))]
public class CubeMotor : MonoBehaviour
{
	bool _movementEnabled;
	public enum eDeathTypes
	{
		hit
	}

	[Header("Set before starting")]
	public CubeInfoScriptableObject _cubeInfoSO;
	CubeController _controller;
	Vector3 moveDirection;

	//all the jump variables.. will try to cut back somehow probably.

	int _extraJumps;
	float _jumpTime;
	float _realJumpForce;
	
	//makes the jumpForce increase to the cubeinfo's faster.
	const float _jumpForceIncreaseMultiplier = 4;

	//will run like a getKey
	bool _jumping;
	//will run like a getKeyDown
	bool _jumpDown;
	bool _jumpDownReset = true;


	private void Start()
	{
		if (_cubeInfoSO == null)
		{
			Debug.LogError("Need to add a CubeInfoScriptableObject to this controller!");
			enabled = false;
		}

		_controller = GetComponent<CubeController>();
	}
	
	private void Update ()
	{
		
		moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, 0);
		if (_controller.Ground())
		{
			_extraJumps = 0;
			_jumpDownReset = true;
		}

		//jump 

		if (Input.GetAxisRaw("Jump") != 0)
		{
			_jumping = true;
			_jumpDown = false;
			if (_jumpDownReset)
			{
				_jumpDown = true;
			}
			_jumpDownReset = false;
		}
		else
		{
			_jumping = false;
			_jumpDownReset = true;
		}

		SetJump();

	}

	private void FixedUpdate()
	{
		if (!_controller.dead && _movementEnabled)
		{
			_controller.Move(_cubeInfoSO.speed, moveDirection);
			_controller.Jump(_realJumpForce, _jumpTime, _cubeInfoSO.jumpTime);
		}

	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "enemy")
		{
			_controller.Dead(_cubeInfoSO.deathTime);
			print("dead");
		}
		if (other.gameObject.tag == "theVoid")
		{
			_controller.Dead(_cubeInfoSO.deathTime);
			print("dead");
		}
	}

	void SetJump()
	{
		//starting it up
		if (_jumpDown && _controller.Ground())
		{
			_jumpTime = _cubeInfoSO.jumpTime;
			//in case jumpForce = 1
			_realJumpForce = _cubeInfoSO.jumpForce - (_cubeInfoSO.jumpForce - 0.5f);
		}
		//extra jumps.
		else if (_jumpDown && !_controller.Ground() && _extraJumps < _cubeInfoSO.maxExtraJumps)
		{
			_jumpTime = _cubeInfoSO.jumpTime;
			_realJumpForce = _cubeInfoSO.jumpForce - (_cubeInfoSO.jumpForce - 1);
			_extraJumps++;
			print(_extraJumps);
		}


		//gets the jump to increase to top speed in roughly a second/jumpForceIncreaseMultiplier.
		if (_realJumpForce < _cubeInfoSO.jumpForce)
		{
			_realJumpForce += _cubeInfoSO.jumpForce * _jumpForceIncreaseMultiplier * Time.deltaTime;
		}
		if (_jumping)
		{
			_jumpTime -= 1 * Time.deltaTime;
		}
		else
		{
			_jumpTime = 0;
		}
		
	}

	public void SetMovement(bool enabled)
	{
		_movementEnabled = enabled;
	}
}
