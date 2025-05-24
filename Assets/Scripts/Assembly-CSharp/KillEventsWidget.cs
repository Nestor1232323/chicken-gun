using System.Collections;
using UnityEngine;

public class KillEventsWidget : MonoBehaviour
{
	[SerializeField]
	private GameObject kill;

	[SerializeField]
	private GameObject killAssist;

	[SerializeField]
	private GameObject revenge;

	[SerializeField]
	private AudioClip killSoundFX;

	[SerializeField]
	private AudioClip killAssistSoundFX;

	[SerializeField]
	private AudioClip revengeSoundFX;

	private AudioSource audioSource;

	private void Start()
	{
		audioSource = GetComponent<AudioSource>();
	}

	private void OnDestroy()
	{
	}

	public void OnPlayerMakeKill()
	{
		kill.SetActive(value: true);
		audioSource.PlayOneShot(killSoundFX);
		StartCoroutine(HideOnject(kill));
	}

	public void OnPlayerMakeKillAssist()
	{
		killAssist.SetActive(value: true);
		audioSource.PlayOneShot(killAssistSoundFX);
		StartCoroutine(HideOnject(killAssist));
	}

	public void OnPlayerMakeRevenge()
	{
		revenge.SetActive(value: true);
		audioSource.PlayOneShot(revengeSoundFX);
		StartCoroutine(HideOnject(revenge));
	}

	private IEnumerator HideOnject(GameObject obj)
	{
		yield return new WaitForSeconds(1.3f);
		obj.SetActive(value: false);
	}
}
