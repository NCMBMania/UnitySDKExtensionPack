using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;

public class DeviceTakeOverCanvas : MonoBehaviour
{
    public GameObject startPanel;
    public GameObject newGamePanel;
    public GameObject mainPanel;
    public GameObject generateTakeOverCodePanel;
    public GameObject inputTakeOverCodePanel;
    public GameObject connectingPanel;

    public GameObject continuePanelObject;

    public Text savedUserName;
    public Text savedUserNameDescpliption;

    public InputField newGameuserNameField;
    public Text newGameStateErrorText;

    public GameObject expTextPrefab;
    private List<ExpText> expTextList = new List<ExpText>();

    public Text playerLevel;
    public Text playerExp;

    public Text takeOverUserName;
    public GameObject takeOverCodePanelObject;
    public GameObject generateTakeOverCodeButtonObject;
    public Text takeOverCode;

    public InputField inputUserName;
    public InputField inputPassword;
    public Text inputTakeOverCodeStateErrorText;

    public void Awake()
    {
        continuePanelObject.SetActive(false);
        takeOverCodePanelObject.SetActive(false);

        savedUserName.text = string.Empty;
        savedUserNameDescpliption.enabled = false;

        newGameStateErrorText.enabled = false;
        inputTakeOverCodeStateErrorText.enabled = false;

        GenerateExpTextObject(10);
    }

    public void SetPlayerLevel(int level)
    {
        playerLevel.text = level.ToString();
    }
    
    public void SetPlayerExp(int exp)
    {
        playerExp.text = exp.ToString();
    }

    public void ShowContinueAndTakeOverButton()
    {
        continuePanelObject.SetActive(true);
    }

    public void HideContinueAndTakeOverButton()
    {
        continuePanelObject.SetActive(false);
    }

    public void ShowSavedUserName(string userName)
    {
        savedUserName.text = userName;
        savedUserNameDescpliption.enabled = true;

        takeOverUserName.text = userName;
    }

    public void ShowTitlePanel()
    {
        HideAllPanel();
        startPanel.SetActive(true);
    }

    public void ShowMainPanel()
    {
        HideAllPanel();
        mainPanel.SetActive(true);
    }

    public void ShowNewGamePanel()
    {
        HideAllPanel();
        newGameuserNameField.text = string.Empty;
        newGamePanel.SetActive(true);
    }

    public void ShowInputTakeOverCodePanel()
    {
        HideAllPanel();
        inputTakeOverCodePanel.SetActive(true);

        inputUserName.text = string.Empty;
        inputPassword.text = string.Empty;
    }

    public void ShowGenerateTakeOverCodePanel()
    {
        HideAllPanel();
        generateTakeOverCodePanel.SetActive(true);

        takeOverCodePanelObject.SetActive(false);
        generateTakeOverCodeButtonObject.SetActive(true);
    }

    void HideAllPanel()
    {
        generateTakeOverCodePanel.SetActive(false);
        inputTakeOverCodePanel.SetActive(false);
        startPanel.SetActive(false);
        mainPanel.SetActive(false);
        newGamePanel.SetActive(false);
    }

    public string GetUserNameTextFromInput()
    {
        return newGameuserNameField.text;
    }

    public void ShowNewGameStateErrorMessage(string errorMessage)
    {
        newGameStateErrorText.text = errorMessage;
        newGameStateErrorText.enabled = true;
    }

    public void ShowInputTakeOverStateErrorMessage(string errorMessage)
    {
        inputTakeOverCodeStateErrorText.text = errorMessage;
        inputTakeOverCodeStateErrorText.enabled = true;
    }

    public void ShowOverConnectingPanel()
    {
        connectingPanel.SetActive(true);
    }

    public void HideConnectingPanel()
    {
        connectingPanel.SetActive(false);
    }

    void GenerateExpTextObject(int num)
    {
        for (int i = 0; i < num; i++)
        {
            GameObject expTextObject = Instantiate(expTextPrefab) as GameObject;
            expTextObject.transform.SetParent(mainPanel.transform.parent);

            ExpText expText = expTextObject.GetComponent<ExpText>();
            expTextList.Add(expText);
        }
    }

    public void ShowExpText(Vector3 position, int exp)
    {
        ExpText expText = expTextList.FirstOrDefault(et => et.IsEnable == false);

        if(expText == null)
        {
            return;
        }
        
        position += new Vector3(UnityEngine.Random.Range(-10f, 10f), 0f, 0f);
        expText.StartMove(position ,exp);
    }

    public void HideAllExpText()
    {
        expTextList.ForEach(et => et.Disable());
    }

    public void ShowTakeOverCodePanel(string code)
    {
        takeOverCodePanelObject.SetActive(true);
        takeOverCode.text = code;

        generateTakeOverCodeButtonObject.SetActive(false);
    }

    public void HideTakeOverCodePanel()
    {
        takeOverCodePanelObject.SetActive(false);
        takeOverCode.text = string.Empty;
    }

    public string GetInputUserName()
    {
        return inputUserName.text;
    }

    public string GetInputPassword()
    {
        return inputPassword.text;
    }
}
