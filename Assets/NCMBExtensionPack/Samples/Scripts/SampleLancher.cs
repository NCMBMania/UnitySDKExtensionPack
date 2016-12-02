using UnityEngine;
using UnityEngine.SceneManagement;

public class SampleLancher : MonoBehaviour
{
    //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    //NCMBSettingsExtendedが未完成のため利用不可//

    public void StartCloudSaveSample()
    {
        SceneManager.LoadScene("CloudSaveSample");
    }

    public void StartDeviceTakeOverSample()
    {
        SceneManager.LoadScene("DeviceTakeOverSample");
    }

}
