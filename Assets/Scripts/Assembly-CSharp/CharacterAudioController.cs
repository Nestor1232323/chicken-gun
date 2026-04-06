using UnityEngine;

public class CharacterAudioController : MonoBehaviour
{
	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private AudioClip makeKillSound;

	[SerializeField]
	private AudioClip deathSound;

	private void Awake()
	{
	}

	public void PlayMakeKillSound()
	{
		audioSource.PlayOneShot(makeKillSound);
	}

	public void PlayDeathSound()
	{
		audioSource.PlayOneShot(deathSound);
	}
}
