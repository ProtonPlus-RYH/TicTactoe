using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickOutsideDisappear : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private bool ifInside;
    private Vector3 initialPosition;

    void Start()
    {
        ifInside = false;
        initialPosition = transform.position;
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (!ifInside)
            {
                gameObject.GetComponent<Transform>().position = initialPosition;
                gameObject.SetActive(false);
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ifInside = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ifInside = false;
    }
}
