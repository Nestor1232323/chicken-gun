using UnityEngine;

public class BL_MuzzleFlash : MonoBehaviour
{
	private ParticleSystem _particleSystem;

	public void Flash()
	{
		if (_particleSystem == null)
		{
			_particleSystem = base.gameObject.GetComponent<ParticleSystem>();
		}
		_particleSystem.Emit(1);
	}
}
