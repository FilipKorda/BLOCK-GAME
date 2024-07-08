using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEST : MonoBehaviour
{
    [SerializeField]
    private bool _myValue;

    public bool MyValue
    {
        get { return _myValue; }
        set
        {
            _myValue = value;
            Debug.Log("MyValue set to: " + _myValue);
        }
    }

    private void Start()
    {
        MyValue = false;
        Debug.Log("Start: MyValue is " + MyValue);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            MyValue = !MyValue;
            Debug.Log("Update: MyValue toggled to " + MyValue);
        }
    }
}
