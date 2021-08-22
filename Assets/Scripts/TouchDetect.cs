using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchDetect : MonoBehaviour
{
    [SerializeField] private GameObject joystick;
    [SerializeField] private GameObject button1;
    [SerializeField] private GameObject button2;
    [SerializeField] private GameObject button3;
    
    void Update()
    {
        if (Input.touchCount > 0)
        {
            if (joystick) joystick.SetActive(true);
            if (button1) button1.SetActive(true);
            if (button2) button2.SetActive(true);
            if (button3) button3.SetActive(true);
        }
    }
}
