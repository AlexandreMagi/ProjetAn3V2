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

    public ParticleSystem PlayFx (string name)
    {
        ParticleSystem fxInstantiated = FindFx(name);
        if (fxInstantiated == null)
            return null;
        ParticleSystem clone = Instantiate(fxInstantiated);
        clone.transform.position = fxInstantiated.transform.position;
        clone.transform.rotation = fxInstantiated.transform.rotation;
        clone.Play();
        return clone;   
    }

    public ParticleSystem PlayFx (string name, Vector3 pos, Quaternion rot)
    {
        ParticleSystem fxInstantiated = FindFx(name);
        if (fxInstantiated == null)
            return null;
        ParticleSystem clone = Instantiate(fxInstantiated, pos, rot);
        clone.Play();
        return clone;
    }

    public ParticleSystem PlayFx(string name, Vector3 pos, RaycastHit hitFromCast, bool addRotation = false)
    {
        ParticleSystem fxInstantiated = FindFx(name);
        if (fxInstantiated == null)
            return null;

        ParticleSystem clone = Instantiate(fxInstantiated, pos, Quaternion.identity);

        clone.transform.LookAt(pos + hitFromCast.normal * 3, Vector3.up);
        if (addRotation)
            clone.transform.Rotate(90,0,0,Space.Self);

        clone.Play();
        return clone;
    }

    public ParticleSystem PlayFx(string name, RaycastHit hitFromCast, Ray rayRef)
    {
        ParticleSystem fxInstantiated = FindFx(name);
        if (fxInstantiated == null)
            return null;

        ParticleSystem clone = Instantiate(fxInstantiated, hitFromCast.point, Quaternion.identity);

        Vector3 originDir = rayRef.direction;
        Vector3 reflectedDir = Vector3.Reflect(originDir, hitFromCast.normal);

        clone.transform.rotation = Quaternion.LookRotation(reflectedDir, Vector3.up);

        clone.Play();
        return clone;
    }

    public ParticleSystem PlayFx(string name, Vector3 hitPoint, Vector3 originalDir, Vector3 normalRef)
    {
        ParticleSystem fxInstantiated = FindFx(name);
        if (fxInstantiated == null)
            return null;

        ParticleSystem clone = Instantiate(fxInstantiated, hitPoint, Quaternion.identity);

        Vector3 reflectedDir = Vector3.Reflect(originalDir, normalRef);

        clone.transform.rotation = Quaternion.LookRotation(reflectedDir, Vector3.up);

        clone.Play();
        return clone;
    }

    public ParticleSystem PlayFx (string name, Vector3 pos, Quaternion rot, float size)
    {
        ParticleSystem fxInstantiated = FindFx(name);
        if (fxInstantiated == null)
            return null;

        var main = fxInstantiated.main;
        main.startSize = size;
        ParticleSystem clone = Instantiate(fxInstantiated, pos, rot);
        clone.Play();
        return clone;
    }

    public ParticleSystem PlayFx (string name, Vector3 pos, Quaternion rot, float sizeMultiplier, float baseScale = 1)
    {
        ParticleSystem fxInstantiated = FindFx(name);
        if (fxInstantiated == null)
            return null;

        ParticleSystem clone = Instantiate(fxInstantiated, pos, rot);
        for (int i = 0; i < clone.transform.childCount; i++)
        {
            clone.transform.GetChild(i).localScale = Vector3.one * sizeMultiplier * baseScale;
        }
        clone.Play();
        return clone;
    }

    public ParticleSystem PlayFx (string name, Transform parent, float lifeTime)
    {
        ParticleSystem fxInstantiated = FindFx(name);
        if (fxInstantiated == null)
            return null;

        var main = fxInstantiated.main;
        main.startLifetime = lifeTime;
        ParticleSystem clone = Instantiate(fxInstantiated, parent);
        clone.Play();
        return clone;
    }

    public ParticleSystem PlayFx(string name, Transform parent, float sizeMultiplier, float baseScale = 1)
    {
        ParticleSystem fxInstantiated = FindFx(name);
        if (fxInstantiated == null)
            return null;

        ParticleSystem clone = Instantiate(fxInstantiated, parent);
        for (int i = 0; i < clone.transform.childCount; i++)
        {
            clone.transform.GetChild(i).localScale = Vector3.one * sizeMultiplier * baseScale;
        }
        clone.Play();
        return clone;
    }

    public ParticleSystem PlayFx (string name, Transform parent)
    {
        ParticleSystem fxInstantiated = FindFx(name);
        if (fxInstantiated == null)
            return null;

        ParticleSystem clone = Instantiate(fxInstantiated, parent);
        clone.Play();
        return clone;
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
