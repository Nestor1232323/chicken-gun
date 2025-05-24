using UnityEngine;

public class PlayGUI : MonoBehaviour
{
	public Transform[] transforms;

	public GUIContent[] GUIContents;

	private Animator[] animator;

	private string currentState = string.Empty;

	private void Start()
	{
		animator = new Animator[transforms.Length];
		for (int i = 0; i < transforms.Length; i++)
		{
			animator[i] = transforms[i].GetComponent<Animator>();
		}
	}

	private void OnGUI()
	{
		GUILayout.BeginVertical("box");
		for (int i = 0; i < GUIContents.Length; i++)
		{
			if (GUILayout.Button(GUIContents[i]))
			{
				currentState = GUIContents[i].text;
			}
			AnimatorStateInfo currentAnimatorStateInfo = animator[0].GetCurrentAnimatorStateInfo(0);
			if (!currentAnimatorStateInfo.IsName("Base Layer.idle2"))
			{
				for (int j = 0; j < animator.Length; j++)
				{
					animator[j].SetBool("idle2ToIdle0", value: false);
					animator[j].SetBool("idle2ToIdle1", value: false);
					animator[j].SetBool("idle2ToWalk", value: false);
					animator[j].SetBool("idle2ToRun", value: false);
					animator[j].SetBool("idle2ToWound", value: false);
					animator[j].SetBool("idle2ToSkill1", value: false);
					animator[j].SetBool("idle2ToSkill0", value: false);
					animator[j].SetBool("idle2ToAttack1", value: false);
					animator[j].SetBool("idle2ToAttack0", value: false);
					animator[j].SetBool("idle2ToDeath", value: false);
				}
			}
			else
			{
				for (int k = 0; k < animator.Length; k++)
				{
					animator[k].SetBool("walkToIdle2", value: false);
					animator[k].SetBool("runToIdle2", value: false);
					animator[k].SetBool("deathToIdle2", value: false);
				}
			}
			if (!(currentState != string.Empty))
			{
				continue;
			}
			if (currentAnimatorStateInfo.IsName("Base Layer.walk") && currentState != "walk")
			{
				for (int l = 0; l < animator.Length; l++)
				{
					animator[l].SetBool("walkToIdle2", value: true);
				}
			}
			if (currentAnimatorStateInfo.IsName("Base Layer.run") && currentState != "run")
			{
				for (int m = 0; m < animator.Length; m++)
				{
					animator[m].SetBool("runToIdle2", value: true);
				}
			}
			if (currentAnimatorStateInfo.IsName("Base Layer.death") && currentState != "death")
			{
				for (int n = 0; n < animator.Length; n++)
				{
					animator[n].SetBool("deathToIdle2", value: true);
				}
			}
			switch (currentState)
			{
			case "idle0":
			{
				for (int num4 = 0; num4 < animator.Length; num4++)
				{
					animator[num4].SetBool("idle2ToIdle0", value: true);
				}
				break;
			}
			case "idle1":
			{
				for (int num8 = 0; num8 < animator.Length; num8++)
				{
					animator[num8].SetBool("idle2ToIdle1", value: true);
				}
				break;
			}
			case "walk":
			{
				for (int num10 = 0; num10 < animator.Length; num10++)
				{
					animator[num10].SetBool("idle2ToWalk", value: true);
				}
				break;
			}
			case "run":
			{
				for (int num6 = 0; num6 < animator.Length; num6++)
				{
					animator[num6].SetBool("idle2ToRun", value: true);
				}
				break;
			}
			case "attack0":
			{
				for (int num2 = 0; num2 < animator.Length; num2++)
				{
					animator[num2].SetBool("idle2ToAttack0", value: true);
				}
				break;
			}
			case "attack1":
			{
				for (int num9 = 0; num9 < animator.Length; num9++)
				{
					animator[num9].SetBool("idle2ToAttack1", value: true);
				}
				break;
			}
			case "skill0":
			{
				for (int num7 = 0; num7 < animator.Length; num7++)
				{
					animator[num7].SetBool("idle2ToSkill0", value: true);
				}
				break;
			}
			case "skill1":
			{
				for (int num5 = 0; num5 < animator.Length; num5++)
				{
					animator[num5].SetBool("idle2ToSkill1", value: true);
				}
				break;
			}
			case "wound":
			{
				for (int num3 = 0; num3 < animator.Length; num3++)
				{
					animator[num3].SetBool("idle2ToWound", value: true);
				}
				break;
			}
			case "death":
			{
				for (int num = 0; num < animator.Length; num++)
				{
					animator[num].SetBool("idle2ToDeath", value: true);
				}
				break;
			}
			}
			currentState = string.Empty;
		}
		GUILayout.EndVertical();
	}
}
