using UnityEngine;
using NCMB;
using NCMBExtended;

public class CloudSaveSample : MonoBehaviour {

    public NCMBUserAuth ncmbUserAuth;
    public CloudSaveCanvas cloudSaveCanvas;

    public void Start()
    {
        OnLogin();
    }

    public void OnLogin()
    {
        cloudSaveCanvas.DisavleConenctingScreen();
        cloudSaveCanvas.SwitchLogin();
    }

    public void OnMain()
    {
        cloudSaveCanvas.DisavleConenctingScreen();
        cloudSaveCanvas.SwicthMain();
        cloudSaveCanvas.SetUserName(NCMBUser.CurrentUser.UserName);
    }

    public void SaveScore()
    {
        NCMBPlayerPrefs.SetInt("Score", cloudSaveCanvas.GetScore());
        NCMBPlayerPrefs.Save(SaveSuccess, SaveFailed, ErrorMessage);
        cloudSaveCanvas.EnableConenctingScreen();
    }

    public void ErrorMessage(string message)
    {
        cloudSaveCanvas.SetErrorMessage(message);
        cloudSaveCanvas.DisavleConenctingScreen();
    }

    public void GetScore()
    {
        Debug.Log("Get Score ");

        string score = NCMBPlayerPrefs.GetInt("Score", errorMessageCallback: ErrorMessage).ToString();
        cloudSaveCanvas.SetScore(score);
    }
    
    public void SaveSuccess()
    {
        Debug.Log("Success Save ");
        cloudSaveCanvas.DisavleConenctingScreen();
    }

    public void SaveFailed()
    {
        Debug.Log("Save Failed");
        cloudSaveCanvas.DisavleConenctingScreen();
    }

    public void StartLogin()
    {
        string userName = cloudSaveCanvas.GetCurrentUserNameFieldText();
        string password = cloudSaveCanvas.GetCurrentPasswordFieldText();

        ncmbUserAuth.Login(userName, password, OnMain, OnLoginRetry);
        cloudSaveCanvas.EnableConenctingScreen();
    }

    public void StartSignin()
    {
        string userName = cloudSaveCanvas.GetCurrentUserNameFieldText();
        string password = cloudSaveCanvas.GetCurrentPasswordFieldText();

        ncmbUserAuth.Signin(userName, password, OnMain, OnLoginRetry);
        cloudSaveCanvas.EnableConenctingScreen();
    }

    public void OnLoginRetry()
    {
        cloudSaveCanvas.DisavleConenctingScreen();
        cloudSaveCanvas.EnableLoginRetry();
    }

    public void LogOut()
    {
        ncmbUserAuth.Logout(OnLogin, OnLogin);
        cloudSaveCanvas.EnableConenctingScreen();
    }

    public void Clear()
    {
        NCMBPlayerPrefs.DeleteServerKey("Score");
        NCMBPlayerPrefs.DeleteLocalKey("Score");
    }
}
