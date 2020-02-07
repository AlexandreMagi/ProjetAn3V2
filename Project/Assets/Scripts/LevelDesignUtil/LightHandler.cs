using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class LightHandler : MonoBehaviour
{
    Light objLight = null;
    float baseIntensity = 0;
    [SerializeField] bool timeScaleIndependent = false;

    [SerializeField] bool blink = false;
    [SerializeField, ShowIf("blink")] Vector2 offRandomTime = new Vector2(0, 1);
    [SerializeField, ShowIf("blink")] Vector2 onRandomTime = new Vector2(0, 1);
    float blinkTimer = 0;
    float currentTimerToNextState = 0;
    // on pourrait faire un changement de couleur à chaque blink si necessaire

    [SerializeField] bool oscillate = false;
    [SerializeField, ShowIf("oscillate")] float frequencyIntensity = 1;
    [SerializeField, ShowIf("oscillate")] float minValueIntensityRelative = -1;
    [SerializeField, ShowIf("oscillate")] float maxValueIntensityRelative = 1;

    [SerializeField] bool changeColor = false;
    [SerializeField, ShowIf("changeColor")] bool fluidified = false;
    Color aimedColor = Color.white;
    [SerializeField, ShowIf("fluidified")] bool lerped = false;
    [SerializeField, ShowIf("fluidified")] float speedFluidify = 3;
    enum modOfColor { RandomBetweenColor, OscillateBetweenTwoColor, Gradient };
    [SerializeField, ShowIf("changeColor")] modOfColor currentMod = 0;
    float colorTimer = 0;
    float currentTimerToNextColor = 0;

    [SerializeField, ShowIf("currentMod", modOfColor.RandomBetweenColor), ShowIf("changeColor")] Vector2 changeEvery = new Vector2(0, 1);
    [SerializeField, ShowIf("currentMod", modOfColor.RandomBetweenColor), ShowIf("changeColor")] Color colorRandOne = Color.white;
    [SerializeField, ShowIf("currentMod", modOfColor.RandomBetweenColor), ShowIf("changeColor")] Color colorRandTwo = Color.white;

    [SerializeField, ShowIf("currentMod", modOfColor.OscillateBetweenTwoColor), ShowIf("changeColor")] float frequencyColor = 1;
    [SerializeField, ShowIf("currentMod", modOfColor.OscillateBetweenTwoColor), ShowIf("changeColor")] Color colorOsciOne = Color.white;
    [SerializeField, ShowIf("currentMod", modOfColor.OscillateBetweenTwoColor), ShowIf("changeColor")] Color colorOsciTwo = Color.white;

    [SerializeField, ShowIf("currentMod", modOfColor.Gradient), ShowIf("changeColor")] Gradient colorGradient = null;
    [SerializeField, ShowIf("currentMod", modOfColor.Gradient), ShowIf("changeColor")] float gradientTime = 1;
    [SerializeField, ShowIf("currentMod", modOfColor.Gradient), ShowIf("changeColor")] float decalTime = 0;


    [SerializeField] bool flickerIntensity = false;
    [SerializeField, ShowIf("flickerIntensity")] Vector2 flickerRandomIntensityMultiplier = new Vector2(1.0f, 1.2f);
    [SerializeField, ShowIf("flickerIntensity")] float intensityFlickDuration = 0.1f;
    float currentIntensityMultiplier = 1.0f;
    float targetIntensityMultiplier;
    float timeLeftIntensity;

    [SerializeField] bool flickerPosition = false;
    [SerializeField, ShowIf("flickerPosition")] float moveRange = 0.1f;
    [SerializeField, ShowIf("flickerPosition")] float moveFlickDuration = 0.1f;
    Vector3 initPos;
    Vector3 currentPos;
    Vector3 targetPos;
    float timeLeftPos;

    Light lightComponent;

    float elapsedTimeForCheckVisi = 0;
    [SerializeField]
    float optimisationTimeCheckVisi = 1f;

    bool isSeen = true;

    // Start is called before the first frame update
    void Start()
    {
        objLight = GetComponent<Light>();
        baseIntensity = objLight.intensity;
        if (currentMod == modOfColor.Gradient)
        {
            colorTimer = decalTime;
        }
        initPos = transform.localPosition;// Save de la position de base
        currentPos = initPos; // Setup de la var de position à celle de base
        lightComponent = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTimeForCheckVisi += Time.unscaledDeltaTime;
        if(elapsedTimeForCheckVisi >= optimisationTimeCheckVisi)
        {
            elapsedTimeForCheckVisi = 0;
            isSeen = InSight();
        }

        if (isSeen)
        {
            lightComponent.enabled = true;

            float dt = timeScaleIndependent ? Time.unscaledDeltaTime : Time.deltaTime;
            float time = timeScaleIndependent ? Time.unscaledTime : Time.time;
            if (blink)
            {
                blinkTimer += dt;
                if (blinkTimer > currentTimerToNextState)
                {
                    // Ici pour play Fx
                    blinkTimer -= currentTimerToNextState;
                    if (objLight.enabled)
                    {
                        currentTimerToNextState = Random.Range(offRandomTime.x, offRandomTime.y);
                        objLight.enabled = false;
                    }
                    else
                    {
                        currentTimerToNextState = Random.Range(onRandomTime.x, onRandomTime.y);
                        objLight.enabled = true;
                    }
                }
            }
            if (oscillate)
            {
                objLight.intensity = baseIntensity + Mathf.Lerp(minValueIntensityRelative, maxValueIntensityRelative, (Mathf.Sin(time * frequencyIntensity) + 1) / 2);
            }
            if (flickerIntensity)
            {
                if (timeLeftIntensity < dt)
                {
                    targetIntensityMultiplier = Random.Range(flickerRandomIntensityMultiplier.x, flickerRandomIntensityMultiplier.y); //calcul d'une nouvelle intensité
                    timeLeftIntensity = intensityFlickDuration;
                }
                else
                {
                    float valueLerp = dt / timeLeftIntensity; // "poids" du changement en fonction du dt, on divise par le temps restant pour que le deplacement soit fluide
                    currentIntensityMultiplier = Mathf.Lerp(currentIntensityMultiplier, targetIntensityMultiplier, valueLerp); // lerp de la valeur actuelle vers la valeur visée
                    if (oscillate) objLight.intensity = objLight.intensity * currentIntensityMultiplier;
                    else objLight.intensity = baseIntensity * currentIntensityMultiplier;
                    timeLeftIntensity -= dt;
                }
            }
            if (changeColor)
            {
                if (currentMod == modOfColor.RandomBetweenColor)
                {
                    colorTimer += dt;
                    if (colorTimer > currentTimerToNextColor)
                    {
                        colorTimer -= currentTimerToNextColor;
                        currentTimerToNextColor = Random.Range(changeEvery.x, changeEvery.y);
                        aimedColor = new Color(
                            Random.Range(colorRandOne.r, colorRandTwo.r),
                            Random.Range(colorRandOne.g, colorRandTwo.g),
                            Random.Range(colorRandOne.b, colorRandTwo.b),
                            Random.Range(colorRandOne.a, colorRandTwo.a));
                    }
                }
                else if (currentMod == modOfColor.OscillateBetweenTwoColor)
                {
                    aimedColor = Color.Lerp(colorOsciOne, colorOsciTwo, (Mathf.Sin(time * frequencyColor) + 1) / 2);
                }
                else if (currentMod == modOfColor.Gradient)
                {
                    colorTimer += dt / gradientTime;
                    if (colorTimer > 1)
                        colorTimer -= 1;
                    aimedColor = colorGradient.Evaluate(colorTimer);
                }


                if (fluidified)
                {
                    if (lerped)
                    {
                        objLight.color = Color.Lerp(objLight.color, aimedColor, dt * speedFluidify);
                    }
                    else
                    {
                        objLight.color = new Color(
                                Mathf.MoveTowards(objLight.color.r, aimedColor.r, dt * speedFluidify),
                                Mathf.MoveTowards(objLight.color.g, aimedColor.g, dt * speedFluidify),
                                Mathf.MoveTowards(objLight.color.b, aimedColor.b, dt * speedFluidify),
                                Mathf.MoveTowards(objLight.color.a, aimedColor.a, dt * speedFluidify));
                    }
                }
                else
                {
                    objLight.color = aimedColor;
                }
            }
            if (flickerPosition)
            {
                if (timeLeftPos < dt)
                {
                    targetPos = initPos + Random.insideUnitSphere * moveRange;
                    timeLeftPos = moveFlickDuration;
                }
                else
                {
                    float valueLerp = dt / timeLeftPos; // "poids" du changement en fonction du dt, on divise par le temps restant pour que le deplacement soit fluide
                    currentPos = Vector3.Lerp(currentPos, targetPos, valueLerp); // lerp de la valeur actuelle vers la valeur visée
                    transform.localPosition = currentPos;
                    timeLeftPos -= dt;
                }

            }

        }
        else
        {
            lightComponent.enabled = false;
        }

    }

    bool InSight(float fovExtention = 0.0f)
    {
        Camera renderCam = CameraHandler.Instance.renderingCam;
        Vector3 diff = transform.position - renderCam.transform.position;
        float dist = diff.magnitude;

        if (dist < Mathf.Max(lightComponent.range, 1e-5f)) // Overlapping or nearly zero?
            return true;

        // Within range of camera.
        if (dist < renderCam.farClipPlane + lightComponent.range)
        {
            // If your aspect ratio <= 1.0f,
            // than you'd use the (default) vertical fov.
            // Otherwise you'd use the horizontal fov.
            float halfFov;

            if (renderCam.aspect <= 1.0f)
                halfFov = (renderCam.fieldOfView + fovExtention) * Mathf.Deg2Rad * 0.5f;
            else
            {
                // You could optimize this by having the horizontal fov
                // calculated only once per frame in your camera class.
                float angle = (renderCam.fieldOfView + fovExtention) * Mathf.Deg2Rad;
                halfFov = Mathf.Atan(Mathf.Tan(angle * 0.5f) * renderCam.aspect);
            }

            float d = Vector3.Dot(renderCam.transform.forward, diff);

            // Within camera's field of view?
            if (Mathf.Acos(d / dist) < halfFov)
                return true;
        }

        return false;
    }
}
