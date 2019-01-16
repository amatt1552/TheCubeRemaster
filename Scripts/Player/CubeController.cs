
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollisionHelp))]
public class CubeController : MonoBehaviour
{
	GameObject _player;
	Rigidbody _rigidbody;
	public Transform _playerMesh;
	MeshRenderer _playerMeshRenderer;

	//collision help
	BoxCollisionHelp _bCH;
	const float _COLLISION_BIAS = 0.98f;

	//player positions
	public Transform respawnPoint;
	public float positionZ;
	public float step = 0.1f;

	public bool moving;

	//layerMasks
	public LayerMask moveLayerMask;
	public LayerMask groundLayerMask;
	
	//death
	bool _deadReset;
	public bool _inCollider;
	public bool dead;
	Collider _savedCollider;

	public UnityEvent onRespawn;

	private void Awake()
	{
		SetPlayerGameObjectAndRigidbody();
		_bCH = GetComponent<BoxCollisionHelp>();
		if (_playerMesh == null)
		{
			_playerMesh = transform.Find("playerMesh");
		}
		if (_playerMesh != null)
		{
			_playerMeshRenderer = _playerMesh.GetComponent<MeshRenderer>();
		}
		if (onRespawn == null)
		{
			onRespawn = new UnityEvent();
		}
	}
	
	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag == "IgnoreOnDeath" && dead)
		{
			_savedCollider = collision.collider;
			Physics.IgnoreCollision(_bCH.boxCollider, collision.collider);
			
		}
	}

	private void OnCollisionStay(Collision collision)
	{
		_inCollider = true;
		if (!dead && _savedCollider != null)
		{
			Physics.IgnoreCollision(_bCH.boxCollider, _savedCollider, false);
		}
	}

	private void OnCollisionExit(Collision collision)
	{
		_inCollider = false;
	}

	/// <summary>
	/// sets the player and rigidbody. gets attached gameObject if empty
	/// </summary>
	/// <param name="player"></param>
	public void SetPlayerGameObjectAndRigidbody(GameObject player = null)
	{
		_player = player ?? gameObject;
		_rigidbody = _player.GetComponent<Rigidbody>();
	}
	
	/// <summary>
	/// basic movement based on the mass that excludes changing the y.
	/// </summary>
	/// <param name="speed"></param>
	/// <param name="direction"></param>
	public void Move(float speed, Vector3 direction)
	{
		if (!_bCH.CollidingFromDirection(direction, moveLayerMask, _COLLISION_BIAS))
		{
			//I dont want this to change the Y at all in this case so setting to be the current velocity.
			moving = direction.magnitude <= 0.1f ? false : true;
			Vector3 newDirection = speed * direction;

			newDirection.y = _rigidbody.velocity.y;

			_rigidbody.velocity = newDirection / _rigidbody.mass;
		}
		else
		{
			moving = false;
		}
		
	}

	
	public void MoveToPoint(Vector3 position)
	{
		_rigidbody.position = position;
	}


	public bool Ground()
	{
		float offset = 0.1f;
		/*was trying increasing its reach
		if (_rigidbody.velocity.y < -10)
		{
			offset = 0.1f;
		}
		*/
		if (_bCH.CollidingFromDirection(-transform.up, groundLayerMask, 0.95f, offset))
		{
			/*if (_rigidbody.velocity.y < -3)
			{
				//trying to stop it a little bit before contact
				_rigidbody.velocity = new Vector3(_rigidbody.velocity.x, 0, _rigidbody.velocity.z);
			}
			*/
			return _inCollider;
		}
		return false;
	}


	/// <summary>
	/// continues moving up if jumpTime is greater than zero.
	/// </summary>
	/// <param name="speed"></param>
	/// <param name="jumping"></param>
	public void Jump(float speed, bool jumping, ref bool distanceReached)
	{
		
		float offset = 0.1f;
		if (_bCH.CollidingFromDirection(transform.up, groundLayerMask, 0.95f, offset))
		{
			distanceReached = true;
		}
		if (jumping && !distanceReached)
		{
			_rigidbody.velocity = new Vector3(_rigidbody.velocity.x, speed, _rigidbody.velocity.z);
		}
	}
	
	/// <summary>
	/// continues moving up if jumpTime is greater than zero.
	/// </summary>
	/// <param name="speed"></param>
	/// <param name="jumping"></param>
	public void Jump(float speed, bool jumping)
	{
		
		if (jumping)
		{
			_rigidbody.velocity = new Vector3(_rigidbody.velocity.x, speed, _rigidbody.velocity.z);
		}
	}


	public void Dead(float deathTime, bool hideMesh = true, bool turnOffPhysics = true, bool removeConstraints = true)
	{
		if (!_deadReset)
		{
			moving = false;
			if (!dead)
			{
				_deadReset = true;
			}
			dead = true;
			if (hideMesh)
			{
				_playerMeshRenderer.enabled = false;
			}
			if (turnOffPhysics)
			{
				_bCH.boxCollider.enabled = false;
				_rigidbody.isKinematic = true;
			}
			if (removeConstraints)
			{
				RemoveConstraints();
			}
			StartCoroutine("RealDead",deathTime);
		}
	}

	IEnumerator RealDead(float deathTime)
	{
		yield return new WaitForSeconds(deathTime);
		Respawn();
		_playerMeshRenderer.enabled = true;
		_deadReset = false;
		dead = false;
	}


	public bool OnEdge()
	{
		float offset = 0.1f;
		float bias = 0.2f;
		
		if (_bCH.CollidingFromDirection(-transform.up, groundLayerMask, bias, offset))
		{
			
			return !_inCollider;
		}
		return true;
	}


	public void Respawn()
	{
		_bCH.boxCollider.enabled = true;
		_player.transform.rotation = Quaternion.identity;
		_rigidbody.velocity = Vector3.zero;
		_rigidbody.angularVelocity = Vector3.zero;
		_rigidbody.isKinematic = false;
		_rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
		transform.position = respawnPoint.position;
		onRespawn.Invoke();
	}


	public void UseGravity(bool enabled)
	{
		_rigidbody.useGravity = enabled;
	}
	

	public void IsKinematic(bool kinematic)
	{
		_rigidbody.isKinematic = kinematic;
		_rigidbody.interpolation = kinematic ? RigidbodyInterpolation.None : RigidbodyInterpolation.Interpolate; 
	}


	public Rigidbody GetRigidbody()
	{
		return _rigidbody;
	}

	public void RemoveConstraints()
	{
		_rigidbody.constraints = RigidbodyConstraints.None;
	}


	public void Recover()
	{
		_rigidbody.velocity = Vector3.up * 2;
		MoveToZ();
		_player.transform.rotation = Quaternion.identity;
		_rigidbody.angularVelocity = Vector3.zero;
		_rigidbody.isKinematic = false;
		_rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
	}


	public void SnapToZ()
	{
		transform.position = new Vector3(transform.position.x, transform.position.y, positionZ);
	}


	public void MoveToZ(float speed = 1)
	{
		Vector3 newPosition = new Vector3(transform.position.x, transform.position.y, positionZ);
		transform.position = Vector3.MoveTowards(transform.position, newPosition, speed * Time.deltaTime);
	}


	public void Step(Vector3 direction, float stepSpeed)
	{
		
		float offset = 0.1f;
		Vector3 centerOffset = Vector3.down * (0.5f - step);
		bool stepOver, somethingInWay;
		//checks if there's something to step over
		stepOver = _bCH.CollidingFromDirection(centerOffset, direction, groundLayerMask, step * 2, offset);

		//makes sure nothing is in the way
		somethingInWay = _bCH.CollidingFromDirection(direction, groundLayerMask, 1-(step * 2), offset);
		if (stepOver && !somethingInWay && direction.magnitude > 0)
		{
			_rigidbody.velocity = (Vector3.up * stepSpeed) + (direction * stepSpeed/2) ;
			
		}
	}
}
