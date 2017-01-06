# これは何？
ニフティクラウド mobile backendのUnity SDK向け、機能拡張パックです。
動作にはNCMB Unity SDK v2.2.0が必要です。

https://github.com/NIFTYCloud-mbaas/ncmb_unity/releases/tag/v2.2.0

## できること
 * NCMBにアクセスするためのAPIキーを暗号化できる
 * iOSとAndroid間、または同OS間でセーブデータの引き継ぎ機能が実装できる
 * iOSとAndroidの両方から使えるクラウドセーブ機能が実装できる
 * PlayerPrefsっぽい書き方でクラウドセーブが使える

## 機能
 * エディタにNCMB専用メニューを追加(管理画面をエディタ内で開く、新規プロジェクト用のウィザードなど)
 * アプリキーとクライアントキーを別ファイルへ暗号化して保存する機能
 * データストア機能をPlayerPrefsっぽい使い方ができるラッパークラス
 * オートサインイン・ログイン機能（ユーザパスワードをGUIDから生成、暗号化して保存）
 * NCMBログイン中にエディターの再生モードを停止した際、ユーザーキャッシュデータを削除する

# サンプルシーンの説明
/Samples以下

* Cloud Save Sample.scene：　クラウドセーブ機能のサンプル。（常にサーバーにセーブデータを保存する）
* Device Take Over Sample.scene：　端末引き継ぎ機能のサンプル。（プレイヤーが能動的にセーブデータをサーバーに保存する）
* SampleLauncher.scene：　上記のサンプルを実機で試す際のランチャ

# 各スクリプトの説明

## NCMBMenuItems
Unity Editorのメニューに「NCMB」を追加し、機能を呼び出せるようにします。
* 管理画面を開く (https://console.mb.cloud.nifty.com/ をエディタ内で開く）
* 管理画面をブラウザで開く
* ドキュメントをブラウザで開く
* NCMBSettingsを生成するウィザード（APIキーの暗号化ファイル生成も行う）
* APIキーの暗号化ファイルを再作成する
* 現在ログイン中のユーザー情報をコンソールに出力する

## NCMBWizard
標準のNCMB Unity SDKはApplication Key/Cliend Keyをヒエラルキー内のGameObjectに保存（つまり.unityファイル内にベタ書き）します。
クラッキング対策や、うっかり公開を防ぐためにキーを別ファイル化して保存し、かつ暗号化します。

上記のウィザードから作ると、キーを暗号化して外部ファイル（Resources/Bin）に保存してくれる。
リポジトリ公開するときはこのbytesファイルを除外するだけで事故を防げます。

## NCMBSettingsExtended
外部ファイルからのApplication Key読み込みに対応したNCMB Settingsの派生クラス。
上記のNCMBWizardからGameObjecとしてヒエラルキー内に生成される。

## NCMBUserAuth
Login/Signin管理クラス。
公式サンプルでも似たようなものがありますが、こちらはパスワードをGUIDから自動発行してサインインさせる機能と、ログイン成功失敗時のコールバックを持ちます。

## NCMBPlayerPrefs
NCMBの会員管理へのフィールド追加とデータ保存をPlayerPrefsっぽく叩くためのクラス。
同時にPlayerPrefsにも保存する。

読み込みの際、ローカルとサーバーでデータが異なる場合はタイムスタンプで新しい方をロードする。
ローカルとサーバーのデータコピー機能も搭載。

### 記述例
```
NCMBPlayerPrefs.SetInt("PlayerHP", 100);
NCMBPlayerPrefs.Save(SuccessCallback, FailedCallback);
```

# License
MIT
