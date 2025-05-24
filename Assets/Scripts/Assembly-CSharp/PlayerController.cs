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
		if (arsenal.Length != 0)
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
					GameObject obj = UnityEngine.Object.Instantiate(arsenal.rightGun);
					obj.transform.parent = rightGunBone;
					obj.transform.localPosition = Vector3.zero;
					obj.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
				}
				if (arsenal.leftGun != null)
				{
					GameObject obj2 = UnityEngine.Object.Instantiate(arsenal.leftGun);
					obj2.transform.parent = leftGunBone;
					obj2.transform.localPosition = Vector3.zero;
					obj2.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
				}
				animator.runtimeAnimatorController = arsenal.controller;
				break;
			}
		}
	}
}
