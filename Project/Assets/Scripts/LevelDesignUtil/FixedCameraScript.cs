using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    bool hitByBulletBool = false;


    // Start is called before the first frame update
    void Start()
    {
        if (lookAtTarget == null) lookAtTarget = CameraHandler.Instance.renderingCam.transform;
        cameraDummy.GetComponent<CamFixedChild>().parentScript = this;

    }

    public void hitByBullet()
    {
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

        Rigidbody[] rbList = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rbList)
        {
            if(rb != null)
            {
                rb.isKinematic = false;
                rb.AddExplosionForce(500f, transform.position, 1f);
            }
           
        }



        if (this != null)
        {
            Destroy(this);
        }    
    }


    // Update is called once per frame
    void Update()
    {
        if (!hitByBulletBool)
        {
            Quaternion currentRot = cameraDummy.rotation;
            Quaternion newRot;

            if (activatedCam) cameraDummy.LookAt(lookAtTarget, Vector3.up);
            else cameraDummy.LookAt(cameraDummy.position + Vector3.down, Vector3.up);
            newRot = cameraDummy.rotation;
            cameraDummy.rotation = Quaternion.Lerp(currentRot, newRot, Time.deltaTime * camSpeed);
        }
    }
}
