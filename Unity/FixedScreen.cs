using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FixedScreen : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Vector2 TouchDist;
    public Vector2 PointerOld;
    int PointerId;
    public bool ispressed;

    public void OnPointerDown(PointerEventData eventData)
    {
        ispressed = transform;
        PointerId = eventData.pointerId;
        PointerOld = eventData.position;
    }

    private void Update()
    {
        if (ispressed)
        {
            if(PointerId >= 0 && PointerId < Input.touches.Length)
            {
                TouchDist = Input.touches[PointerId].position - PointerOld;
                PointerOld = Input.touches[PointerId].position;
            }
            else
            {
                TouchDist = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - PointerOld;
                PointerOld = Input.mousePosition;
            }
        }
        else
        {
            TouchDist = new Vector2();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ispressed = false;
    }
}