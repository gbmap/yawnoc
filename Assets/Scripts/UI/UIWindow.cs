using System.Collections;
using Frictionless;
using UnityEngine;

namespace UI
{

    public class UIVisibleCollection
    {
        UIVisible[] _visibles;

        public UIVisibleCollection(GameObject obj)
        {
            _visibles = obj.GetComponentsInChildren<UIVisible>(true);
        }

        public void Show()
        {
            System.Array.ForEach(_visibles, v => v.Show());
        }

        public void Hide()
        {
            System.Array.ForEach(_visibles, v => v.Hide());
        }
    }

    public class UIWindow : MonoBehaviour
    {
        UIVisibleCollection _visibles;

        public EUIState ShowOnState;
        public EUIState CloseButtonStateChange;

        public bool IsActive;
        
        protected virtual void Awake()
        {
            _visibles = new UIVisibleCollection(gameObject);
        }

        protected virtual void Start()
        {
            if (IsActive)
                Show();
            else
                Hide();
        }

        protected virtual void OnEnable()
        {
            MessageRouter.AddHandler<Messages.UI.OnUIChangeState>(Cb_OnUIStateChange);
        }

        protected virtual void OnDisable()
        {
            MessageRouter.RemoveHandler<Messages.UI.OnUIChangeState>(Cb_OnUIStateChange);
        }

        public void Show(float delay = 0f)
        {
            IsActive = true;
            StartCoroutine(ShowCoroutine(delay));
        }

        IEnumerator ShowCoroutine(float delay)
        {
            yield return new WaitForSeconds(delay);
            var animator = GetComponent<Animator>();
            _visibles.Show();
            animator.SetBool("Active", IsActive);
        }

        public void Hide()
        {
            IsActive = false;
            var animator = GetComponent<Animator>();
            _visibles.Hide();
            animator.SetBool("Active", IsActive);
        }

        public void ChangeState(EUIState state)
        {
            MessageRouter.RaiseMessage(new Messages.UI.OnUIChangeState {
                State = state
            });
        }

        public void Btn_Close()
        {
            ChangeState(CloseButtonStateChange);
        }

        private void Cb_OnUIStateChange(Messages.UI.OnUIChangeState msg)
        {
            if (msg.State == ShowOnState && !IsActive)
                Show();
            else if (IsActive)
                Hide();
        }
    }
}