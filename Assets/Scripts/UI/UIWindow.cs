using System.Collections;
using UnityEngine;

namespace UI
{
    public class UIWindow : MonoBehaviour
    {
        Transform _child;
        protected virtual void Awake()
        {
            _child = transform.GetChild(0);
        }

        public void Show(float delay =0f)
        {
            StartCoroutine(ShowCoroutine(delay));
        }

        IEnumerator ShowCoroutine(float delay)
        {
            yield return new WaitForSeconds(delay);
            _child.gameObject.SetActive(true);
        }

        public void Hide()
        {
            var animator = _child.GetComponent<Animator>();
            animator.SetTrigger("Hide");
        }
    }
}