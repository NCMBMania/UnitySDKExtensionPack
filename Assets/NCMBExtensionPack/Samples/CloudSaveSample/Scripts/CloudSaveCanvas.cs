using UnityEngine;
using UnityEngine.UI;

public class CloudSaveCanvas : MonoBehaviour
{
    public GameObject loginPanel;
    public GameObject mainPanel;
    public GameObject connectingPanel;

    public Text userName;
    public InputField scoreInput;

    public Text retryText;
    public Text errorMessage;

    public InputField userNameField;
    public InputField passwordField;

    public string GetCurrentUserNameFieldText()
    {
        return userNameField.text;
    }

    public string GetCurrentPasswordFieldText()
    {
        return passwordField.text;
    }

    public void SwitchLogin()
    {
        errorMessage.enabled = false;
        retryText.enabled = false;
        loginPanel.SetActive(true);
        mainPanel.SetActive(false);
    }

    public void SwicthMain()
    {
        errorMessage.enabled = false;
        retryText.enabled = false;
        loginPanel.SetActive(false);
        mainPanel.SetActive(true);

        scoreInput.text = string.Empty;
    }

    public void SetUserName(string name)
    {
        userName.text = name;
    }

    public void SetScore(string score)
    {
        scoreInput.text = score;
    }

    public int GetScore()
    {
        int scoreInt;
        int.TryParse(scoreInput.text, out scoreInt);
        return scoreInt;
    }

    public void EnableConenctingScreen()
    {
        connectingPanel.SetActive(true);
    }

    public void DisavleConenctingScreen()
    {
        connectingPanel.SetActive(false);
    }

    public void EnableLoginRetry()
    {
        retryText.enabled = true;
    }

    public void SetErrorMessage(string message)
    {
        errorMessage.text = message;
        errorMessage.enabled = true;
    }
}
