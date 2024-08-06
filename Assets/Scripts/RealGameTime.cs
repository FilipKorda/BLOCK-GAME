using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RealGameTime : MonoBehaviour
{
    private float elapsedTime = 0f;
    public TextMeshProUGUI timeText;
    private bool startTime;

    public void InitTime()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        if (currentSceneName == "Level 1")
        {
            timeText.gameObject.SetActive(true);
            StartTime();
        }
    }

    void Update()
    {
        if (startTime)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimeText();
        }
    }

    public void StartTime()
    {
        startTime = true;
    }

    public void StopTime()
    {
        startTime = false;
    }

    public void ResetTime()
    {
        startTime = false;
        elapsedTime = 0f;
    }

    public void EnabledTimeText()
    {
        timeText.gameObject.SetActive(true);
        StartTime();
    }

    public void DisableTimeText()
    {
        timeText.gameObject.SetActive(false);
        StopTime();
    }

    private void UpdateTimeText()
    {
        int hours = Mathf.FloorToInt(elapsedTime / 3600);
        int minutes = Mathf.FloorToInt((elapsedTime % 3600) / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);

        timeText.text = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
    }
}
