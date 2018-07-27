using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MultiDisplayDropdown : Dropdown
{
    protected GameObject m_DropdownComp
    {
        get
        {
            if (baseDropdownhandler != null)
            {
                return baseDropdownhandler.GetValue(this) as GameObject;
            }
            return null;
        }
    }

    private FieldInfo _baseDropdownHandler = null;
    FieldInfo baseDropdownhandler
    {
        get
        {
            if (_baseDropdownHandler == null)
            {
                _baseDropdownHandler = typeof(Dropdown).GetField("m_Dropdown", BindingFlags.NonPublic | BindingFlags.Instance);
            }
            return _baseDropdownHandler;
        }
    }

    protected override GameObject CreateBlocker(Canvas rootCanvas)
    {
        GameObject blockerObj = new GameObject("Blocker");
        RectTransform rectTransform = blockerObj.AddComponent<RectTransform>();
        rectTransform.SetParent(rootCanvas.transform, false);
        rectTransform.anchorMin = Vector3.zero;
        rectTransform.anchorMax = Vector3.one;
        rectTransform.sizeDelta = Vector2.zero;
        Canvas canvas = blockerObj.AddComponent<Canvas>();
        canvas.overrideSorting = true;
        Canvas component = this.m_DropdownComp.GetComponent<Canvas>();
        canvas.sortingLayerID = component.sortingLayerID;
        canvas.sortingOrder = component.sortingOrder - 1;
        var ray = blockerObj.AddComponent<MultiDisplayGraphicRaycaster>();
        InheritGraphicRaycaster(ray);
        Image image = blockerObj.AddComponent<Image>();
        image.color = Color.clear;
        Button button = blockerObj.AddComponent<Button>();
        button.onClick.AddListener(new UnityAction(this.Hide));
        return blockerObj;
    }

    protected override GameObject CreateDropdownList(GameObject template)
    {
        GameObject obj = Instantiate(template);
        var ray = obj.GetComponent<MultiDisplayGraphicRaycaster>();
        InheritGraphicRaycaster(ray);
        return obj;
    }

    private void InheritGraphicRaycaster(MultiDisplayGraphicRaycaster caster)
    {
        var parentRaycaster = gameObject.GetComponentInParent<MultiDisplayGraphicRaycaster>();
        if (parentRaycaster != null)
        {
            caster.DisplayIndex = parentRaycaster.DisplayIndex;
        }
    }
}
