using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Powiadomienie o osiągnięciu w prawym górnym rogu: panel wjeżdża z prawej z lekkim odbiciem,
// ikona "pulsuje", po czasie wyświetlania panel wyjeżdża. Kilka osiągnięć naraz pokazuje się po kolei.
// Animacje działają na czasie rzeczywistym, więc także w pauzie.
public class AchievementUI : MonoBehaviour
{
    [SerializeField] private GameObject achievementPanel;
    [SerializeField] private TextMeshProUGUI achievementTitle;
    [SerializeField] private Image achievementImage;
    [SerializeField] private float displayTime = 3.0f;

    [Header("Animation")]
    [Tooltip("O ile pikseli w prawo panel jest schowany przed wjazdem.")]
    [SerializeField] private float slideDistance = 260f;
    [SerializeField] private float slideInDuration = 0.45f;
    [SerializeField] private float slideOutDuration = 0.35f;
    [SerializeField] private float iconPunchScale = 0.25f;

    private readonly Queue<AchievementData> queue = new();
    private RectTransform panelRect;
    private CanvasGroup canvasGroup;
    private Vector2 shownPosition;
    private bool showing;

    private void Awake()
    {
        panelRect = (RectTransform)achievementPanel.transform;
        shownPosition = panelRect.anchoredPosition;
        if (!achievementPanel.TryGetComponent(out canvasGroup)) canvasGroup = achievementPanel.AddComponent<CanvasGroup>();
    }

    private void Start()
    {
        achievementPanel.SetActive(false);
    }

    public void ShowAchievement(AchievementData data)
    {
        queue.Enqueue(data);
        if (!showing) StartCoroutine(ShowQueue());
    }

    private IEnumerator ShowQueue()
    {
        showing = true;
        while (queue.Count > 0)
        {
            AchievementData data = queue.Dequeue();
            achievementTitle.text = data.achievementTitle;
            achievementImage.sprite = data.achievementImage;

            Vector2 hiddenPosition = shownPosition + new Vector2(slideDistance, 0f);
            panelRect.anchoredPosition = hiddenPosition;
            canvasGroup.alpha = 0f;
            achievementImage.transform.localScale = Vector3.one;
            achievementPanel.SetActive(true);

            Sequence sequence = DOTween.Sequence().SetUpdate(true).SetLink(achievementPanel);
            sequence.Append(panelRect.DOAnchorPos(shownPosition, slideInDuration).SetEase(Ease.OutBack));
            sequence.Join(canvasGroup.DOFade(1f, slideInDuration * 0.5f));
            sequence.Append(achievementImage.transform.DOPunchScale(Vector3.one * iconPunchScale, 0.5f, 6, 0.8f));
            sequence.AppendInterval(displayTime);
            sequence.Append(panelRect.DOAnchorPos(hiddenPosition, slideOutDuration).SetEase(Ease.InBack));
            sequence.Join(canvasGroup.DOFade(0f, slideOutDuration).SetEase(Ease.InQuad));
            yield return sequence.WaitForCompletion();

            achievementPanel.SetActive(false);
            panelRect.anchoredPosition = shownPosition;
        }
        showing = false;
    }
}
