using UnityEngine;

public class Butterfly : MonoBehaviour
{
	public Vector3 zoneSize = Vector3.one;

	public GameObject[] butterflyObjects;

	public int butterflyCount;

	public float maxSpeed = 1f;

	public float arrivalRadius = 0.2f;

	private Vector3[] targets;

	private Transform[] flies;

	private Vector3[] velocities;

	private void Start()
	{
		targets = new Vector3[butterflyCount];
		flies = new Transform[butterflyCount];
		velocities = new Vector3[butterflyCount];
		for (int i = 0; i < butterflyCount; i++)
		{
			GameObject gameObject = Object.Instantiate(butterflyObjects[Random.Range(0, butterflyObjects.Length - 1)], new Vector3(base.transform.position.x + Random.Range(0f - zoneSize.x, zoneSize.x) / 2f, base.transform.position.y + Random.Range(0f - zoneSize.y, zoneSize.y) / 2f, base.transform.position.z + Random.Range(0f - zoneSize.z, zoneSize.z) / 2f), Quaternion.identity);
			flies[i] = gameObject.transform;
			targets[i] = GetRandomTarget(flies[i].position);
		}
	}

	private void Update()
	{
		for (int i = 0; i < butterflyCount; i++)
		{
			flies[i].LookAt(targets[i]);
			if (Seek(i))
			{
				targets[i] = GetRandomTarget(flies[i].position);
			}
		}
	}

	private Vector3 GetRandomTarget(Vector3 position)
	{
		return new Vector3(base.transform.position.x + Random.Range(0f - zoneSize.x, zoneSize.x) / 2f, base.transform.position.y + Random.Range(0f - zoneSize.y, zoneSize.y) / 2f, base.transform.position.z + Random.Range(0f - zoneSize.z, zoneSize.z) / 2f);
	}

	private bool Seek(int index)
	{
		flies[index].position += velocities[index];
		Vector3 vector = targets[index] - flies[index].position;
		if (vector.magnitude > arrivalRadius)
		{
			vector.Normalize();
			vector *= maxSpeed * Time.deltaTime;
			velocities[index] = vector;
			return false;
		}
		return true;
	}
}
