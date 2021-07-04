/*
	https://www.dylanwolf.com/2018/11/24/stupid-unity-ui-navigation-tricks/

	Prevents toggle from being deselect after click outside of menu.
	It sucks but for now it's enough.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PreventDeselectionGroup : MonoBehaviour
{
    EventSystem evt;

    private void Start()
    {
        evt = EventSystem.current;
    }

    GameObject sel;

    private void Update()
    {
        if (evt.currentSelectedGameObject != null && evt.currentSelectedGameObject != sel)
            sel = evt.currentSelectedGameObject;
        else if (sel != null && evt.currentSelectedGameObject == null)
            evt.SetSelectedGameObject(sel);
    }
}

