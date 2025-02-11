using System;
using UnityEngine;

public class UNIT : MonoBehaviour
{
    private GameObject _selectCircle;
    private void Start()
    {
        _selectCircle = transform.GetChild(0).gameObject;
        _selectCircle.gameObject.SetActive(false);
    }
}
