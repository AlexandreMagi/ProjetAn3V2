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
    float exploSize = 1;
    [SerializeField]
    Vector2 camShake = new Vector2(0.3f, 0.4f);
    [SerializeField]
    string fxName = "VFX_ExplosionShooterBullet";


    // Start is called before the first frame update
    void Start()
    {
        if (lookAtTarget == null) lookAtTarget = Player.Instance.transform;
        cameraDummy.GetComponent<CamFixedChild>().parentScript = this;
    }

    public void hitByBullet()
    {
        FxManager.Instance.PlayFx(fxName, cameraDummy.position, cameraDummy.rotation, exploSize);
        CameraHandler.Instance.AddShake(camShake.x, camShake.y);
        Destroy(this.gameObject);
    }


    // Update is called once per frame
    void Update()
    {
        Quaternion currentRot = cameraDummy.rotation;
        Quaternion newRot;

        if (activatedCam) cameraDummy.LookAt(lookAtTarget, Vector3.up);
        else cameraDummy.LookAt(cameraDummy.position + Vector3.down, Vector3.up);
        newRot = cameraDummy.rotation;
        cameraDummy.rotation = Quaternion.Lerp(currentRot, newRot, Time.deltaTime * camSpeed);
    }
}
