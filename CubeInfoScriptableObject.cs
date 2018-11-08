using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CubeInfo", menuName = "CubeInfo")]

public class CubeInfoScriptableObject: ScriptableObject
{
	public float speed = 1;
	public float jumpForce = 10;
	public float gravityScale = 1;
	public float jumpTime = 0.1f;
	public int maxExtraJumps = 1;
	public float deathTime = 3;
	public Texture[] skins;
}
