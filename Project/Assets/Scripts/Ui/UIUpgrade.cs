using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIUpgrade : MonoBehaviour
{
    // --- Acces static
    public static UIUpgrade Instance { get; private set; }
    void Awake() { Instance = this; }

    [SerializeField, Tooltip("Effet de particule UI à jouer")] UIParticuleSystem particleEffectUpgrade = null;
    [SerializeField, Tooltip("Animation à jouer sur le texte")] DataSimpleAnim textAnim = null;
    [SerializeField, Tooltip("Text à scale")] RectTransform text = null;
    bool doAnimText = false; // Dis si il faut jouer l'animation
    float currPurcentageAnim = 1; // Pourcentage actuel de l'animation


    [SerializeField, Tooltip("Effet de particule UI à jouer sur curseur")] UIParticuleSystem particleEffectUpgradeCursor = null;
    [SerializeField] Canvas cvs = null;
    bool canTpFx = false;

    void Update()
    {
        // --- Animation du texte
        if (doAnimText)
        {
            doAnimText = !textAnim.AddPurcentage(currPurcentageAnim, Time.unscaledDeltaTime, out currPurcentageAnim);
            if (text != null) text.gameObject.SetActive(doAnimText);
            text.localScale = Vector3.one * textAnim.ValueAt(currPurcentageAnim);
        }
        else
            text.localScale = Vector3.zero;


        if (particleEffectUpgradeCursor != null && canTpFx)
        {
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, Main.Instance.GetCursorPos(), cvs.worldCamera, out pos);
            particleEffectUpgradeCursor.transform.position = transform.TransformPoint(pos);
        }
        if (Time.frameCount % Mathf.CeilToInt(1 / (Time.deltaTime != 0 ? Time.deltaTime : 0.01f) / 15) == 0 && !particleEffectUpgradeCursor.isPlaying) canTpFx = false;
    }

    /// <summary>
    /// Fonctions qui gère lance les fx quand le joueur obtien une nouvelle capacité
    /// </summary>
    public void PlayerGetAnUpgrade()
    {
        if (particleEffectUpgrade != null && particleEffectUpgradeCursor != null)
        {
            if (text != null) text.gameObject.SetActive(true);
            particleEffectUpgrade.Play();
            //particleEffectUpgradeCursor.Play();
            currPurcentageAnim = 0;
            doAnimText = true;
            canTpFx = true;
        }
        else
            Debug.Log("NO PARTICLE SYSTEM IN UI UPGRADE");
    }

}
