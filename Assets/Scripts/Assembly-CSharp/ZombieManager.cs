using System;
using ExitGames.Client.Photon;
using Photon;
using UnityEngine;

public class ZombieManager : Photon.MonoBehaviour
{
	private struct Wave
	{
		public float duration;

		public float[] spawnChances;

		public float spawnPeriod;

		public Wave(float duration, float[] spawnChances, float spawnPeriod)
		{
			this.duration = duration;
			this.spawnChances = spawnChances;
			this.spawnPeriod = spawnPeriod;
		}
	}

	[SerializeField]
	private GameObject[] zombiePrefabs;

	public int currentWave;

	public static Action<int> WaveChanged;

	public float totalTime;

	public double zstarttime;

	private Wave[] waves = new Wave[12]
	{
		new Wave(40f, new float[4] { 3f, 3f, 3f, 1f }, 6f),
		new Wave(40f, new float[4] { 3f, 3f, 3f, 1f }, 6f),
		new Wave(40f, new float[4] { 3f, 3f, 3f, 1f }, 6f),
		new Wave(40f, new float[4] { 3f, 3f, 3f, 1f }, 6f),
		new Wave(40f, new float[4] { 3f, 3f, 3f, 1f }, 6f),
		new Wave(40f, new float[4] { 3f, 3f, 3f, 1f }, 6f),
		new Wave(40f, new float[4] { 3f, 3f, 3f, 1f }, 6f),
		new Wave(40f, new float[4] { 3f, 3f, 3f, 1f }, 6f),
		new Wave(40f, new float[4] { 3f, 3f, 3f, 1f }, 6f),
		new Wave(40f, new float[4] { 3f, 3f, 3f, 1f }, 6f),
		new Wave(40f, new float[4] { 3f, 3f, 3f, 1f }, 6f),
		new Wave(40f, new float[4] { 3f, 3f, 3f, 1f }, 6f)
	};

	private float[] currentSpawnChances;

	private float ZombieSpawnPeriod => CurrentWave.spawnPeriod;

	private Wave CurrentWave
	{
		get
		{
			if (currentWave > waves.Length - 1)
			{
				return waves[waves.Length - 1];
			}
			return waves[currentWave];
		}
	}

	private void Start()
	{
		if (PhotonNetwork.isMasterClient)
		{
			currentSpawnChances = CurrentWave.spawnChances;
			InvokeRepeating("CreateZombie", 1f, ZombieSpawnPeriod);
			zstarttime = PhotonNetwork.time;
			Hashtable hashtable = new Hashtable();
			hashtable.Add("zstarttime", zstarttime);
			PhotonNetwork.room.SetCustomProperties(hashtable);
			UnityEngine.MonoBehaviour.print("totalTime  =  " + PhotonNetwork.room.CustomProperties["zstarttime"]);
		}
		else
		{
			UnityEngine.MonoBehaviour.print("zstarttime  =  " + zstarttime);
			zstarttime = (double)PhotonNetwork.room.CustomProperties["zstarttime"];
		}
	}

	private void Update()
	{
		if (PhotonNetwork.isMasterClient)
		{
			CheckWave();
		}
	}

	private void CreateZombie()
	{
		if (UnityEngine.Object.FindObjectsOfType<InvZombie>().Length > 20)
		{
			return;
		}
		Vector3 vector = ArenaScript.instance.ZombieSpawnPoints[UnityEngine.Random.Range(0, ArenaScript.instance.ZombieSpawnPoints.Length)].position;
		if (Physics.Raycast(vector + Vector3.up * 5f, Vector3.down, out var hitInfo, 250f))
		{
			vector = hitInfo.point - Vector3.up * 0f;
		}
		int num = 0;
		float value = UnityEngine.Random.value;
		float sum = 0f;
		Array.ForEach(currentSpawnChances, delegate(float i)
		{
			sum += i;
		});
		float[] array = new float[currentSpawnChances.Length];
		array[0] = currentSpawnChances[0] / sum;
		for (int num2 = 1; num2 < array.Length; num2++)
		{
			array[num2] = array[num2 - 1] + currentSpawnChances[num2] / sum;
		}
		for (int num3 = 0; num3 < array.Length - 1; num3++)
		{
			if (value > array[num3])
			{
				num = num3 + 1;
			}
		}
		Debug.Log("RandomVal = " + value + "zombieIndex " + num);
		if (num > zombiePrefabs.Length - 1)
		{
			num = zombiePrefabs.Length - 1;
		}
		PhotonNetwork.Instantiate(zombiePrefabs[num].name, vector, Quaternion.identity, 0);
	}

	private void CheckWave()
	{
		if (!PhotonNetwork.isMasterClient)
		{
			return;
		}
		double num = PhotonNetwork.time - zstarttime;
		float num2 = 0f;
		for (int i = 0; i < waves.Length; i++)
		{
			if (num > (double)num2 && i > currentWave)
			{
				currentWave = i;
				PhotonNetwork.RPC(base.photonView, "WaveChangedR", PhotonTargets.All, false, currentWave);
				break;
			}
			num2 += waves[i].duration;
		}
	}

	[PunRPC]
	private void WaveChangedR(int wave)
	{
		currentWave = wave;
		currentSpawnChances = CurrentWave.spawnChances;
		if (WaveChanged != null)
		{
			WaveChanged(wave);
		}
		Debug.Log("wave = " + wave);
	}

	private void OnMasterClientSwitched()
	{
		if (PhotonNetwork.isMasterClient)
		{
			CancelInvoke("CreateZombie");
			InvokeRepeating("CreateZombie", 1f, ZombieSpawnPeriod);
		}
		else
		{
			CancelInvoke("CreateZombie");
		}
	}
}
