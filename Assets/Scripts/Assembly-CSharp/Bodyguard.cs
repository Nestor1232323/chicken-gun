using System;
using Photon;
using UnityEngine;

public class Bodyguard : Photon.MonoBehaviour, IDestroyable
{
	private PhotonView myEnemy;

	private CharacterMotor myOwner;

	[SerializeField]
	protected float hp;

	protected Animator animator;

	[SerializeField]
	protected float damagePerAttack;

	[SerializeField]
	protected float attackDist;

	public bool isAttacking;

	[SerializeField]
	private AudioSource hitAudio;

	[SerializeField]
	private float minDistToOwner = 2.1f;

	[SerializeField]
	private float speed = 8f;

	private CharacterController characterController;

	private float lastChTime1;

	private bool isChasingOwner;

	private void Awake()
	{
		animator = GetComponent<Animator>();
		if (animator == null)
		{
			animator = GetComponentInChildren<Animator>();
		}
		characterController = GetComponent<CharacterController>();
	}

	private void Start()
	{
		animator.Play("Idle");
		if (base.photonView.isMine)
		{
			CharacterMotor characterMotor = myOwner;
			characterMotor.DamagedBySomeone = (Action<UnityEngine.Object, int>)Delegate.Combine(characterMotor.DamagedBySomeone, new Action<UnityEngine.Object, int>(OnMyOwnerDamaged));
		}
	}

	protected virtual void LateUpdate()
	{
		if (base.photonView.isMine)
		{
			CheckAttack();
			KeepDistToOwner();
		}
		else
		{
			UpdateCloneState();
		}
	}

	public void ApplyHeal(float val)
	{
	}

	private void KeepDistToOwner()
	{
		if (hp <= 0f || myOwner == null)
		{
			return;
		}
		Vector3 vector = myOwner.transform.position - base.transform.position;
		if (vector.sqrMagnitude >= minDistToOwner * minDistToOwner)
		{
			base.transform.LookAt(myOwner.transform);
			base.transform.SetLocalEulerX(0f);
			base.transform.SetLocalEulerZ(0f);
			characterController.SimpleMove(vector.normalized * speed);
			if (!isChasingOwner)
			{
				PhotonNetwork.RPC(base.photonView, "SyncAnimRun", PhotonTargets.All, false);
			}
			lastChTime1 = Time.time;
			isChasingOwner = true;
		}
		else if (Time.time - lastChTime1 > 1f && isChasingOwner)
		{
			PhotonNetwork.RPC(base.photonView, "SyncAnimIdle", PhotonTargets.All, false);
			isChasingOwner = false;
		}
	}

	[PunRPC]
	private void SyncAnimRun()
	{
		animator.Play("Run");
	}

	[PunRPC]
	private void SyncAnimIdle()
	{
		animator.CrossFade("Idle", 0.3f);
	}

	public void ApplyDamage(float val, int fromWhom)
	{
		if (base.photonView != null)
		{
			PhotonNetwork.RPC(base.photonView, "DamageRPC", PhotonTargets.All, false, val, fromWhom);
		}
	}

	[PunRPC]
	public void DamageRPC(float dmg, int fromWhom)
	{
		if (!(hp <= 0f) && base.photonView.isMine)
		{
			hp -= dmg;
			PhotonNetwork.RPC(base.photonView, "UpdateHP", PhotonTargets.All, false, hp);
		}
	}

	[PunRPC]
	public void UpdateHP(float newHP)
	{
		hp = newHP;
		if (hp <= 0f)
		{
			animator.Play("Die");
			if (base.photonView.isMine)
			{
				Invoke("DestroySelf", 3f);
			}
		}
	}

	private void OnMyOwnerDamaged(UnityEngine.Object sender, int frommWhomViewId)
	{
		PhotonView photonView = PhotonView.Find(frommWhomViewId);
		if (photonView != null && (photonView.transform.position - myOwner.transform.position).sqrMagnitude < 225f)
		{
			myEnemy = photonView;
		}
	}

	protected virtual void CheckAttack()
	{
		if (!(myEnemy != null) || !(hp > 0f))
		{
			return;
		}
		if ((base.transform.position - myEnemy.transform.position).sqrMagnitude < attackDist * attackDist)
		{
			if (!isAttacking)
			{
				StartAttack();
			}
		}
		else if (isAttacking)
		{
			StopAttack();
		}
	}

	protected virtual void StartAttack()
	{
		int num = 0;
		PhotonNetwork.RPC(base.photonView, "StartAttackR", PhotonTargets.All, false, num);
	}

	[PunRPC]
	protected virtual void StartAttackR(int attackIndex)
	{
		animator.SetBool("Attack", value: true);
		isAttacking = true;
	}

	protected virtual void StopAttack()
	{
		PhotonNetwork.RPC(base.photonView, "StopAttackR", PhotonTargets.All, false);
	}

	[PunRPC]
	protected virtual void StopAttackR()
	{
		isAttacking = false;
		animator.SetBool("Attack", value: false);
	}

	public virtual void DamageTarget()
	{
		if (!(myEnemy == null))
		{
			hitAudio.Play();
			if (base.photonView.isMine && (base.transform.position - myEnemy.transform.position).sqrMagnitude <= attackDist * attackDist)
			{
				myEnemy.GetComponent<DamageReciver2>().Damage(damagePerAttack);
			}
		}
	}

	public void SetOwner(CharacterMotor owner)
	{
		myOwner = owner;
	}

	protected virtual void DestroySelf()
	{
		PhotonNetwork.Destroy(base.gameObject);
	}

	private void UpdateCloneState()
	{
	}
}
