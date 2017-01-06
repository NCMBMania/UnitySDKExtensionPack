using NCMBExtension;
using System;
using UnityEngine;

#pragma warning disable 0414 //利用されていないフィールドのWarningを表示しない//
public class DeviceTakeOverSample : MonoBehaviour
{
    public NCMBUserAuth ncmbUserAuth;
    public DeviceTakeOverCanvas canvas;

    private enum State { Title, NewGame, Main, Connecting, GenerateTakeOverCode, InputTakeOverCode }

    [SerializeField]
    private State currentState = State.Title;

    public TapEnemy enemy;

    public int playerExp = 0;
    public int playerLevel = 0;

    public bool IsEnemyDead = false;

    private void Start()
    {
        OnTitleState();
    }

    public void OnTitleState()
    {
        canvas.HideConnectingPanel();

        currentState = State.Title;
        canvas.ShowTitlePanel();
        canvas.HideTakeOverCodePanel();

        if (ncmbUserAuth.IsLoginDataExist())
        {
            canvas.ShowContinueAndTakeOverButton();
            canvas.ShowSavedUserName(ncmbUserAuth.GetUserNameFromLoginData());
        }
        else
        {
            canvas.HideContinueAndTakeOverButton();
        }
    }

    public void OnNewGameState()
    {
        currentState = State.NewGame;
        canvas.ShowNewGamePanel();
    }

    private void OnMainState()
    {
        currentState = State.Main;
        Debug.Log("Start Main");

        canvas.HideConnectingPanel();

        canvas.ShowMainPanel();

        canvas.SetPlayerExp(playerExp);
        canvas.SetPlayerLevel(playerLevel);
    }

    public void OnGenerateTakeOverCodeState()
    {
        currentState = State.GenerateTakeOverCode;
        canvas.ShowGenerateTakeOverCodePanel();
    }

    public void OnInputTakeOverCodeState()
    {
        currentState = State.InputTakeOverCode;
        canvas.ShowInputTakeOverCodePanel();
    }

    //ニューゲーム//
    public void CreateNewPlayerAccount()
    {
        //Input Fieldからユーザー名を拾ってくる///
        string userName = canvas.GetUserNameTextFromInput();

        if (string.IsNullOrEmpty(userName))
        {
            //ユーザー名が空なら警告出してやり直し///
            canvas.ShowNewGameStateErrorMessage("ユーザー名を入力して下さい");
            return;
        }

        //端末上の古いデータを消す//
        PlayerPrefs.DeleteAll();

        enemy.Clear();

        //ユーザー名でオートログイン//
        ncmbUserAuth.AutoSignin(userName, LogOutAndChangeMainState, ShowUserNameCantUse);

        //接続中...の表示//
        canvas.ShowOverConnectingPanel();
    }

    private void ShowUserNameCantUse(object sender, EventArgs e)
    {
        canvas.HideConnectingPanel();
        canvas.ShowNewGameStateErrorMessage("別のユーザー名を作成して下さい");
    }

    private void LogOutAndChangeMainState(object sender, EventArgs e)
    {
        //ログイン成功（アカウント作成成功）したら即ログアウトしてゲーム開始//
        ncmbUserAuth.Logout(LoadDataAndChangeMainState, LogOutError);
    }

    //ボタンから呼ばれる//
    public void Continue()
    {
        LoadDataAndChangeMainState(this, EventArgs.Empty);
    }

    //コンティニュー//
    public void LoadDataAndChangeMainState(object sender, EventArgs e)
    {
        playerLevel = PlayerPrefs.GetInt("PlayerLevel", 1);
        playerExp = PlayerPrefs.GetInt("PlayerExp", 0);
        enemy.SetLevel(PlayerPrefs.GetInt("EnemyLevel", 1));
        enemy.SetHitPoint(PlayerPrefs.GetInt("EnemyHitPoint", 30));
        OnMainState();
    }

    private void LogOutError(object sender, EventArgs e)
    {
        canvas.HideConnectingPanel();
    }

    //ゲーム終了//
    public void ExitGame()
    {
        if(IsEnemyDead) return;

        canvas.HideAllExpText();

        PlayerPrefs.SetInt("PlayerLevel", playerLevel);
        PlayerPrefs.SetInt("PlayerExp", playerExp);

        //敵が死んだ瞬間に保存するとおかしなことになる//
        PlayerPrefs.SetInt("EnemyLevel", enemy.GetLevel());
        PlayerPrefs.SetInt("EnemyHitPoint", enemy.GetHitPoint());
        PlayerPrefs.Save();

        OnTitleState();
    }

    public void GiveExp(Vector3 position, int exp)
    {
        canvas.ShowExpText(position, exp);

        playerExp += exp;

        if (playerExp > playerLevel * 100)
        {
            playerExp = 0;
            playerLevel += 1;
        }

        canvas.SetPlayerExp(playerExp);
        canvas.SetPlayerLevel(playerLevel);
    }

    //引き継ぎコードの発行//
    public void ShowTakeOverCodeAndLocalDelete()
    {
        //接続中...の表示//
        canvas.ShowOverConnectingPanel();

        //まず端末に保存されたプレイヤー名・パスワードでNCMBにログイン//
        ncmbUserAuth.AutoLogin(CopySaveDataLocalToServer, TryAgainGenerate);
    }

    private void CopySaveDataLocalToServer(object sender, EventArgs e)
    {
        //NCMBにログイン成功したら、NCMBPlayerPrefsを使ってローカルデータをサーバーにセーブ//
        NCMBPlayerPrefs.CopyIntLocalToServer("PlayerLevel");
        NCMBPlayerPrefs.CopyIntLocalToServer("PlayerExp");
        NCMBPlayerPrefs.CopyIntLocalToServer("EnemyLevel");
        NCMBPlayerPrefs.CopyIntLocalToServer("EnemyHitPoint");
        NCMBPlayerPrefs.Save(ShowTakeOverCodeAndLogOut, TryAgainGenerateError);
    }

    private void ShowTakeOverCodeAndLogOut(object sender, EventArgs e)
    {
        //セーブに成功したら引き継ぎコードとしてパスワードを表示する//
        canvas.ShowTakeOverCodePanel(ncmbUserAuth.GetPasswordFromLoginData());

        //同時にログアウトする//
        ncmbUserAuth.Logout(DeleteLocalData, TryAgainGenerate);
    }

    private void DeleteLocalData(object sender, EventArgs e)
    {    
        //接続中...の非表示//
        canvas.HideConnectingPanel();

        //ローカルに保存されたプレイヤー名・パスワードファイルを削除する//
        ncmbUserAuth.DeleteLoginData();

        //PlayerPrefsの内容も削除する//
        PlayerPrefs.DeleteAll();
    }

    private void TryAgainGenerateError(ConnectionEventArgs e)
    {
        //接続中...の非表示//
        canvas.HideConnectingPanel();
    }

    private void TryAgainGenerate(object sender, EventArgs e)
    {
        //接続中...の非表示//
        canvas.HideConnectingPanel();
    }

    public void LoginInputAccount()
    {
        //InputFieldからID・パスワードを取得//
        string userName = canvas.GetInputUserName();
        string password = canvas.GetInputPassword();

        //空欄があったらエラー//
        if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
        {
            canvas.ShowInputTakeOverStateErrorMessage("ユーザー名とコードを入力して下さい");
            return;
        }

        //接続中...の表示//

        canvas.ShowOverConnectingPanel();
        //ログインに挑戦//
        ncmbUserAuth.Login(userName, password, CopySaveDataServerToLocal, TryAgainInput);
    }

    private void CopySaveDataServerToLocal(object sender, EventArgs e)
    {         
        //サーバーからローカルへコピー//
        NCMBPlayerPrefs.CopyIntServerToLocal("PlayerLevel");
        NCMBPlayerPrefs.CopyIntServerToLocal("PlayerExp");
        NCMBPlayerPrefs.CopyIntServerToLocal("EnemyLevel");
        NCMBPlayerPrefs.CopyIntServerToLocal("EnemyHitPoint");

        //データを取得したらサーバー側のアカウントを削除//
        ncmbUserAuth.DeleteCurrentAccount(ReGenerateAccount, LogOutError);
    }

    private void ReGenerateAccount(object sender, EventArgs e)
    {
        //入力されたユーザー名でアカウントを再作成し、メインゲームへ遷移//
        ncmbUserAuth.AutoSignin(canvas.GetInputUserName(), LogOutAndChangeMainState, TryAgainInput);

        //接続中...の表示//
        canvas.ShowOverConnectingPanel();
    }

    private void TryAgainInput(object sender, EventArgs e)
    {
        canvas.HideConnectingPanel();
        canvas.ShowInputTakeOverStateErrorMessage("もう一度入力して下さい");
    }
}