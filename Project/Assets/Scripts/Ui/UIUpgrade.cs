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
    }

    /// <summary>
    /// Fonctions qui gère lance les fx quand le joueur obtien une nouvelle capacité
    /// </summary>
    public void PlayerGetAnUpgrade()
    {
        if (particleEffectUpgrade != null)
        {
            if (text != null) text.gameObject.SetActive(true);
            particleEffectUpgrade.Play();
            currPurcentageAnim = 0;
            doAnimText = true;
        }
        else
            Debug.Log("NO PARTICLE SYSTEM IN UI UPGRADE");
    }

}
