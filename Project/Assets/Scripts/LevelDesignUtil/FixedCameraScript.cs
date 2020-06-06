using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class FixedCameraScript : MonoBehaviour
{

    [SerializeField]
    Transform lookAtTarget = null;
    [SerializeField]
    Transform cameraDummy = null;
    [SerializeField]
    bool activatedCam = true;
    [SerializeField]
    float camSpeed = 5;
    [SerializeField]
    Vector2 camShake = new Vector2(0.3f, 0.4f);
    [SerializeField]
    string fxName = "VFX_CameraExplosion";
    [SerializeField]
    ParticleSystem blinkParticleToStopOnDeath = null;
    [SerializeField]
    GameObject vfxMesh = null;
    [SerializeField]
    GameObject mainMesh = null;

    Material[] mats = null;

    bool hitByBulletBool = false;

    [SerializeField]
    bool canNextSequence = false;

    [SerializeField, ShowIf("canNextSequence")]
    float timeBeforeNextSequence = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        if (lookAtTarget == null) lookAtTarget = CameraHandler.Instance.renderingCam.transform;
        cameraDummy.GetComponent<CamFixedChild>().parentScript = this;

        if (mainMesh != null)
        {
            mats = mainMesh.GetComponent<Renderer>().materials;
        }


    }

    public void hitByBullet(DataWeaponMod mod)
    {
        if (!hitByBulletBool)
        {
            hitByBulletBool = true;

            if (!Weapon.Instance.CheckIfModIsMinigun (mod))
                MetricsGestionnary.Instance.EventMetrics(MetricsGestionnary.MetricsEventType.ShootHit);
            MetricsGestionnary.Instance.EventMetrics(MetricsGestionnary.MetricsEventType.CameraDestroyed);

            Weapon.Instance.OnShotGunHitTarget();

            FxManager.Instance.PlayFx(fxName, cameraDummy.position, cameraDummy.rotation);
            CameraHandler.Instance.AddShake(camShake.x, camShake.y);

            //Prop[] prop = GetComponentsInChildren<Prop>();
            //foreach (Prop pr in prop)
            //{
            //    if (pr != null)
            //    {
            //        pr.enabled = true;
            //    }

            //}

            blinkParticleToStopOnDeath.Stop();

            Rigidbody[] rbList = GetComponentsInChildren<Rigidbody>();
            foreach (Rigidbody rb in rbList)
            {
                if (rb != null)
                {
                    rb.gameObject.transform.parent = null;
                    rb.isKinematic = false;
                    rb.AddExplosionForce(500f, transform.position, 1f);
                }

            }

            if (canNextSequence)
                StartCoroutine(NextSequence());

            if (mats != null)
                mats[1].SetFloat("_RevealLightEnabled", 0);

            if (vfxMesh != null)
                vfxMesh.transform.gameObject.SetActive(false);

            PublicManager.Instance.OnPlayerAction(PublicManager.ActionType.DamageFixedCam, gameObject.transform.position);


            this.enabled = false;
        }
        
    }

    IEnumerator NextSequence()
    {

        yield return new WaitForSeconds(timeBeforeNextSequence);

        SequenceHandler.Instance.NextSequence();

        yield break;
    }


    // Update is called once per frame
    void Update()
    {
        if (!hitByBulletBool)
        {
            Quaternion currentRot = cameraDummy.rotation;
            Quaternion newRot;

            if (activatedCam) cameraDummy.LookAt(lookAtTarget, Vector3.up);
            else
            {
                cameraDummy.LookAt(cameraDummy.position + Vector3.down, Vector3.up);
                newRot = cameraDummy.rotation;
                cameraDummy.rotation = Quaternion.Slerp(currentRot, newRot, Time.deltaTime * camSpeed);
            }
        }

    }
}
