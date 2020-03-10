using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailManager : MonoBehaviour
{
    public static TrailManager Instance { get; private set; }
    void Awake()
    {
        // Singleton
        if (Instance != null)
        {
            Debug.LogWarning("There is already an instance of a Trail Manager");
            Destroy(this);
        }
        else Instance = this;
    }

    List<bulletTrailData> allBulletTrails = new List<bulletTrailData>();
    [SerializeField] GameObject bulletTrailPrefab = null;
    [SerializeField] int pullSize = 20; // Taille du pull initial de balle
    [SerializeField] float trailSpeed = 10; // Vitesse de déplacement du trail
    [SerializeField] float distanceToEndKill = 0.3f; // Distance sous laquelle les balles seront comptées comme arrivées
    [SerializeField] float timeBeforeBulletCanBeShootAgain = 1; // Temps avant qu'un trail ne puisse être réutilisé
    [SerializeField] float offsetPosInit = 0; // Temps avant qu'un trail ne puisse être réutilisé

    // Start is called before the first frame update
    void Start()
    {
        // Destruction automatique si le prefab de bullet trail n'a pas été assigné ou qu'il n'a pas de particle system
        if (bulletTrailPrefab == null)
        {
            Debug.LogWarning("No Bullet Prefab Assigned To Trail Manager");
            Instance = null;
            Destroy(this);
        }


        GameObject instanceTest = Instantiate(bulletTrailPrefab);
        if (instanceTest.GetComponent<ParticleSystem>() == null)
        {
            Debug.LogWarning("Bullet trail prefab hasn't particle system component, this script will autodestroy");
            Instance = null;
            Destroy(this);
        }
        Destroy(instanceTest);
        

        // --- Crée un pull de bullet trail
        for (int i = 0; i < pullSize; i++)
        {
            Transform currBulletTrail = Instantiate(bulletTrailPrefab).transform; // Création du bullet trail
            currBulletTrail.parent = transform;
            currBulletTrail.gameObject.SetActive(false); // Desactivation des instances
            bulletTrailData currData = new bulletTrailData(currBulletTrail);
            currData.bulletTrailFx.Pause();
            allBulletTrails.Add(currData); // Ajout à la liste
        }
    }

    /// <summary>
    /// Affiche un trail qui va de la position de départ à la position d'arrivé
    /// </summary>
    /// <param name="posInit"></param>
    /// <param name="posFinal"></param>
    public void RequestBulletTrail(Vector3 posInit, Vector3 posFinal)
    {
        bool enoughBulletTrail = false; // Permet de determiner si il y'a assez de bullet trail
        posInit += Vector3.up * offsetPosInit;
        for (int i = 0; i < allBulletTrails.Count; i++)
        {
            if (!allBulletTrails[i].traveling && allBulletTrails[i].timeBeforeCanBeShotAgain == 0)
            {
                allBulletTrails[i].bulletTrail.gameObject.SetActive(true);
                allBulletTrails[i].traveling = true;
                allBulletTrails[i].speed = trailSpeed;
                allBulletTrails[i].posInit = posInit;
                allBulletTrails[i].posFinal = posFinal;
                allBulletTrails[i].distanceEndKill = distanceToEndKill;
                allBulletTrails[i].timeBeforeCanBeShotAgain = timeBeforeBulletCanBeShootAgain;
                allBulletTrails[i].bulletTrail.transform.position = posInit;
                allBulletTrails[i].bulletTrail.transform.LookAt(posFinal, Vector3.up);
                allBulletTrails[i].bulletTrailFx.Play();
                enoughBulletTrail = true;
                break;
            }
        }
        if (!enoughBulletTrail) AddBulletTrail(posInit, posFinal);
    }

    /// <summary>
    /// Permet d'ajouter une bullet trail à la liste
    /// </summary>
    /// <param name="posInit"></param>
    /// <param name="posFinal"></param>
    void AddBulletTrail (Vector3 posInit, Vector3 posFinal)
    {
        Transform currBulletTrail = Instantiate(bulletTrailPrefab).transform; // Création du bullet trail
        currBulletTrail.gameObject.SetActive(false); // Desactivation de l'instance
        bulletTrailData newBulletTrail = new bulletTrailData(currBulletTrail);

        newBulletTrail.traveling = true;
        newBulletTrail.speed = trailSpeed;
        newBulletTrail.posInit = posInit;
        newBulletTrail.posFinal = posFinal;
        newBulletTrail.distanceEndKill = distanceToEndKill;
        newBulletTrail.timeBeforeCanBeShotAgain = timeBeforeBulletCanBeShootAgain;
        newBulletTrail.bulletTrail.transform.position = posInit;
        newBulletTrail.bulletTrail.transform.LookAt(posFinal, Vector3.up);
        newBulletTrail.bulletTrailFx.Play();

        allBulletTrails.Add(newBulletTrail);
    }

    void Update()
    {
        foreach (var bulletTrail in allBulletTrails)
        {
            if (bulletTrail.traveling)
            {
                bulletTrail.bulletTrail.position = Vector3.MoveTowards(bulletTrail.bulletTrail.position, bulletTrail.posFinal, Time.unscaledDeltaTime * bulletTrail.speed);
                if (Vector3.Distance(bulletTrail.bulletTrail.position, bulletTrail.posFinal) < bulletTrail.distanceEndKill) bulletTrail.traveling = false;
            }
            else if (bulletTrail.timeBeforeCanBeShotAgain > 0)
            {
                bulletTrail.timeBeforeCanBeShotAgain -= Time.unscaledDeltaTime;
                if (bulletTrail.timeBeforeCanBeShotAgain < 0)
                {
                    bulletTrail.timeBeforeCanBeShotAgain = 0;
                    bulletTrail.bulletTrail.gameObject.SetActive(false);
                    bulletTrail.bulletTrailFx.Pause();
                }
            }
        }
    }

}

public class bulletTrailData
{
    public Vector3 posInit = Vector3.zero;
    public Vector3 posFinal = Vector3.zero;
    public float speed = 0;
    public float distanceEndKill = 0;
    public bool traveling = false;
    public float timeBeforeCanBeShotAgain = 0;
    public Transform bulletTrail = null;
    public ParticleSystem bulletTrailFx = null;


    public bulletTrailData (Transform _bulletTrail)
    {
        bulletTrail = _bulletTrail;
        bulletTrailFx = bulletTrail.GetComponent<ParticleSystem>();
    }
}
