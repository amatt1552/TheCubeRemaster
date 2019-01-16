using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(CubeController))]
public class CubeMotor : MonoBehaviour
{
	//state of object. not sure if I want to use this yet.
	enum CurrentState
	{
		alive,
		popped,
		fell,
		burned,
		squished,
		pushed
	}
	CurrentState currentState;

	//launcher
	bool _inLauncher;
	Launcher _currentLauncher;

	//bouncer
	bool _inBouncer;
	bool _groundJumpActive = true;
	Bouncer _currentBouncer;
	
	//speedboost
	bool _inBooster;
	
	//movingObject
	bool _onMovingObject;
	Rigidbody _movingRb;

	//camera
	bool _inCameraEdit;
	public CameraFollow cameraFollow;
	CameraEditSettings _currentCameraEdit;

	bool _movementEnabled = true;

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

	//jump variables end.

	//helps prevent missing the ground check.
	bool _ground;
	bool _groundOneShot;

	//for falling
	bool fallImminent;
	bool falling;

	//particle effects
	ParticleSystem _movingEffect;
	ParticleSystem _poppedEffect;

	private void Awake()
	{
		if (_cubeInfoSO == null)
		{
			Debug.LogError("Need to add a CubeInfoScriptableObject to this controller!");
			enabled = false;
		}

		_controller = GetComponent<CubeController>();
		rb = _controller.GetRigidbody();
		targetHeight = _cubeInfoSO.jumpHeight;

		//particle effects on player

		GameObject tempObj;
		//consistant names needed for sanity. fix all of them later
		tempObj = Instantiate(_cubeInfoSO.slidingEffects[0]);
		tempObj.transform.parent = transform;
		tempObj.transform.localPosition = Vector3.down * 0.4f; 
		_movingEffect = tempObj.GetComponent<ParticleSystem>();

		tempObj = Instantiate(_cubeInfoSO.poppedEffects[0]);
		tempObj.transform.parent = transform;
		tempObj.transform.localPosition = Vector3.zero;
		tempObj.transform.rotation = transform.rotation;
		_poppedEffect = tempObj.GetComponent<ParticleSystem>();

		_controller.onRespawn.AddListener(OnRespawn);
	}

	private void Start()
	{
		if (cameraFollow == null && Camera.main != null)
		{
			cameraFollow = Camera.main.gameObject.GetComponent<CameraFollow>();
		}
		cameraFollow.FollowedObject = gameObject;
		cameraFollow.SnapToObject(); 
	}
	
	private void Update ()
	{
		_moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, 0);
		
		//ground stuff
		if (_controller.Ground())
		{
			_ground = true;
			_extraJumps = 0;
			_jumpDownReset = true;
			_distanceReached = false;
			//only should execute once right when landed.
			if (!_groundOneShot)
			{
				print("aHeroHasFallen.");
				_groundOneShot = true;
			}
			print(_controller.moving);
			if (_controller.moving && !_movingEffect.isPlaying)
			{
				_movingEffect.Play();
			}
			else if (!_controller.moving)
			{
				_movingEffect.Stop();
			}
		}
		else
		{
			_ground = false;
			_groundOneShot = false;
			_movingEffect.Stop();
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
			_distanceReached = true;
			_realJumpForce = 0;
			_holdingJump = false;
			_jumpDownReset = true;
		}

		SetJump();

		//falling stuff
		//fallImminent and falling makes sure it doesn't play more than once

		if (_ground && _controller.OnEdge() && !fallImminent && !falling && !_controller.moving)
		{
			float fallTime;
			fallTime = 0.2f;
			StartCoroutine("FallImminent", fallTime);
		}
		if(!_controller.OnEdge() && !falling && fallImminent)
		{
			StopCoroutine("FallImminent");
			fallImminent = false;
		}
		
	}
	
	private void FixedUpdate()
	{
		
		Launching();
		Bouncing();
		Boosting();
		OnMovingObject();
		

		if (!_controller.dead)
		{
			currentState = CurrentState.alive;
			if (_movementEnabled)
			{

				_controller.Move(_cubeInfoSO.speed + _extraSpeed + _movingObjectDirection.x, _moveDirection);

				if (!_holdingJump)
				{
					_controller.Step(_moveDirection, 1f);
				}
				if (_groundJumpActive && !_inLauncher && !_inBouncer)
				{
					_controller.Jump(_realJumpForce, _holdingJump, ref _distanceReached);

				}
			}
		}
	}


	private void OnTriggerEnter(Collider other)
	{
		//death stuffs
		if (other.tag == "Enemy")
		{
			_poppedEffect.Play();
			_controller.Dead(_cubeInfoSO.deathTime);
			print("dead");
		}
		if (other.tag == "TheVoid")
		{
			_controller.Dead(_cubeInfoSO.deathTime, false, false);
			print("dead");
		}
		if (other.tag == "Blaster")
		{
			_controller.Dead(_cubeInfoSO.deathTime);
			Instantiate(_cubeInfoSO.burnedEffectsA[0],transform.position, transform.rotation);
			print("dead");
		}

		if (other.tag == "Pusher")
		{
			_controller.Dead(_cubeInfoSO.deathTime, false, false);
			print("dead");
		}

		if (other.tag == "Hammer")
		{
			_controller.Dead(_cubeInfoSO.deathTime, false, false);
			print("dead");
		}

		if (other.tag == "Smasher")
		{
			_controller.Dead(_cubeInfoSO.deathTime, false, false, false);
			if (!_ground)
				_controller.GetRigidbody().AddForce(Vector3.down * 10, ForceMode.Impulse);

			TheCubeGameManager.playerMesh.transform.localPosition = Vector3.down * 0.5f;
			TheCubeGameManager.playerMesh.transform.localScale = new Vector3(TheCubeGameManager.playerMesh.transform.localScale.x, TheCubeGameManager.playerMesh.transform.localScale.x / 2, TheCubeGameManager.playerMesh.transform.localScale.z / 10);
			print("dead");
		}

		//other
		if (other.tag == "Launcher")
		{
			print("inLaucher!");
			Launcher launch = other.GetComponent<Launcher>();
			if (launch != null)
			{
				_currentLauncher = launch;
			}
			
			_inLauncher = true;
		}

		if (other.tag == "Bouncer")
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

		if (other.tag == "Booster")
		{
			print("SPEEDBOOST!");
			_inBooster = true;
		}

		if (other.tag == "Moving")
		{
			print("inMovingObject");
			_onMovingObject = true;
			Rigidbody movingRb = other.GetComponent<Rigidbody>(); 
			if (movingRb != null)
			{
				_movingRb = movingRb;
			}
		}
		if (other.tag == "CameraEdit")
		{
			_currentCameraEdit = other.GetComponent<CameraEditSettings>();
			if(_currentCameraEdit != null)
			{
				print("moving camera..");
				cameraFollow.SetOffset(_currentCameraEdit.newCameraOffset);
			}
		}

	}

	//maybe have these based on ground instead.

	private void OnCollisionEnter(Collision collision)
	{
		if (falling && !_controller.dead)
		{
			StartCoroutine("Recover", 3);
		}
		
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

	private void OnCollisionStay(Collision collision)
	{
		if (!falling && !_controller.dead)
		{
			_controller.MoveToZ();
		}
	}

	private void OnCollisionExit(Collision collision)
	{
		if (falling)
		{
			StopCoroutine("Recover");
		}
	}
	private void OnTriggerExit(Collider other)
	{
		if (other.tag == "Launcher")
		{
			print("leftLaucher!");
			_inLauncher = false;
		}
		if (other.tag == "Bouncer")
		{
			print("leftBouncer!");
			_inBouncer = false;
		}
		if (other.tag == "Booster")
		{
			print("enoughBoosting!");
			_inBooster = false;
		}
		if (other.tag == "Moving")
		{
			print("leftMovingObject!");
			_onMovingObject = false;
		}
		if (other.tag == "CameraEdit")
		{
			
			print("resetting Camera..");
			cameraFollow.SetToDefault();
			
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
				print("extra hop");
				_realJumpForce = 1f;
				_extraJumps++;
				_positionBeforeJump = transform.position;
				_distanceReached = false;
				_jumpOneShot = true;
			}

			//gets the jump to increase to top speed in roughly a second/jumpForceIncreaseMultiplier.
			if (_realJumpForce < _cubeInfoSO.jumpForce)
			{

				_realJumpForce += _cubeInfoSO.jumpForce * _jumpForceIncreaseMultiplier * Time.deltaTime;
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
				_controller.positionZ = _currentLauncher.GetDistanceZ();
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
		if (_movingRb != null && _onMovingObject && _ground)
		{
			_movingObjectDirection = _movingRb.velocity;
		}
		else
		{
			_movingObjectDirection = Vector3.zero;
		}
	}

	public void SetMovementActive(bool enabled)
	{
		_movementEnabled = enabled;
	}

	IEnumerator FallImminent(float waitTime)
	{
		fallImminent = true;
		yield return new WaitForSeconds(waitTime);
		print("I fell.");
		_movementEnabled = false;
		_controller.RemoveConstraints();
		fallImminent = false;
		falling = true;
	}

	//eventChecking

	void OnRespawn()
	{
		falling = false;
		_movementEnabled = true;
		TheCubeGameManager.playerMesh.transform.localPosition = Vector3.zero;
		TheCubeGameManager.playerMesh.transform.localScale = TheCubeGameManager.startScale;
		cameraFollow.SetToDefault();
		cameraFollow.SnapToObject();
	}

	IEnumerator Recover(float recoverTime)
	{
		yield return new WaitForSeconds(recoverTime);
		_controller.Recover();
		falling = false;
		_movementEnabled = true;
	}

}
