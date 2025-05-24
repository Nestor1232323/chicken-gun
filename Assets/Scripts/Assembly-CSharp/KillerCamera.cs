using System.Collections;
using UnityEngine;

public class KillerCamera : BaseCamera
{
	public Transform target;

	private bool lookingFX;

	private void Start()
	{
	}

	private void LateUpdate()
	{
		if (!lookingFX && target != null)
		{
			base.transform.position = target.transform.position - Vector3.right * 3f + Vector3.up * 1.92f;
			base.transform.LookAt(target.position + Vector3.up * 1.47f);
		}
	}

	public override void SetTarget(Transform target)
	{
		base.transform.SetParent(null);
		this.target = target;
		StartCoroutine(LookFX());
	}

	private IEnumerator LookFX()
	{
		lookingFX = true;
		Vector3 startPos = base.transform.position;
		Vector3 endPos = base.transform.position - base.transform.forward * 2.9f + Vector3.up * 0.65f;
		float T = 0.4f;
		float t = 0f;
		Quaternion q1 = base.transform.rotation;
		Quaternion q2 = Quaternion.LookRotation(startPos - endPos);
		while (t < T)
		{
			base.transform.position = Vector3.Lerp(startPos, endPos, t / T);
			base.transform.rotation = Quaternion.Lerp(q1, q2, t / T);
			t += Time.deltaTime;
			yield return null;
		}
		yield return new WaitForSeconds(0.4f);
		if (target != GameController.instance.OurPlayer.transform)
		{
			lookingFX = false;
		}
	}

	public override void Enable()
	{
		base.enabled = true;
		base.Enable();
		base.transform.SetParent(null);
	}

	public override void Disable()
	{
		base.Disable();
		base.enabled = false;
		target = null;
	}
}
