using UnityEngine;

public class InvZombie : BotController
{
	private CharacterController characterController;

	public bool useNavMesh = true;

	private Vector3 moveDelta = Vector3.zero;

	protected override void Awake()
	{
		if (ArenaScript.instance != null)
		{
			useNavMesh = ArenaScript.instance.isNavMeshArena;
		}
		if (useNavMesh)
		{
			Object.Destroy(GetComponent<CharacterController>());
			Object.Destroy(GetComponent<Rigidbody>());
		}
		else
		{
			Object.Destroy(agent);
			characterController = GetComponent<CharacterController>();
			characterController.enabled = true;
		}
	}

	protected override void Start()
	{
		base.Start();
		if (!useNavMesh)
		{
			GetComponent<Rigidbody>().useGravity = false;
			GetComponent<Rigidbody>().isKinematic = true;
		}
	}

	protected override void Update()
	{
		base.Update();
		if (!useNavMesh)
		{
			MoveToTarget();
		}
	}

	private void MoveToTarget()
	{
		if (!(targetPlayer == null) && !(hp <= 0f) && (base.photonView.isMine || !(targetPlayer == null)))
		{
			if (!isAttacking)
			{
				moveDelta = (targetPlayer.transform.position - base.transform.position).normalized * 4f;
				moveDelta.y = -1f;
				characterController.SimpleMove(moveDelta);
			}
			base.transform.rotation = Quaternion.LookRotation(targetPlayer.transform.position - base.transform.position);
			base.transform.SetEulerX(0f);
			base.transform.SetEulerZ(0f);
		}
	}
}
