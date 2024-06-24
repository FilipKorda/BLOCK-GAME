using System;
using MoreMountains.Feedbacks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization.Events;

namespace UI
{
    public class MenuViewsController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI menuViewHeader;
        [SerializeField] private RectTransform menuHeaderContentContainer;
        [SerializeField] private MenuView startMenuView;
        [SerializeField] private MMF_Player headerAnimation;
        [SerializeField] private UnityEventString updateString = new UnityEventString();
        [SerializeField] private UnityEvent onMenuStart;

        public UnityEventString OnUpdateString
        {
            get => updateString;
            set => updateString = value;
        }

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

            HandeFeedback(setActive);
        }

        public void HandeFeedback(bool play)
        {
            if (!headerAnimation) return;
            if (play) headerAnimation.PlayFeedbacks();
            else if (headerAnimation.IsPlaying) headerAnimation.StopFeedbacks();
        }

        public void SetHeaderText(string text) => menuViewHeader.text = text;


        public void ExitGame() => UIMenuManager.Instance.ExitGame();
    }
}

