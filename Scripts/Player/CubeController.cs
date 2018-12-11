
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
	BoxCollisionHelp _bCH;
	const float _COLLISION_BIAS = 0.98f;

	public Transform respawnPoint;
	public bool moving;
	public LayerMask moveLayerMask;
	public LayerMask groundLayerMask;
	
	bool _deadReset;
	public bool _inCollider;
	public bool dead;

	private void Awake()
	{
		SetPlayerGameObjectAndRigidbody();
		_bCH = GetComponent<BoxCollisionHelp>();
	}

	private void OnCollisionStay(Collision collision)
	{
		_inCollider = true;
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
			moving = true;
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
	public void Jump(float speed, bool jumping, bool distanceReached = false)
	{
		if (jumping && !distanceReached)
		{
			_rigidbody.velocity = new Vector3(_rigidbody.velocity.x, speed, _rigidbody.velocity.z);
		}
	}

	public void Dead(float deathTime)
	{
		if (!_deadReset)
		{
			if (!dead)
			{
				_deadReset = true;
			}
			dead = true;
			StartCoroutine("RealDead",deathTime);
		}
	}
	IEnumerator RealDead(float deathTime)
	{
		yield return new WaitForSeconds(deathTime);
		_rigidbody.velocity = Vector3.zero;
		Respawn();
		_deadReset = false;
		dead = false;
	}

	public void Respawn()
	{
		transform.position = respawnPoint.position;
	}

	public void UseGravity(bool enabled)
	{
		_rigidbody.useGravity = enabled;
	}
	
	public void IsKinematic(bool kinematic)
	{
		_rigidbody.isKinematic = kinematic;
	}

	public Rigidbody GetRigidbody()
	{
		return _rigidbody;
	}
}
