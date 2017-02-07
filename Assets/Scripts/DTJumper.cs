using UnityEngine;
using System.Collections;

public class DTJumper : MonoBehaviour {
    public Animator[] jumpers;
    public void JumperSignal(bool b)
    {
        for (int i = 0; i < jumpers.Length; i++)
        {
            jumpers[i].SetBool("onJump", b);
        }
    }
}
