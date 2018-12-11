using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(CubeController))]
public class CubeMotor : MonoBehaviour
{
	bool _inLauncher;
	Launcher _currentLauncher;

	bool _inBouncer;
	bool _groundJumpActive = true;
	Bouncer _currentBouncer;

	bool _inBooster;

	bool _onMovingObject;
	Rigidbody _movingRb;

	bool _movementEnabled = true;

	
	public enum eDeathTypes
	{
		hit
	}

	[Header("Set before starting")]
	public CubeInfoScriptableObject _cubeInfoSO;
	CubeController _controller;
	Rigidbody rb;
	Vector3 _moveDirection;
	Vector3 _movingObjectDirection;
	float _extraSpeed;

	//all the jump variables.. will try to cut back somehow probably.

	int _extraJumps;
	float _realJumpForce;
	float targetHeight;
	//makes the jumpForce increase to the cubeinfo's faster.
	const float _jumpForceIncreaseMultiplier = 4;

	//will run like a getKey
	bool _holdingJump;
	//will run like a getKeyDown
	bool _jumpDown;
	//helps reset the the _jumpdown
	bool _jumpDownReset = true;
	bool _jumpOneShot;
	//sets the moment you jump
	Vector3 _positionBeforeJump;
	bool _distanceReached;

	//helps prevent missing the ground check.
	bool _ground;
	bool _groundOneShot;

	private void Start()
	{
		
		if (_cubeInfoSO == null)
		{
			Debug.LogError("Need to add a CubeInfoScriptableObject to this controller!");
			enabled = false;
		}
		_controller = GetComponent<CubeController>();
		rb = _controller.GetRigidbody();
		targetHeight = _cubeInfoSO.jumpHeight;
	}
	
	private void Update ()
	{
		
		_moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, 0);
		if (_controller.Ground())
		{
			_extraJumps = 0;
			_jumpDownReset = true;
			_distanceReached = false;
			//only should execute once right when landed.
			if (!_groundOneShot)
			{
				print("aHeroHasFallen.");
				_groundOneShot = true;
			}
		}
		else
		{
			_groundOneShot = false;
		}
		//jump input

		if (Input.GetAxisRaw("Jump") != 0)
		{
			_holdingJump = true;
			_jumpDown = false;
			if (_jumpDownReset)
			{
				_jumpDown = true;
			}
			_jumpDownReset = false;
		}
		else
		{
			_holdingJump = false;
			_jumpDownReset = true;
		}

		SetJump();

	}
	
	private void FixedUpdate()
	{
		
		Launching();
		Bouncing();
		Boosting();
		OnMovingObject();

		if (!_controller.dead && _movementEnabled)
		{
			_controller.Move(_cubeInfoSO.speed + _extraSpeed, _moveDirection); //+ _movingObjectDirection * Time.deltaTime));
			if (_groundJumpActive && !_inLauncher) 
				_controller.Jump(_realJumpForce, _holdingJump, _distanceReached);
		}

	}

	private void OnTriggerEnter(Collider other)
	{
		//death stuffs
		if (other.tag == "enemy")
		{
			_controller.Dead(_cubeInfoSO.deathTime);
			print("dead");
		}
		if (other.tag == "theVoid")
		{
			_controller.Dead(_cubeInfoSO.deathTime);
			print("dead");
		}

		if (other.tag == "launcher")
		{
			print("inLaucher!");
			Launcher launch = other.GetComponent<Launcher>();
			if (launch != null)
			{
				_currentLauncher = launch;
			}
			
			_inLauncher = true;
		}

		if (other.tag == "bouncer")
		{
			print("inBouncer!");
			Bouncer bouncer = other.GetComponent<Bouncer>();
			if (bouncer != null)
			{
				_currentBouncer = bouncer;
			}
			_groundJumpActive = false;
			_inBouncer = true;
		}

		if (other.tag == "booster")
		{
			print("SPEEDBOOST!");
			_inBooster = true;
		}

		if (other.tag == "moving")
		{
			print("inMovingObject");
			_onMovingObject = true;
			Rigidbody movingRb = other.GetComponent<Rigidbody>(); 
			if (movingRb != null)
			{
				_movingRb = movingRb;
			}
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (!_inLauncher && _currentLauncher != null)
		{
			_movementEnabled = true;
			_currentLauncher = null;
		}

		if (!_inBouncer && _currentBouncer != null)
		{
			_currentBouncer = null;
			_groundJumpActive = true;
		}

		if (!_onMovingObject && _movingRb != null)
		{
			_movingRb = null;
			_movingObjectDirection = Vector3.zero;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.tag == "launcher")
		{
			print("leftLaucher!");
			_inLauncher = false;
		}
		if (other.tag == "bouncer")
		{
			print("leftBouncer!");
			_inBouncer = false;
		}
		if (other.tag == "booster")
		{
			print("enoughBoosting!");
			_inBooster = false;
		}
		if (other.tag == "moving")
		{
			print("leftMovingObject!");
			_onMovingObject = false;
		}
	}

	void SetJump()
	{
		
			if (!_jumpDown)
			{
				_jumpOneShot = false;
			}

			if (_jumpDown && _controller.Ground() && !_jumpOneShot)
			{
				print("hop");
				_realJumpForce = 1f;
				_positionBeforeJump = transform.position;
				_distanceReached = false;
				_jumpOneShot = true;
			}
			//extra jumps
			else if (_jumpDown && !_controller.Ground() && _extraJumps < _cubeInfoSO.maxExtraJumps)
			{
				_realJumpForce = 1f;
				_extraJumps++;
				_positionBeforeJump = transform.position;
				_distanceReached = false;
				_jumpOneShot = true;
			}

			//gets the jump to increase to top speed in roughly a second/jumpForceIncreaseMultiplier.
			if (_realJumpForce < _cubeInfoSO.jumpForce)
			{

				_realJumpForce += _cubeInfoSO.jumpForce * _jumpForceIncreaseMultiplier * Time.fixedDeltaTime;
			}
			else
			{
				_realJumpForce = _cubeInfoSO.jumpForce;
			}

			//stops jump when the the distance between the last point before jumping and player is greater than the set jumpHieght

			if (transform.position.y - _positionBeforeJump.y > targetHeight)
			{
				_distanceReached = true;
			}
		
	}

	void Launching()
	{
		if (_currentLauncher != null)
		{
			if (_inLauncher && _jumpDown && _controller.Ground())
			{
				_movementEnabled = false;
				_currentLauncher.Launch(gameObject);
			}
			
		}
	}

	void Bouncing()
	{
		if (_currentBouncer != null)
		{
			if (_inBouncer && _jumpDown && _controller.Ground())
			{
				_currentBouncer.Bounce(_controller.GetRigidbody());
			}

		}
	}

	void Boosting()
	{
		if (_inBooster)
		{
			_extraSpeed = _cubeInfoSO.speedBoostSpeed - _cubeInfoSO.speed;
		}
		else
		{
			_extraSpeed = 0;
		}
	}

	void OnMovingObject()
	{
		if (_movingRb != null && _onMovingObject)
		{
			_movingObjectDirection = _movingRb.velocity;
			print(_movingRb.velocity);
		}
	}

	public void SetMovementActive(bool enabled)
	{
		_movementEnabled = enabled;
	}
}
