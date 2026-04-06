using UnityEngine;

public class CharacterInputController : MonoBehaviour
{
	public void Disable()
	{
		base.enabled = false;
	}

	public void Enable()
	{
		base.enabled = true;
	}
}
