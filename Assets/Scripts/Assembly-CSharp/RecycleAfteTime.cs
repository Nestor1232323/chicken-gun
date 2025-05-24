using System.Collections;
using UnityEngine;

public class RecycleAfteTime : MonoBehaviour
{
	public float time = 2f;

	private IEnumerator Start()
	{
		yield return new WaitForSeconds(time);
		base.gameObject.Recycle();
	}
}
