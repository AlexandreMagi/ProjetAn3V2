using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneShakeManager : MonoBehaviour
{

    [SerializeField] List<Rigidbody> allAffectedProps = null;
    [Header("Passive Shake")]
    [SerializeField] float minPassiveShake = 50;
    [SerializeField] float maxPassiveShake = 100;
    [SerializeField] Vector2 randomTimePassiveShake = new Vector2(0.1f, 0.2f);
    float remainingTimeBeforeLittleShake = 0.5f;

    [Header("Active Shake")]
    [SerializeField] float minActiveShake = 180;
    [SerializeField] float maxActiveShake = 220;
    [SerializeField] Vector2 randomTimeActiveShake = new Vector2(2f, 5f);
    float remainingTimeBeforeBigShake = 0.5f;

    [Header("Autres")]
    [SerializeField] float randomPurcentageOnEachObject = 0.2f;
    [SerializeField] float frequencyPerlinNoise = 0.1f;
    float customTimeForPerlin = 0;
    [SerializeField] float randomDirOnJumps = 0.1f;

    public static PlaneShakeManager Instance { get; private set; }
    void Awake()
    {
        Instance = this;
    }

    public bool activated = true;

    // Start is called before the first frame update
    void Start()
    {
        remainingTimeBeforeLittleShake = Random.Range(randomTimePassiveShake.x, randomTimePassiveShake.y);
        remainingTimeBeforeBigShake = Random.Range(randomTimeActiveShake.x, randomTimeActiveShake.y);
    }

    // Update is called once per frame
    void Update()
    {
        if (activated)
        {
            customTimeForPerlin += Time.deltaTime * frequencyPerlinNoise;
            if (remainingTimeBeforeLittleShake > 0)
            {
                remainingTimeBeforeLittleShake -= Time.deltaTime;
                if (remainingTimeBeforeLittleShake < 0)
                {
                    remainingTimeBeforeLittleShake = Random.Range(randomTimePassiveShake.x, randomTimePassiveShake.y);
                    float shakeForce = Mathf.Lerp(minPassiveShake, maxPassiveShake, GetPerlinFloat(18));
                    ShakeProps(shakeForce);
                }
            }

            if (remainingTimeBeforeBigShake > 0)
            {
                remainingTimeBeforeBigShake -= Time.deltaTime;
                if (remainingTimeBeforeBigShake < 0)
                {
                    remainingTimeBeforeBigShake = Random.Range(randomTimeActiveShake.x, randomTimeActiveShake.y);
                    float shakeForce = Mathf.Lerp(minActiveShake, maxActiveShake, GetPerlinFloat(10));
                    ShakeProps(shakeForce);
                    CameraHandler.Instance.PlaneShake(GetPerlinFloat(10));
                }
            }
        }
    }

    void ShakeProps(float force)
    {
        if (activated)
        {
            foreach (var propRb in allAffectedProps)
            {
                Vector3 dirVector = (Vector3.up + new Vector3(Random.Range(-randomDirOnJumps, randomDirOnJumps), Random.Range(-randomDirOnJumps, randomDirOnJumps), Random.Range(-randomDirOnJumps, randomDirOnJumps))).normalized;
                propRb.AddForce(dirVector * (force + force * Random.Range(0, randomPurcentageOnEachObject) * Mathf.Sign(Random.Range(-1f, 1f))), ForceMode.Impulse);
            }
        }
    }

    float GetPerlinFloat(float seed) { return Mathf.PerlinNoise(seed, customTimeForPerlin); }

}
