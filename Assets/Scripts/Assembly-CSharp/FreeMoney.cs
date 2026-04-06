using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeMoney : MonoBehaviour
{
    public void Get(int mod)
    {
        LocalStore.GiveMoney(mod);
    }
}
