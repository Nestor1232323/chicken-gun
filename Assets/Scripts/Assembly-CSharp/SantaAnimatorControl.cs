using System.Collections;
using UnityEngine;

public class SantaAnimatorControl : MonoBehaviour
{
	public Animator chrAnimator;

	public RuntimeAnimatorController[] chrAnimatorController;

	public CharacterController chrController;

	[Space(20f)]
	public SkinnedMeshRenderer mat;

	public Texture2D[] tex;

	private int texIdx;

	[Space(20f)]
	public GameObject[] items;

	private GameObject itemInHand;

	public Transform[] itemPoint = new Transform[2];

	public GameObject[] meshData;

	[Space(20f)]
	public float jumpSpeed = 8f;

	public float moveAbilityInAir = 4f;

	private float jumpAmount;

	private float runParam;

	private Vector3 moveDirection = Vector3.zero;

	private float gravity = 10f;

	private AnimatorStateInfo stateInfo;

	public float[] throughPower = new float[3];

	private void Update()
	{
		stateInfo = chrAnimator.GetCurrentAnimatorStateInfo(0);
		if (!stateInfo.IsTag("InAttack"))
		{
			chrAnimator.SetInteger("AttackIdx", 0);
		}
		if (Input.GetButtonDown("Fire1") && Input.GetKey("z"))
		{
			SetAttack(1);
		}
		else if (Input.GetButtonDown("Fire1") && Input.GetButton("Fire2"))
		{
			SetAttack(3);
		}
		else if (Input.GetButtonDown("Fire1"))
		{
			SetAttack(2);
		}
		if (Input.GetButtonDown("Fire2") && stateInfo.IsName("na_Idle_00"))
		{
			chrAnimator.SetBool("Items_Bool", value: true);
		}
		if (Input.GetKeyDown("x"))
		{
			chrAnimator.SetBool("Guard_Bool", value: true);
		}
		if (Input.GetKeyUp("x"))
		{
			chrAnimator.SetBool("Guard_Bool", value: false);
		}
		if (Input.GetKeyDown("c"))
		{
			chrAnimator.SetTrigger("Damage_Trg");
		}
		if (Input.GetKeyDown("v") && (stateInfo.IsName("na_Idle_00") || stateInfo.IsName("na_Failed_Loop_00")))
		{
			chrAnimator.SetBool("Failed_Bool", !chrAnimator.GetBool("Failed_Bool"));
		}
		if (Input.GetKeyDown("b") && (stateInfo.IsName("na_Idle_00") || stateInfo.IsName("na_Success_Loop_00")))
		{
			chrAnimator.SetBool("Success_Bool", !chrAnimator.GetBool("Success_Bool"));
		}
		float axis = Input.GetAxis("Horizontal");
		float axis2 = Input.GetAxis("Vertical");
		Vector3 vector = new Vector3(axis, 0f, axis2);
		float num = vector.magnitude;
		if (num > 1f)
		{
			num = 1f;
			vector.Normalize();
		}
		runParam = 0f;
		if (num != 0f)
		{
			if (Input.GetKey("z"))
			{
				runParam = 1f;
			}
			vector = Camera.main.transform.rotation * vector;
			vector.y = 0f;
			if (vector != Vector3.zero)
			{
				base.transform.forward = vector;
			}
		}
		chrAnimator.SetFloat("Speed", num + runParam);
		if (chrController.isGrounded)
		{
			if (stateInfo.IsName("na_Jump_00_fall") || stateInfo.IsName("na_Jump_01_fall"))
			{
				chrAnimator.SetInteger("JumpIdx", 0);
				jumpAmount = 0f;
			}
			if (chrAnimator.GetInteger("JumpIdx") == 0)
			{
				moveDirection = new Vector3(0f, 0f, 0f);
			}
			if (Input.GetButtonDown("Jump"))
			{
				chrAnimator.SetInteger("JumpIdx", 1);
			}
		}
		else if (!chrController.isGrounded)
		{
			if (Input.GetButtonDown("Jump") && chrAnimator.GetInteger("JumpIdx") == 1)
			{
				chrAnimator.SetInteger("JumpIdx", 2);
			}
			moveDirection = new Vector3(vector.x * moveAbilityInAir, moveDirection.y, vector.z * moveAbilityInAir);
			moveDirection.y -= gravity * Time.deltaTime;
			chrAnimator.SetFloat("JumpVelocity", moveDirection.y - jumpAmount * 0.5f);
		}
		chrController.Move(moveDirection * Time.deltaTime);
	}

	private void SetAttack(int param)
	{
		chrAnimator.SetInteger("AttackIdx", param);
	}

	private void InstanceItem()
	{
		if (!itemInHand)
		{
			int num = Random.Range(0, items.Length);
			itemInHand = Object.Instantiate(items[num], itemPoint[0].position, itemPoint[0].rotation);
			itemInHand.transform.parent = itemPoint[0];
			itemInHand.transform.localPosition = Vector3.zero;
			itemInHand.transform.localRotation = Quaternion.identity;
			itemInHand.transform.localScale = Vector3.one;
			itemInHand.GetComponent<ItemControl>().ChangeTextureRandom();
		}
	}

	private void ThroughItem()
	{
		if ((bool)itemInHand)
		{
			itemInHand.transform.parent = null;
			itemInHand.GetComponent<Rigidbody>().isKinematic = false;
			int num = chrAnimator.GetInteger("AttackIdx") - 1;
			if (num < 0)
			{
				num = 0;
				itemInHand.GetComponent<ItemControl>().waitTime = 1f;
			}
			Vector3 force = base.transform.forward * throughPower[num];
			force.y = throughPower[num] * 0.75f;
			itemInHand.GetComponent<Rigidbody>().AddForce(force);
			itemInHand.GetComponent<ItemControl>().InitBullet();
			itemInHand = null;
			chrAnimator.SetBool("Items_Bool", value: false);
		}
	}

	private void SetJump()
	{
		if (chrAnimator.GetInteger("JumpIdx") <= 1)
		{
			moveDirection = new Vector3(0f, jumpSpeed, 0f);
			jumpAmount += jumpSpeed;
			chrAnimator.SetInteger("JumpIdx", 1);
			chrAnimator.SetFloat("JumpVelocity", jumpAmount * 0.5f);
		}
		else if (chrAnimator.GetInteger("JumpIdx") == 2)
		{
			moveDirection.y += jumpSpeed * 1.5f;
			jumpAmount += jumpSpeed * 1.5f;
			chrAnimator.SetInteger("JumpIdx", 3);
		}
	}

	public void ControllerChange(int idx)
	{
		if (base.gameObject.activeSelf)
		{
			StartCoroutine(AnimControllerChange(idx));
		}
		else
		{
			chrAnimator.runtimeAnimatorController = chrAnimatorController[idx];
		}
	}

	private IEnumerator AnimControllerChange(int idx)
	{
		PlayClip("na_Idle_00", 0);
		yield return new WaitForSeconds(0.1f);
		chrAnimator.runtimeAnimatorController = chrAnimatorController[idx];
		PlayClip("na_Idle_00", 0);
	}

	public void PlayClip(string stateName, int item)
	{
		if (item == 0 && (bool)itemInHand)
		{
			itemInHand.GetComponent<ItemControl>().DestroyItem(0f);
		}
		else if (item == 1 && !itemInHand)
		{
			InstanceItem();
		}
		chrAnimator.CrossFade(stateName, 0.05f);
	}

	public string MeshData()
	{
		int[] array = new int[3];
		array = GetMeshProperty(meshData);
		return "Character\n      Vertex : " + array[0] + ", Tris : " + array[1] + ", Bones : " + array[2];
	}

	private GameObject[] CollectMeshRenderer(GameObject rootObject)
	{
		SkinnedMeshRenderer[] componentsInChildren = rootObject.GetComponentsInChildren<SkinnedMeshRenderer>();
		MeshRenderer[] componentsInChildren2 = rootObject.GetComponentsInChildren<MeshRenderer>();
		GameObject[] array;
		if (componentsInChildren.Length + componentsInChildren2.Length == 0)
		{
			array = new GameObject[1];
		}
		else
		{
			array = new GameObject[componentsInChildren.Length + componentsInChildren2.Length];
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				array[i] = componentsInChildren[i].gameObject;
			}
			for (int j = 0; j < componentsInChildren2.Length; j++)
			{
				array[j + componentsInChildren.Length] = componentsInChildren2[j].gameObject;
			}
		}
		return array;
	}

	private int[] GetMeshProperty(GameObject[] mesh)
	{
		int[] array = new int[3];
		Transform[] array2 = null;
		if (mesh[0] != null)
		{
			for (int i = 0; i < mesh.Length; i++)
			{
				SkinnedMeshRenderer component = mesh[i].GetComponent<SkinnedMeshRenderer>();
				if ((bool)component)
				{
					array[0] += component.sharedMesh.vertices.Length;
					array[1] += component.sharedMesh.triangles.Length / 3;
					array2 = ((i != 0) ? RejectDoubledBones(array2, component.bones) : component.bones);
					array[2] = array2.Length;
				}
				else
				{
					array[0] += mesh[i].GetComponent<MeshFilter>().sharedMesh.vertices.Length;
					array[1] += mesh[i].GetComponent<MeshFilter>().sharedMesh.triangles.Length / 3;
					array[2] = array[2];
				}
			}
		}
		return array;
	}

	private Transform[] RejectDoubledBones(Transform[] boneListA, Transform[] boneListB)
	{
		Transform[] array = new Transform[boneListB.Length];
		int num = 0;
		for (int i = 0; i < boneListB.Length; i++)
		{
			bool flag = false;
			for (int j = 0; j < boneListA.Length; j++)
			{
				if (boneListB[i] == boneListA[j])
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				array[num] = boneListB[i];
				num++;
			}
		}
		Transform[] array2 = new Transform[boneListA.Length + num];
		for (int k = 0; k < boneListA.Length; k++)
		{
			array2[k] = boneListA[k];
		}
		for (int l = 0; l < num; l++)
		{
			array2[l + boneListA.Length] = array[l];
		}
		return array2;
	}

	public void SetShader(int shaderId)
	{
		string[] array = new string[3] { "Specular", "Diffuse", "Unlit/Texture" };
		for (int i = 0; i < meshData.Length; i++)
		{
			SkinnedMeshRenderer component = meshData[i].GetComponent<SkinnedMeshRenderer>();
			if ((bool)component)
			{
				component.material.shader = Shader.Find(array[shaderId]);
			}
			else
			{
				meshData[i].GetComponent<MeshRenderer>().material.shader = Shader.Find(array[shaderId]);
			}
		}
	}

	public void ChangeTexture(bool isResetTex)
	{
		if (isResetTex)
		{
			texIdx = 0;
		}
		else
		{
			texIdx++;
			texIdx = (int)Mathf.Repeat(texIdx, tex.Length);
		}
		mat.material.mainTexture = tex[texIdx];
	}
}
