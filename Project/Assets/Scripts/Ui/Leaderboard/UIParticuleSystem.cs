using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class UIParticuleSystem : MonoBehaviour
{
    [Title("Base Parameters")] // --------------------------------------------------
    [SerializeField, PropertyRange(0.001f,10), Tooltip("Durée d'émission constante du particle system")] float duration = 1;
    [SerializeField, Tooltip("Dit si le particle system se relance tout seul lorsqu'il a fini")] bool looping = false;
    [SerializeField, Tooltip("Dit si le particle system se lance au start")] bool playOnStart = false;
    [SerializeField, Tooltip("Valeurs min et max de durée de vie d'une particule à l'init")] Vector2 lifeTime = new Vector2(0.5f, 1f);
    [SerializeField, Tooltip("Valeurs min et max de vitesse d'une particule à l'init")] Vector2 speed = new Vector2(20, 40);
    [SerializeField, Tooltip("Valeurs min et max de taille d'une particule à l'init")] Vector2 size = new Vector2(0.5f, 1.5f);
    [SerializeField, Tooltip("Valeurs min et max de rotation d'une particule à l'init")] Vector2 rotation = new Vector2(0, 360);
    float remainingDuration = 0; // Temps restant d'émission du particle system


    [Title("Pop Mode")] // --------------------------------------------------
    enum PopMode { circle, squareEdge, line }
    [SerializeField, Tooltip("Façon de positioner les particules à l'init")] PopMode popMode = PopMode.circle;
    [ShowIf("popMode", PopMode.circle), SerializeField, PropertyRange(0.001f,50f), Tooltip("Range de portée de pop (random entre zero et valeur)")] float rangePop = 0;
    [ShowIf("popMode", PopMode.line), SerializeField, Tooltip("Dit si la ligne est horizontale ou verticale")] bool horizontal = true;


    [Title("Pop Parameters")] // --------------------------------------------------
    [SerializeField, Tooltip("Taille du pull de particule")] int maxParticle = 30;
    [SerializeField, Tooltip("Vitesse d'émission de particles")] float rateOfParticle = 5;
    [SerializeField, Tooltip("Nombre de particules émises en burst à chaque play")] int burstParticle = 0;
    float timerBeforeNextParticle = 0.1f; // Valeur de temps avant le prochain pop de particle en émission continue


    [Title("Dir Parameters")] // --------------------------------------------------
    enum DirMode { goFrom, constant }
    [SerializeField, Tooltip("Direction appliquée aux particules à l'init")] DirMode dirMod = DirMode.goFrom;
    [SerializeField, ShowIf("dirMod", DirMode.constant), Tooltip("Direction précise donnée aux particules (normalisé derriere)")] Vector2 constantDir = new Vector2 (0f, 1f);
    [SerializeField, ShowIf("dirMod", DirMode.goFrom), Tooltip("Position de laquelle les particules s'écartent")] Transform goFromPos = null;


    [Title("Other Parameters")] // --------------------------------------------------
    [SerializeField, Tooltip("Prefab de particule")] GameObject particlePrefab = null;
    [SerializeField, Tooltip("Si remplis, sera le parent des particules")] Transform overrideParent = null;
    [SerializeField, Tooltip("Si remplis, remplace le sprite des particules")] Sprite sprite = null;


    [Title("Lifetime Parameters")] // --------------------------------------------------
    [SerializeField, Tooltip("Vitesse de la particule sur sa vie (multiplié par la vitesse à l'init)")] AnimationCurve speedOverLifeTime = AnimationCurve.Linear(0,0,1,1);
    [SerializeField, Tooltip("Taille de la particule sur sa vie (multiplié par la taille à l'init)")] AnimationCurve sizeOverLifeTime = AnimationCurve.Linear(0,0,1,1);
    [SerializeField, Tooltip("Couleur de la particule sur sa vie")] Gradient colorOverLifeTime = null;


    List<CustomParticle> allParticles = new List<CustomParticle>(); // Stock toute les particules du pull
    RectTransform rect = null; // Stock le transform de l'émitter
    
    void Start()
    {
        rect = GetComponent<RectTransform>();
        for (int i = 0; i < maxParticle; i++)
        {
            Transform newParticle = Instantiate(particlePrefab, overrideParent != null ? overrideParent : transform).transform;
            allParticles.Add(new CustomParticle(newParticle));
            newParticle.gameObject.SetActive(false);
        }
        if (playOnStart) Play();
    }

    #region Global Functions

    public void Play()
    {
        remainingDuration += duration;
        remainingDuration = Mathf.Clamp(remainingDuration, 0, duration);
        for (int i = 0; i < burstParticle; i++) {  NewParticle(); }
        Resume();
    }

    public void Resume() { timerBeforeNextParticle = 1 / rateOfParticle; }
    public void Pause() { timerBeforeNextParticle = 0; }

    #endregion

    #region Update Functons

    void Update()
    {
        if (rateOfParticle != 0) HandleConstantEmmission();
        if (allParticles.Count != 0) HandleCurrentParticles();
        if (duration != 0 && remainingDuration != 0) HandlePSDuration();
    }

    void HandleConstantEmmission()
    {
        if (timerBeforeNextParticle > 0 && remainingDuration > 0)
        {
            timerBeforeNextParticle -= Time.unscaledDeltaTime;
            if (timerBeforeNextParticle < 0)
            {
                float _saveValue = timerBeforeNextParticle;
                for (timerBeforeNextParticle = _saveValue; timerBeforeNextParticle < 0; timerBeforeNextParticle += 1 / rateOfParticle)
                {
                    NewParticle();
                }
            }
        }
    }

    void HandleCurrentParticles()
    {
        for (int i = 0; i < maxParticle; i++)
        {
            if (allParticles[i].lifeTimeRemaining > 0)
            {
                allParticles[i].lifeTimeRemaining -= Time.unscaledDeltaTime;
                allParticles[i].UpdateValues();
                if (allParticles[i].lifeTimeRemaining < 0)
                {
                    allParticles[i].lifeTimeRemaining = 0;
                    allParticles[i].actualParticle.gameObject.SetActive(false);
                }
            }
        }
    }

    void HandlePSDuration()
    {
        if (remainingDuration > 0)
        {
            remainingDuration -= Time.unscaledDeltaTime;
            if (remainingDuration < 0)
            {
                if (looping) Play();
                else Pause();
            }
        }
    }

    #endregion

    #region Particle Pop Functions

    public void NewParticle()
    {
        for (int i = 0; i < maxParticle; i++)
        {
            if (allParticles[i].lifeTimeRemaining == 0 && allParticles[i].actualParticle != null) 
            {
                allParticles[i].actualParticle.gameObject.SetActive(true);

                SetupInitPos(allParticles[i]);
                SetupDir(allParticles[i]);

                if (sprite != null) allParticles[i].particleImage.sprite = sprite;
                allParticles[i].actualParticle.rotation = Quaternion.Euler(0, 0, Random.Range(rotation.x, rotation.y));
                allParticles[i].size = Random.Range(size.x, size.y);
                allParticles[i].lifeTimeTotal = Random.Range(lifeTime.x, lifeTime.y);
                allParticles[i].lifeTimeRemaining = allParticles[i].lifeTimeTotal;
                allParticles[i].speed = Random.Range(speed.x, speed.y);
                allParticles[i].speedOverLifeTime = speedOverLifeTime;
                allParticles[i].sizeOverLifeTime = sizeOverLifeTime;
                allParticles[i].colorOverLifeTime = colorOverLifeTime;
                break;
            }
        }
    }

    void SetupInitPos(CustomParticle particle)
    {
        Vector3[] corners = new Vector3[4];
        rect.GetWorldCorners(corners);
        Vector2 newRectSize = (corners[2] - corners[0]);
        switch (popMode)
        {
            // ---
            case PopMode.circle: 
                Vector2 addedPos = Random.insideUnitCircle * rangePop;
                particle.actualParticle.position = transform.position + new Vector3(addedPos.x, addedPos.y);
                break;

            // ---
            case PopMode.squareEdge:
                Vector2 randomBasePos = Random.insideUnitCircle;
                if (Mathf.Abs(randomBasePos.y) > Mathf.Abs(randomBasePos.x))
                {
                    randomBasePos.y = 1 * Mathf.Sign(randomBasePos.y);
                    randomBasePos.x = Random.Range(0f, 1f) * Mathf.Sign(Random.Range(-1f, 1f));
                }
                else
                {
                    randomBasePos.x = 1 * Mathf.Sign(randomBasePos.x);
                    randomBasePos.y = Random.Range(0f, 1f) * Mathf.Sign(Random.Range(-1f, 1f));
                }
                Vector2 adaptedSize = randomBasePos * newRectSize / 2 * rect.lossyScale;
                particle.actualParticle.position = transform.position + new Vector3(adaptedSize.x, adaptedSize.y);
                break;

            // ---
            case PopMode.line:
                Vector2 randomPos = Vector2.zero;
                float randomValue = Random.Range(0f, 1f) * Mathf.Sign(Random.Range(-1f, 1f));
                if (horizontal) randomPos.x = randomValue;
                else randomPos.y = randomValue;
                Vector2 adaptedToRectSize = randomPos * newRectSize / 2 * rect.lossyScale;
                particle.actualParticle.position = transform.position + new Vector3(adaptedToRectSize.x, adaptedToRectSize.y);
                break;
        }
    }

    void SetupDir(CustomParticle particle)
    {
        switch (dirMod)
        {
            // ---
            case DirMode.goFrom:
                particle.dirGoTo = (particle.actualParticle.transform.position - goFromPos.transform.position).normalized;
                break;

            // ---
            case DirMode.constant:
                particle.dirGoTo = (constantDir).normalized;
                break;
        }
    }

    #endregion

}



public class CustomParticle
{
    public float lifeTimeTotal = 0;
    public float lifeTimeRemaining = 0;
    public Transform actualParticle;
    public Image particleImage;
    public Vector2 dirGoTo = Vector2.zero;
    public float speed = 0;
    public AnimationCurve speedOverLifeTime = AnimationCurve.Linear(0, 0, 1, 1);
    public AnimationCurve sizeOverLifeTime = AnimationCurve.Linear(0, 0, 1, 1);
    public float size = 1;
    public Gradient colorOverLifeTime = null;

    public void UpdateValues()
    {
        float lifeTimePurcentage = (lifeTimeTotal - lifeTimeRemaining) / lifeTimeTotal;
        actualParticle.localScale = Vector3.one * sizeOverLifeTime.Evaluate(lifeTimePurcentage) * size;
        actualParticle.Translate(dirGoTo * speedOverLifeTime.Evaluate(lifeTimePurcentage) * speed, Space.World);
        particleImage.color = colorOverLifeTime.Evaluate(lifeTimePurcentage);
    }

    public CustomParticle(Transform _actualParticle)
    {
        actualParticle = _actualParticle;
        particleImage = actualParticle.GetComponent<Image>();
    }
}
