using UnityEngine;
using UnityEngine.UI;

public class BL_DemoRoundRobin : MonoBehaviour
{
	public Text _sfxPlaying;

	public AudioClip[] sfxRoundRobin1;

	public AudioClip[] sfxRoundRobin2;

	public AudioClip[] sfxRoundRobin3;

	public AudioClip[] sfxRoundRobin4;

	public AudioClip[] sfxRoundRobin5;

	public int currentSFX;

	private BL_Turret _turret;

	private void Start()
	{
		_turret = Object.FindObjectOfType<BL_Turret>();
		Refresh();
	}

	private void Update()
	{
		_sfxPlaying.text = _turret.GetCurrentSFXName();
		if (Input.GetKeyDown(KeyCode.PageDown) || Input.GetAxis("Mouse ScrollWheel") > 0f)
		{
			currentSFX++;
			if (currentSFX > 4)
			{
				currentSFX = 0;
			}
			Refresh();
		}
		if (Input.GetKeyDown(KeyCode.PageUp) || Input.GetAxis("Mouse ScrollWheel") < 0f)
		{
			currentSFX--;
			if (currentSFX < 0)
			{
				currentSFX = 4;
			}
			Refresh();
		}
	}

	private void Refresh()
	{
		switch (currentSFX)
		{
		case 0:
			_turret.sfxFire = sfxRoundRobin1;
			break;
		case 1:
			_turret.sfxFire = sfxRoundRobin2;
			break;
		case 2:
			_turret.sfxFire = sfxRoundRobin3;
			break;
		case 3:
			_turret.sfxFire = sfxRoundRobin4;
			break;
		case 4:
			_turret.sfxFire = sfxRoundRobin5;
			break;
		}
	}
}
