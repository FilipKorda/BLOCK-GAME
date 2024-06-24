using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace UI
{
    public class MenuViewsController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI menuViewHeader;
        [SerializeField] private RectTransform menuHeaderContentContainer;
        [SerializeField] private MenuView startMenuView;
        [SerializeField] private UnityEvent onMenuStart;

        private void OnEnable()
        {
            onMenuStart?.Invoke();
        }

        private MenuView currentMenuView;

        public void SetStartCurrentView()
        {
            SetCurrentView(startMenuView);
        }

        public void SetCurrentView(MenuView view)
        {
            if (currentMenuView) currentMenuView.gameObject.SetActive(false);
            currentMenuView = view;



            if (currentMenuView) currentMenuView.gameObject.SetActive(true);
        }

        public void SetHeaderActive(bool setActive)
        {
            menuHeaderContentContainer
                .gameObject.SetActive(setActive);


        }



        public void SetHeaderText(string text) => menuViewHeader.text = text;


        //public void ExitGame() => UIMenuManager.Instance.ExitGame();
    }
}

