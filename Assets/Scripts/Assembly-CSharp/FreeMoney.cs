using UnityEngine;

public class FreeMoney : MonoBehaviour
{
	public void Get(int mod)
	{
		LocalStore.GiveMoney(mod);
	}
}
