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

        public void Show(float delay = 0f)
        {
            StartCoroutine(ShowCoroutine(delay));
        }

        IEnumerator ShowCoroutine(float delay)
        {
            yield return new WaitForSeconds(delay);
            var animator = GetComponent<Animator>();
            animator.SetTrigger("Show");
        }

        public void Hide()
        {
            var animator = GetComponent<Animator>();
            animator.SetTrigger("Hide");
        }
    }
}