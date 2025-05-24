using UnityEngine;

public class BotAnimEvents : MonoBehaviour
{
	private BotController myBot;

	private void Start()
	{
		myBot = GetComponentInParent<BotController>();
		if (myBot == null)
		{
			myBot = GetComponent<BotController>();
		}
	}

	public void AttackEvent()
	{
		myBot.DamageTarget();
	}
}
