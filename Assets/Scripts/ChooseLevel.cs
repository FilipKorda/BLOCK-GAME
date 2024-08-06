using System.Collections;
using TMPro;
using UnityEngine;

public class ChooseLevel : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private SceneData[] sceneDatas;
    [SerializeField] private SceneData thisMainMenuScene;
    [SerializeField] private LoadingSystem loadingSystem;
    private readonly int maxLength = 6;

    [SerializeField] private GameObject titleText;
    [SerializeField] private GameObject avaibleCodesPanel;
    [SerializeField] private GameObject chooseLevel;
    [SerializeField] private GameObject gameTitle;

    [SerializeField] private TextMeshProUGUI notificationText;
    private readonly string wrongCode = "<color=red>WRONG CODE!</color>";
    private readonly string fieldIsEmpty = "<color=white>FIELD IS EMPTY!</color>";

    void Start()
    {
        if (inputField != null)
        {
            inputField.onValueChanged.AddListener(ValidateInput);
        }
    }

    private void OnEnable()
    {
        inputField.text = "";
        titleText.SetActive(true);
        avaibleCodesPanel.SetActive(true);
        chooseLevel.SetActive(true);
        gameTitle.SetActive(true);
    }

    private void OnDisable()
    {
        inputField.text = "";
        titleText.SetActive(false);
        avaibleCodesPanel.SetActive(false);
        chooseLevel.SetActive(false);
        gameTitle.SetActive(false);
    }

    public void ActiveChooseLevelPanles()
    {
        titleText.SetActive(true);
        avaibleCodesPanel.SetActive(true);
        chooseLevel.SetActive(true);
        gameTitle.SetActive(true);
    }

    public void DeactiveChooseLevelPanles()
    {
        titleText.SetActive(false);
        avaibleCodesPanel.SetActive(false);
        chooseLevel.SetActive(false);
        gameTitle.SetActive(false);
    }

    void ValidateInput(string input)
    {
        string validInput = "";
        foreach (char c in input)
        {
            if (char.IsDigit(c))
            {
                validInput += c;
            }
        }

        if (validInput.Length > maxLength)
        {
            validInput = validInput.Substring(0, maxLength);
        }

        if (validInput != input)
        {
            inputField.text = validInput;
        }
    }


    public void ClickEnter()
    {
        if (inputField.text == "")
        {
            inputField.text = "";
            Debug.Log("Pole jest puste");
            StartCoroutine(Notification());
            notificationText.text = fieldIsEmpty;
            return;
        }

        bool codeFound = false;

        foreach (var sceneData in sceneDatas)
        {
            if (inputField.text == sceneData.levelCode.ToString())
            {
                loadingSystem.LoadChoosenLexel(sceneData, thisMainMenuScene);
                DeactiveChooseLevelPanles();                
                Debug.Log("<color=green>Good Code for </color>" + sceneData.sceneName);
                codeFound = true;
                break;
            }
        }

        if (!codeFound)
        {
            StartCoroutine(Notification());
            notificationText.text = wrongCode;
            inputField.text = "";
            Debug.Log("<color=red>Wrong Code </color>");
        }
    }

    private IEnumerator Notification()
    {
        notificationText.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        notificationText.gameObject.SetActive(false);
    }

}
