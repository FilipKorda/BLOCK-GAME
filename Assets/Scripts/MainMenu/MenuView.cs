using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace UI
{
    public class MenuView : MonoBehaviour
    {
        [SerializeField] private MenuViewsController menuViewsController;

        [SerializeField] private bool setHeaderActive;
        [SerializeField] private string headerText;

        [SerializeField] private UnityEvent onActive;
        [SerializeField] private UnityEvent onDisable;

        [SerializeField] private bool resetUsersToLastActive;

        [SerializeField] private GameObject[] elementsToDisable;

        public void OnEnable()
        {
            if (setHeaderActive)
            {
                menuViewsController.SetHeaderActive(true);
                menuViewsController.SetHeaderText(headerText);
            }

            if (elementsToDisable is { Length: > 0 })
                foreach (var elementToDisable in elementsToDisable)
                {
                    if (elementToDisable != null) elementToDisable.SetActive(false);
                }

            onActive?.Invoke();
        }

        [ContextMenu("Set as current view", true, 0)]
        public void SetAsCurrentMenuView()
        {
            if (menuViewsController)
                menuViewsController.SetCurrentView(this);
        }

        public void SetAsCurrentMenuViewAfterFrame()
        {
            menuViewsController.StartCoroutine(SetAsCurrentMenuViewCoro());
        }

        private IEnumerator SetAsCurrentMenuViewCoro()
        {
            yield return null;
            SetAsCurrentMenuView();
        }

        public void OnDisable()
        {
            if (setHeaderActive)
            {
                menuViewsController.SetHeaderActive(false);
                menuViewsController.SetHeaderText("");
            }

            if (elementsToDisable is { Length: > 0 })
                foreach (var elementToDisable in elementsToDisable)
                {
                    if (elementToDisable != null) elementToDisable.SetActive(true);
                }

            onDisable?.Invoke();
        }
    }
}