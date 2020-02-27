using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptHintTriggerOrb : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        Invoke("FirstHint", 3);
        Invoke("SecondHint", 10);
    }
    private void OnTriggerExit(Collider other)
    {
        HintScript.Instance.Depop();
        CancelInvoke();
        Destroy(this.gameObject);
    }

    void FirstHint()
    {
        HintScript.Instance.PopHint("Il faut peut-être détruire ces sphères...", 5);
    }

    void SecondHint()
    {
        HintScript.Instance.PopHint("Appuyez sur le bouton gauche pour utiliser l'orbe de gravité.");
    }

}
