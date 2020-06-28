using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnparentAndFollow : MonoBehaviour
{

    [SerializeField] Transform _child = null;

    void Start()
    {
        if (_child != null)
            _child.SetParent(null);
    }


    void Update()
    {
        if (_child != null)
        {
            _child.transform.position = transform.position;
            _child.transform.rotation = transform.rotation;
        }
    }

    void OnDisable()
    {
        if (_child != null)
            _child.gameObject.SetActive(false);
    }

    void OnEnable()
    {
        if (_child != null)
            _child.gameObject.SetActive(true);
    }
}
