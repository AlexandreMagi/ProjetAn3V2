using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FxManager : MonoBehaviour
{
    private static FxManager _instance;
    public static FxManager Instance
    {
        get
        {
            return _instance;
        }
    }
    void Awake()
    {
        _instance = this;
    }

    [SerializeField]
    ParticleSystem[] particleTab = null;

    public void PlayFx (string name, Vector3 pos, Quaternion rot)
    {
        ParticleSystem fxInstantiated = FindFx(name);
        if (fxInstantiated == null)
            return;
        ParticleSystem clone = Instantiate(fxInstantiated, pos, rot);
        clone.Play();
    }

    public void PlayFx (string name, Vector3 pos, Quaternion rot, float size)
    {
        ParticleSystem fxInstantiated = FindFx(name);
        if (fxInstantiated == null)
            return;

        var main = fxInstantiated.main;
        main.startSize = size;
        ParticleSystem clone = Instantiate(fxInstantiated, pos, rot);
        clone.Play();
    }

    ParticleSystem FindFx (string name)
    {
        for (int i = 0; i < particleTab.Length; i++)
        {
            if (particleTab[i].name == name)
            {
                return particleTab[i];
            }
        }
        Debug.Log("ERROR - Fx named : '" + name + "' doesn't exist");
        return null;
    }


}
