using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class PressingButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    public float InputPressing;
    public float Sensibility = 2.5f;
    public bool Pressing;

    public void OnPointerDown(PointerEventData eventData)
    {
        Pressing = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Pressing = false;
    }

    private void Update()
    {
        if (Input.GetAxis("Fire2") == 0)
        {
            if (Pressing)
            {
                InputPressing += Time.deltaTime * Sensibility;
            }
            else
            {
                InputPressing -= Time.deltaTime * Sensibility;
            }

            InputPressing = Mathf.Clamp(InputPressing, 0, 1);
        }
    }
}