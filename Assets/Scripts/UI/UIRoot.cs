using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class UIRoot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public bool IsTouchOnUI { get; private set; }

        public void OnPointerEnter(PointerEventData eventData)
        {
            IsTouchOnUI = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            IsTouchOnUI = false;
        }
    }
}