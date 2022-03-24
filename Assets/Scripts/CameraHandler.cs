using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraHandler : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera vcam;

    [SerializeField] int zoomSpeed = 1;


    public void Update()
    {
        if (Input.mouseScrollDelta.y != 0) // if there's input, then zoom in or out
        {
            if (Input.mouseScrollDelta.y > 0)
            {
                ZoomIn();
            }
            else
            {
                ZoomOut();
            } 
        }
    }

    public void ZoomIn()
    {
        vcam.m_Lens.OrthographicSize += zoomSpeed;
    }

    public void ZoomOut()
    {
        vcam.m_Lens.OrthographicSize -= zoomSpeed;
    }
}
