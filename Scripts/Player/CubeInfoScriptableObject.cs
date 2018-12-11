using UnityEngine;

[CreateAssetMenu(fileName = "CubeInfo", menuName = "CubeInfo")]

public class CubeInfoScriptableObject: ScriptableObject
{
	public float speed = 2;
	public float speedBoostSpeed = 4;
	public float jumpForce = 10;
	public float gravityScale = 1;
	public float jumpHeight = 2f;
	public int maxExtraJumps = 1;
	public float deathTime = 3;
	public Texture[] skins;
}
