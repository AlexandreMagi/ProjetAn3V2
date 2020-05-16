using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class UIParticuleSystem : MonoBehaviour
{
    [Title("Base Parameters")] // --------------------------------------------------
    [PropertyRange(0.001f,10), Tooltip("Durée d'émission constante du particle system")] public float duration = 1;
    [Tooltip("Dit si le particle system se relance tout seul lorsqu'il a fini")] public bool looping = false;
    [Tooltip("Dit si le particle system se lance au start")] public bool playOnStart = false;
    [Tooltip("Valeurs min et max de durée de vie d'une particule à l'init")] public Vector2 lifeTime = new Vector2(0.5f, 1f);
    [Tooltip("Valeurs min et max de vitesse d'une particule à l'init")] public Vector2 speed = new Vector2(20, 40);
    [Tooltip("Valeurs min et max de taille d'une particule à l'init")] public Vector2 size = new Vector2(0.5f, 1.5f);
    [Tooltip("Valeurs min et max de rotation d'une particule à l'init")] public Vector2 rotation = new Vector2(0, 360);
    float remainingDuration = 0; // Temps restant d'émission du particle system


    [Title("Pop Mode")] // --------------------------------------------------
    public enum PopMode { circle, squareEdge, square, line }
    [Tooltip("Façon de positioner les particules à l'init")] public PopMode popMode = PopMode.circle;
    [ShowIf("popMode", PopMode.circle), PropertyRange(0.001f,50f), Tooltip("Range de portée de pop (random entre zero et valeur)")] public float rangePop = 0;
    [ShowIf("popMode", PopMode.line), Tooltip("Dit si la ligne est horizontale ou verticale")] public bool horizontal = true;


    [Title("Pop Parameters")] // --------------------------------------------------
    [Tooltip("Taille du pull de particule")] public int maxParticle = 30;
    [Tooltip("Vitesse d'émission de particles")] public float rateOfParticle = 5;
    [Tooltip("Nombre de particules émises en burst à chaque play")] public int burstParticle = 0;
    float timerBeforeNextParticle = 0.1f; // Valeur de temps avant le prochain pop de particle en émission continue


    [Title("Dir Parameters")] // --------------------------------------------------
    public enum DirMode { goFrom, constant }
    [Tooltip("Direction appliquée aux particules à l'init")] public DirMode dirMod = DirMode.goFrom;
    [ShowIf("dirMod", DirMode.constant), Tooltip("Direction précise donnée aux particules (normalisé derriere)")] public Vector2 constantDir = new Vector2 (0f, 1f);
    [ShowIf("dirMod", DirMode.goFrom), Tooltip("Position de laquelle les particules s'écartent")] public Transform goFromPos = null;


    [Title("Other Parameters")] // --------------------------------------------------
    [Tooltip("Prefab de particule")] public GameObject particlePrefab = null;
    [Tooltip("Si remplis, sera le parent des particules")] public Transform overrideParent = null;
    [Tooltip("Si remplis, remplace le sprite des particules")] public Sprite sprite = null;


    [Title("Lifetime Parameters")] // --------------------------------------------------
    [Tooltip("Vitesse de la particule sur sa vie (multiplié par la vitesse à l'init)")] public AnimationCurve speedOverLifeTime = AnimationCurve.Linear(0,0,1,1);
    [Tooltip("Taille de la particule sur sa vie (multiplié par la taille à l'init)")] public AnimationCurve sizeOverLifeTime = AnimationCurve.Linear(0,0,1,1);
    [Tooltip("Couleur de la particule sur sa vie")] public Gradient colorOverLifeTime = null;

    [Tooltip("Active la rotation orbitale sur les particules")] public bool orbitalRotation = false;
    [ShowIf("orbitalRotation"), Tooltip("Point de pivot pour une rotation orbitale")] public Transform rotatePivot = null;
    [ShowIf("orbitalRotation"), Tooltip("Vitesses de rotation orbitale")] public Vector2 rotateSpeed = new Vector2(0f, 360f);
    [ShowIf("orbitalRotation"), Tooltip("Vitesses de rotation orbitale au cours de la vie d'une particule (multiplicateur)")] public AnimationCurve orbitalSpeedOverLifeTime = AnimationCurve.Linear(0, 0, 1, 1);

    [Tooltip("Permet d'avoir un outline sur les particules")] public bool enableOutline = false;
    [ShowIf("enableOutline"), Tooltip("Couleur de l'outline de la particule sur sa vie")] public Gradient outlineColorOverLifeTime = null;
    [ShowIf("enableOutline"), Tooltip("Taille de l'outline de la particule")] public float outlineSize = 1;
    [ShowIf("enableOutline"), Tooltip("Taille de l'outline de la particule sur sa vie")] public AnimationCurve outlineSizeOverLifeTime = AnimationCurve.Linear(0, 0, 1, 1);


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
    public void Stop()
    {
        for (int i = 0; i < allParticles.Count; i++)
        {
            allParticles[i].lifeTimeRemaining = 0;
            allParticles[i].actualParticle.gameObject.SetActive(false);
        }
    }

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
                if (dirMod == DirMode.goFrom && goFromPos != null) allParticles[i].goFrom = goFromPos;
                allParticles[i].actualParticle.rotation = Quaternion.Euler(0, 0, Random.Range(rotation.x, rotation.y));
                allParticles[i].size = Random.Range(size.x, size.y);
                allParticles[i].lifeTimeTotal = Random.Range(lifeTime.x, lifeTime.y);
                allParticles[i].lifeTimeRemaining = allParticles[i].lifeTimeTotal;
                allParticles[i].speed = Random.Range(speed.x, speed.y);
                allParticles[i].speedOverLifeTime = speedOverLifeTime;
                allParticles[i].sizeOverLifeTime = sizeOverLifeTime;
                allParticles[i].colorOverLifeTime = colorOverLifeTime;
                allParticles[i].orbitalRotation = orbitalRotation;
                allParticles[i].rotatePivot = rotatePivot;
                allParticles[i].rotateSpeed = Random.Range(rotateSpeed.x, rotateSpeed.y); ;
                allParticles[i].orbitalSpeedOverLifeTime = orbitalSpeedOverLifeTime;
                allParticles[i].outlineColorOverLifeTime = outlineColorOverLifeTime;
                allParticles[i].enableOutline = enableOutline;
                allParticles[i].outlineComponent.enabled = enableOutline;
                allParticles[i].outlineSize = outlineSize;
                allParticles[i].outlineSizeOverLifeTime = outlineSizeOverLifeTime;
                break;
            }
        }
    }

    void SetupInitPos(CustomParticle particle)
    {
        Vector3[] corners = new Vector3[4];
        rect.GetWorldCorners(corners);
        Vector2 newRectSize = (corners[2] - corners[0]);
        Vector2 randomBasePos, adaptedSize;
        switch (popMode)
        {
            // ---
            case PopMode.circle: 
                Vector2 addedPos = Random.insideUnitCircle * rangePop;
                particle.actualParticle.position = transform.position + new Vector3(addedPos.x, addedPos.y);
                break;

            // ---
            case PopMode.squareEdge:
                randomBasePos = Random.insideUnitCircle;
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
                adaptedSize = randomBasePos * newRectSize / 2 * rect.lossyScale;
                particle.actualParticle.position = transform.position + new Vector3(adaptedSize.x, adaptedSize.y);
                break;

            // ---
            case PopMode.square:
                randomBasePos = Vector2.zero;
                randomBasePos.x = Random.Range(0f, 1f) * Mathf.Sign(Random.Range(-1f, 1f));
                randomBasePos.y = Random.Range(0f, 1f) * Mathf.Sign(Random.Range(-1f, 1f));
                
                adaptedSize = randomBasePos * newRectSize / 2 * rect.lossyScale;
                particle.actualParticle.position = transform.position + new Vector3(adaptedSize.x, adaptedSize.y);
                break;

            // ---
            case PopMode.line:
                randomBasePos = Vector2.zero;
                float randomValue = Random.Range(0f, 1f) * Mathf.Sign(Random.Range(-1f, 1f));
                if (horizontal) randomBasePos.x = randomValue;
                else randomBasePos.y = randomValue;
                adaptedSize = randomBasePos * newRectSize / 2 * rect.lossyScale;
                particle.actualParticle.position = transform.position + new Vector3(adaptedSize.x, adaptedSize.y);
                break;
        }
    }

    void SetupDir(CustomParticle particle)
    {
        switch (dirMod)
        {
            // ---
            case DirMode.goFrom:
                if (goFromPos != null) particle.dirGoTo = (particle.actualParticle.position - goFromPos.position).normalized;
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
    public Transform goFrom = null;
    public bool orbitalRotation = false;
    public Transform rotatePivot = null;
    public float rotateSpeed = 90;
    public AnimationCurve orbitalSpeedOverLifeTime = AnimationCurve.Linear(0, 0, 1, 1);

    public bool enableOutline = false;
    public Outline outlineComponent = null;
    public Gradient outlineColorOverLifeTime = null;
    public float outlineSize = 1;
    public AnimationCurve outlineSizeOverLifeTime = AnimationCurve.Linear(0, 0, 1, 1);

    public void UpdateValues()
    {
        float lifeTimePurcentage = (lifeTimeTotal - lifeTimeRemaining) / lifeTimeTotal;
        actualParticle.localScale = Vector3.one * sizeOverLifeTime.Evaluate(lifeTimePurcentage) * size;
        actualParticle.Translate(dirGoTo * speedOverLifeTime.Evaluate(lifeTimePurcentage) * speed * Time.unscaledDeltaTime, Space.World);
        particleImage.color = colorOverLifeTime.Evaluate(lifeTimePurcentage);

        if (orbitalRotation && rotatePivot != null)
        {
            actualParticle.RotateAround(rotatePivot.position, Vector3.forward, rotateSpeed * Time.unscaledDeltaTime * orbitalSpeedOverLifeTime.Evaluate(lifeTimePurcentage));
            if (goFrom != null) dirGoTo = (actualParticle.position - goFrom.position).normalized;
        }

        if (outlineComponent != null && enableOutline)
        {
            outlineComponent.effectColor = outlineColorOverLifeTime.Evaluate(lifeTimePurcentage);
            outlineComponent.effectDistance = outlineSize * outlineSizeOverLifeTime.Evaluate(lifeTimePurcentage) * new Vector2 (1,-1);
        }
    }

    public CustomParticle(Transform _actualParticle)
    {
        actualParticle = _actualParticle;
        particleImage = actualParticle.GetComponent<Image>();
        outlineComponent = actualParticle.GetComponent<Outline>();
    }
}
