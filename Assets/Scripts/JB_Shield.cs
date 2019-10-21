using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class JB_Shield : NetworkBehaviour
{
    public AudioClip shieldOnSound;

    private AudioSource myAudioSource;



    private void Awake()
    {
        myAudioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        CmdShieldOnSound();
    }

    [Command]
    void CmdShieldOnSound()
    {
        myAudioSource.clip = shieldOnSound;
        myAudioSource.Play();
        RpcShieldOn();
    }

    [ClientRpc]
    void RpcShieldOn()
    {
        myAudioSource.clip = shieldOnSound;
        myAudioSource.Play();
    }
}
