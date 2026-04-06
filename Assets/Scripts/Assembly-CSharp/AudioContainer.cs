using UnityEngine;

public class AudioContainer : MonoBehaviour
{
	public static AudioContainer instance;

	public SoundsPack ClassicSoundsPack;

	public SoundsPack MuscleSoundsPack;

	public SoundsPack SportsCarSoundsPack;

	public SoundsPack TruckSoundsPack;

	public AudioClip[] HitSounds;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
			Object.DontDestroyOnLoad(base.gameObject);
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	public AudioClip GetAudioClip(CarSoundFamily family, SoundID soundID, int gradation = 0)
	{
		SoundsPack soundsPack = null;
		switch (family)
		{
		case CarSoundFamily.Classic:
			soundsPack = ClassicSoundsPack;
			break;
		case CarSoundFamily.Muscle:
			soundsPack = MuscleSoundsPack;
			break;
		case CarSoundFamily.SportsCar:
			soundsPack = SportsCarSoundsPack;
			break;
		case CarSoundFamily.Truck:
			soundsPack = TruckSoundsPack;
			break;
		}
		if (soundsPack != null)
		{
			switch (soundID)
			{
			case SoundID.Acceleration:
				return soundsPack.acceleration;
			case SoundID.EngineStart:
				return soundsPack.engineStart;
			case SoundID.EngineIdle:
				return soundsPack.endineIdle;
			case SoundID.StableMaxSpeed:
				return soundsPack.stableMaxSpeedSound;
			case SoundID.Deceleration:
				return soundsPack.deceleration;
			case SoundID.Drift:
				return soundsPack.drift;
			case SoundID.Brake:
				return soundsPack.brake;
			case SoundID.Explosion:
				return soundsPack.explosion;
			case SoundID.Hit:
				return HitSounds[gradation];
			}
		}
		return null;
	}
}
