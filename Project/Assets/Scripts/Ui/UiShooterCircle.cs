using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiShooterCircle : MonoBehaviour
{
    #region Singleton
    private static UiShooterCircle _instance;
    public static UiShooterCircle Instance
    {
        get
        {
            return _instance;
        }
    }
    void Awake()
    {
        _instance = this;
    }
    #endregion

    [SerializeField]
    Transform rootShooterCircle = null;
    Camera RenderCamera;
    private void Start()
    {
        RenderCamera = CameraHandler.Instance.renderingCam;
    }

    public GameObject CreateShooterCircle (GameObject obj)
    {
        return Instantiate(obj, rootShooterCircle.transform);
    }
    public void MoveShooterCircle(GameObject obj, Transform parent)
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, RenderCamera.WorldToScreenPoint(parent.transform.position), GetComponent<Canvas>().worldCamera, out pos);
        obj.transform.position = transform.TransformPoint(pos);
    }

}
