using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DeadBodyPartManager : MonoBehaviour
{
    public static DeadBodyPartManager Instance { get; private set; }
    void Awake() { Instance = this; }

    public enum TypeOfFracture { Swarmer,Box,Barrel, Shooter, none};

    [SerializeField] GameObject SwarmerPrefab = null;
    [SerializeField] int nbSwarmerPrefab = 10;
    [SerializeField] GameObject BoxPrefab = null;
    [SerializeField] int nbBoxPrefab = 10;
    [SerializeField] GameObject BarrelPrefab = null;
    [SerializeField] int nbBarrelPrefab = 10;
    [SerializeField] GameObject ShooterPrefab = null;
    [SerializeField] int nbShooterPrefab = 10;

    List<FractureManager> SwarmerManagers = new List<FractureManager>();
    List<FractureManager> BoxManagers = new List<FractureManager>();
    List<FractureManager> BarrelManagers = new List<FractureManager>();
    List<FractureManager> ShooterManagers = new List<FractureManager>();

    void Start()
    {
        for (int i = 0; i < nbSwarmerPrefab; i++)  
        {
            FractureManager instance = Instantiate(SwarmerPrefab).GetComponent<FractureManager>();
            instance.DepopAll();
            instance.gameObject.SetActive(false);
            SwarmerManagers.Add(instance); 
        }
        for (int i = 0; i < nbBoxPrefab; i++)  
        {
            FractureManager instance = Instantiate(BoxPrefab).GetComponent<FractureManager>();
            instance.DepopAll();
            instance.gameObject.SetActive(false);
            BoxManagers.Add(instance); 
        }
        for (int i = 0; i < nbBarrelPrefab; i++)  
        {
            FractureManager instance = Instantiate(BarrelPrefab).GetComponent<FractureManager>();
            instance.DepopAll();
            instance.gameObject.SetActive(false);
            BarrelManagers.Add(instance); 
        }
        for (int i = 0; i < nbShooterPrefab; i++)  
        {
            FractureManager instance = Instantiate(ShooterPrefab).GetComponent<FractureManager>();
            instance.DepopAll();
            instance.gameObject.SetActive(false);
            ShooterManagers.Add(instance); 
        }
    }

    public void RequestPop (TypeOfFracture fractureType, Vector3 pos, Vector3 decalAllPos)
    {
        FractureManager usedFractureManager = FindFractureManagerToUse(fractureType);
        if (usedFractureManager != null)
        {
            usedFractureManager.DepopAll();
            usedFractureManager.gameObject.SetActive(true);
            usedFractureManager.transform.position = pos;
            usedFractureManager.SpawnBodyParts(pos, decalAllPos);
        }
    }

    FractureManager FindFractureManagerToUse(TypeOfFracture fractureType)
    {
        List<FractureManager> ListUsed = null;
        switch (fractureType)
        {
            case TypeOfFracture.Swarmer:
                ListUsed = SwarmerManagers;
                break;
            case TypeOfFracture.Box:
                ListUsed = BoxManagers;
                break;
            case TypeOfFracture.Barrel:
                ListUsed = BarrelManagers;
                break;
            case TypeOfFracture.Shooter:
                ListUsed = ShooterManagers;
                break;
            case TypeOfFracture.none:
                return null;
        }
        for (int i = 0; i < ListUsed.Count; i++) { if (ListUsed[i].available) return ListUsed[i]; }

        float savedTimer = 0;
        int indexSaved = -1;
        for (int i = 0; i < ListUsed.Count; i++)
        {
            if (ListUsed[i].timeRemainingBeforeActivation < savedTimer || indexSaved == -1)
            {
                savedTimer = ListUsed[i].timeRemainingBeforeActivation;
                indexSaved = i;
            }
        }
        if (indexSaved != -1)
        {
            return ListUsed[indexSaved];
        }


        return null;
    }


}
