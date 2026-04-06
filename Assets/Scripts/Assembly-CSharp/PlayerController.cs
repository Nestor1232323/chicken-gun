using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
	[Serializable]
	public struct Arsenal
	{
		public string name;

		public GameObject rightGun;

		public GameObject leftGun;

		public RuntimeAnimatorController controller;
	}

	public Transform rightGunBone;

	public Transform leftGunBone;

	public Arsenal[] arsenal;

	private Animator animator;

	private void Awake()
	{
		animator = GetComponent<Animator>();
		if (arsenal.Length > 0)
		{
			SetArsenal(arsenal[0].name);
		}
	}

	public void SetArsenal(string name)
	{
		Arsenal[] array = this.arsenal;
		for (int i = 0; i < array.Length; i++)
		{
			Arsenal arsenal = array[i];
			if (arsenal.name == name)
			{
				if (rightGunBone.childCount > 0)
				{
					UnityEngine.Object.Destroy(rightGunBone.GetChild(0).gameObject);
				}
				if (leftGunBone.childCount > 0)
				{
					UnityEngine.Object.Destroy(leftGunBone.GetChild(0).gameObject);
				}
				if (arsenal.rightGun != null)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate(arsenal.rightGun);
					gameObject.transform.parent = rightGunBone;
					gameObject.transform.localPosition = Vector3.zero;
					gameObject.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
				}
				if (arsenal.leftGun != null)
				{
					GameObject gameObject2 = UnityEngine.Object.Instantiate(arsenal.leftGun);
					gameObject2.transform.parent = leftGunBone;
					gameObject2.transform.localPosition = Vector3.zero;
					gameObject2.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
				}
				animator.runtimeAnimatorController = arsenal.controller;
				break;
			}
		}
	}
}
