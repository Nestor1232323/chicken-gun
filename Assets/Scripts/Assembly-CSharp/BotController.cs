using Photon;
using UnityEngine;
using UnityEngine.AI;

public class BotController : Photon.MonoBehaviour, IDestroyable, IDamageReciver
{
	[SerializeField]
	protected Animator animator;

	[SerializeField]
	private Animation animation2;

	[SerializeField]
	protected NavMeshAgent agent;

	[SerializeField]
	protected float damagePerAttack;

	[SerializeField]
	protected float attackDist;

	[SerializeField]
	protected CharacterMotor targetPlayer;

	private Vector3 targetPoint;

	[SerializeField]
	protected float hp;

	public bool isAttacking;

	[SerializeField]
	protected int attackTypesCount = 1;

	private float UpdateTargetPeriod = 2f;

	[SerializeField]
	protected float destroyingDuration = 2.5f;

	[SerializeField]
	private AudioSource myVoiceAudio;

	[SerializeField]
	private AudioSource hitAudio;

	[SerializeField]
	private AudioClip dieSound;

	protected virtual void Awake()
	{
		agent.enabled = base.photonView.isMine;
	}

	protected virtual void Start()
	{
		if (base.photonView.isMine)
		{
			InvokeRepeating("UpdateTarget", 0f, UpdateTargetPeriod);
		}
	}

	protected virtual void Update()
	{
		if (base.photonView.isMine)
		{
			CheckAttack();
		}
	}

	protected virtual void CheckAttack()
	{
		if (!(targetPlayer != null) || !(hp > 0f))
		{
			return;
		}
		if ((base.transform.position - targetPlayer.transform.position).sqrMagnitude < attackDist * attackDist)
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

	public virtual void Damage(float dmg, int fromWhom = 0)
	{
		ApplyDamage(dmg, fromWhom);
	}

	public virtual void Heal(float val)
	{
		ApplyHeal(val);
	}

	public void ApplyDamage(float val, int fromWhom)
	{
		if (base.photonView != null && !(hp <= 0f))
		{
			hp -= val;
			PhotonNetwork.RPC(base.photonView, "UpdateHP", PhotonTargets.All, false, hp);
		}
	}

	[PunRPC]
	public void UpdateHP(float newHP)
	{
		hp = newHP;
		if (hp <= 0f)
		{
			if (animator != null)
			{
				animator.CrossFade("Die", 0.8f);
			}
			else if (animation2 != null)
			{
				animation2.Play("Die");
			}
			if (agent != null && agent.isOnNavMesh)
			{
				agent.isStopped = true;
			}
			GetComponent<CapsuleCollider>().height = GetComponent<CapsuleCollider>().height * 0.2f;
			GetComponent<CapsuleCollider>().center = Vector3.zero;
			if (base.photonView.isMine)
			{
				Invoke("DestroySelf", destroyingDuration);
			}
			myVoiceAudio.clip = dieSound;
			myVoiceAudio.loop = false;
			myVoiceAudio.PlayOneShot(dieSound);
		}
	}

	protected virtual void DestroySelf()
	{
		PhotonNetwork.Destroy(base.gameObject);
	}

	public void ApplyHeal(float val)
	{
	}

	protected virtual void UpdateTarget()
	{
		if (GameController.instance.OurPlayer == null)
		{
			return;
		}
		_ = GameController.instance.OurPlayer.transform.position;
		float num = (GameController.instance.OurPlayer.transform.position - base.transform.position).sqrMagnitude;
		targetPlayer = GameController.instance.OurPlayer;
		foreach (CharacterMotor player in GameController.instance.Players)
		{
			if (player.IsAlive())
			{
				float sqrMagnitude = (player.transform.position - base.transform.position).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					num = sqrMagnitude;
					_ = player.transform.position;
					targetPlayer = player;
				}
			}
		}
		PhotonNetwork.RPC(base.photonView, "UpdateTargetR", PhotonTargets.All, false);
		if (agent != null && agent.enabled)
		{
			agent.SetDestination(targetPlayer.transform.position);
		}
	}

	[PunRPC]
	protected virtual void UpdateTargetR()
	{
	}

	protected virtual void OnMasterClientSwitched()
	{
		if (PhotonNetwork.isMasterClient)
		{
			CancelInvoke("UpdateTarget");
			InvokeRepeating("UpdateTarget", 0f, UpdateTargetPeriod);
		}
		else
		{
			CancelInvoke("UpdateTarget");
		}
		if (agent != null)
		{
			agent.enabled = PhotonNetwork.isMasterClient;
		}
	}

	protected virtual void StartAttack()
	{
		if (agent != null && agent.enabled)
		{
			agent.isStopped = true;
		}
		int num = Random.Range(0, attackTypesCount) + 1;
		PhotonNetwork.RPC(base.photonView, "StartAttackR", PhotonTargets.All, false, num);
	}

	[PunRPC]
	protected virtual void StartAttackR(int attackIndex)
	{
		if (animator != null)
		{
			animator.SetBool("Attack", value: true);
		}
		else if (animation2 != null)
		{
			animation2.Play("Attack");
		}
		isAttacking = true;
	}

	protected virtual void StopAttack()
	{
		if (agent != null && agent.enabled)
		{
			agent.isStopped = false;
		}
		PhotonNetwork.RPC(base.photonView, "StopAttackR", PhotonTargets.All, false);
	}

	[PunRPC]
	protected virtual void StopAttackR()
	{
		isAttacking = false;
		if (animator != null)
		{
			animator.SetBool("Attack", value: false);
		}
		else if (animation2 != null)
		{
			animation2.Play("Run");
		}
	}

	public virtual void DamageTarget()
	{
		if (!(targetPlayer == null))
		{
			hitAudio.Play();
			if (base.photonView.isMine && (base.transform.position - targetPlayer.transform.position).sqrMagnitude <= attackDist * attackDist)
			{
				targetPlayer.GetComponent<DamageReciver2>().Damage(damagePerAttack);
			}
		}
	}
}
