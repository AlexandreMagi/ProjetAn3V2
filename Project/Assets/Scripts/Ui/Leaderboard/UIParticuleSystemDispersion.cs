using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class UIParticuleSystemDispersion : MonoBehaviour
{

    enum PopMode { circle, squareEdge }
    [SerializeField] PopMode popMode = PopMode.circle;
    [ShowIf("popMode", PopMode.circle), SerializeField] float rangePop = 0;

    [SerializeField] Vector2 lifeTime = new Vector2 (0.5f, 1f);

    [SerializeField] float nbParticule = 10;
    [SerializeField] Vector2 speed = new Vector2(20, 40);
    [SerializeField] Vector2 size = new Vector2(0.5f, 1.5f);
    [SerializeField] Vector2 rotation = new Vector2(0, 360);
    [SerializeField] GameObject particlePrefab = null;
    [SerializeField] AnimationCurve speedOverLifeTime = AnimationCurve.Linear(0,0,1,1);
    [SerializeField] AnimationCurve sizeOverLifeTime = AnimationCurve.Linear(0,0,1,1);
    [SerializeField] Gradient colorOverLifeTime = null;

    [SerializeField] int maxParticle = 30;

    List<CustomParticle> allParticles = new List<CustomParticle>();
    RectTransform rect = null;

    void Start()
    {
        rect = GetComponent<RectTransform>();
        for (int i = 0; i < maxParticle; i++)
        {
            Transform newParticle = Instantiate(particlePrefab, transform).transform;
            allParticles.Add(new CustomParticle(newParticle));
            newParticle.gameObject.SetActive(false);
        }
    }

    void Update()
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

    public void Play(Vector2 posInit)
    {
        int usedParticle = 0;
        for (int i = 0; i < maxParticle; i++)
        {
            if (allParticles[i].lifeTimeRemaining == 0)
            {
                allParticles[i].actualParticle.gameObject.SetActive(true);

                switch (popMode)
                {
                    case PopMode.circle:
                        Vector2 addedPos = Random.insideUnitCircle * rangePop;
                        allParticles[i].actualParticle.position = transform.position + new Vector3(addedPos.x, addedPos.y);
                        break;
                    case PopMode.squareEdge:
                        Vector2 randomBasePos = Random.insideUnitCircle;
                        if (Mathf.Abs(randomBasePos.y) > Mathf.Abs(randomBasePos.x))
                        {
                            randomBasePos.y = 1 * Mathf.Sign (randomBasePos.y);
                            randomBasePos.x = Random.Range(0f, 1f) * Mathf.Sign(Random.Range(-1f, 1f));
                        }
                        else
                        {
                            randomBasePos.x = 1 * Mathf.Sign(randomBasePos.x);
                            randomBasePos.y = Random.Range(0f, 1f) * Mathf.Sign(Random.Range(-1f, 1f));
                        }
                        Vector2 adaptedSize = randomBasePos * rect.sizeDelta / 2;
                        allParticles[i].actualParticle.position = transform.position + new Vector3(adaptedSize.x, adaptedSize.y);
                        break;
                }

                allParticles[i].actualParticle.rotation = Quaternion.Euler(0, 0, Random.Range(rotation.x, rotation.y));
                allParticles[i].size = Random.Range(size.x, size.y);
                allParticles[i].lifeTimeTotal = Random.Range(lifeTime.x, lifeTime.y);
                allParticles[i].lifeTimeRemaining = allParticles[i].lifeTimeTotal;
                allParticles[i].speed = Random.Range(speed.x, speed.y);
                allParticles[i].dirGoTo = (new Vector2(allParticles[i].actualParticle.position.x, allParticles[i].actualParticle.position.y) - posInit).normalized;
                allParticles[i].speedOverLifeTime = speedOverLifeTime;
                allParticles[i].sizeOverLifeTime = sizeOverLifeTime;
                allParticles[i].colorOverLifeTime = colorOverLifeTime;

                usedParticle++;
            }
            if (usedParticle == nbParticule) break;
        }
    }

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

    public CustomParticle (Transform _actualParticle)
    {
        actualParticle = _actualParticle;
        particleImage = actualParticle.GetComponent<Image>();
    }
}
