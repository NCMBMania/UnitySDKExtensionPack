using UnityEngine;
using NCMB;
using NCMBExtension;
using System;

public class CloudSaveSample : MonoBehaviour {

    public NCMBUserAuth ncmbUserAuth;
    public CloudSaveCanvas cloudSaveCanvas;

    public void Start()
    {
        OnLogin(this, EventArgs.Empty);
    }

    public void OnLogin(object sender, EventArgs e)
    {
        cloudSaveCanvas.DisableConenctingScreen();
        cloudSaveCanvas.SwitchLogin();
    }

    public void SaveScore()
    {
        NCMBPlayerPrefs.SetInt("Score", cloudSaveCanvas.GetScore());
        NCMBPlayerPrefs.Save(SaveSuccess, SaveFailed);
        cloudSaveCanvas.EnableConenctingScreen();
    }

    public void GetScore()
    {
        Debug.Log("Get Score ");

        string score = NCMBPlayerPrefs.GetInt("Score", failueEventHandler:FailueGetScore).ToString();
        cloudSaveCanvas.SetScore(score);
    }

    public void FailueGetScore(ConnectionEventArgs eventArgs)
    {
        cloudSaveCanvas.SetErrorMessage(eventArgs.errorMessage);
        cloudSaveCanvas.DisableConenctingScreen();
    }
    
    public void SaveSuccess(object sender, EventArgs e)
    {
        Debug.Log("Success Save ");
        cloudSaveCanvas.DisableConenctingScreen();
    }

    public void SaveFailed(ConnectionEventArgs eventArgs)
    { 
        Debug.Log("Save Failed");
        cloudSaveCanvas.DisableConenctingScreen();

        cloudSaveCanvas.SetErrorMessage(eventArgs.errorMessage);
        cloudSaveCanvas.DisableConenctingScreen();
    }

    public void StartLogin()
    {
        string userName = cloudSaveCanvas.GetCurrentUserNameFieldText();
        string password = cloudSaveCanvas.GetCurrentPasswordFieldText();

        cloudSaveCanvas.EnableConenctingScreen();
        ncmbUserAuth.Login(userName, password, OnMain, OnLoginRetry);
    }

    public void StartSignin()
    {
        string userName = cloudSaveCanvas.GetCurrentUserNameFieldText();
        string password = cloudSaveCanvas.GetCurrentPasswordFieldText();

        cloudSaveCanvas.EnableConenctingScreen();
        ncmbUserAuth.Signin(userName, password, OnMain, OnLoginRetry);
    }


    public void OnMain(object sender, EventArgs e)
    {
        cloudSaveCanvas.DisableConenctingScreen();
        cloudSaveCanvas.SwicthMain();
        cloudSaveCanvas.SetUserName(NCMBUser.CurrentUser.UserName);
    }

    public void OnLoginRetry(object sender, EventArgs e)
    {
        cloudSaveCanvas.DisableConenctingScreen();
        cloudSaveCanvas.EnableLoginRetry();
    }

    public void LogOut()
    {
        cloudSaveCanvas.EnableConenctingScreen();
        ncmbUserAuth.Logout(OnLogin, OnLogin);
    }

    public void Clear()
    {
        NCMBPlayerPrefs.DeleteServerKey("Score");
        NCMBPlayerPrefs.DeleteLocalKey("Score");
    }
}
