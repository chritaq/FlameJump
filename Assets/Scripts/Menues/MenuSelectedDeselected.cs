using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuSelectedDeselected : MonoBehaviour, IDeselectHandler, ISelectHandler
{
    [SerializeField] private GameObject background;
    public void OnDeselect(BaseEventData eventData)
    {
        background.SetActive(false);
    }

    public void OnSelect(BaseEventData eventData)
    {
        background.SetActive(true);
    }

    private void OnDisable()
    {
        background.SetActive(false);
    }
}
