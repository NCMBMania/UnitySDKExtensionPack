using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TextTransition : MonoBehaviour
{
    Text thisText;
    public string[] stringArray;
    public float spanTime = 0.5f;

    void Awake()
    {
        thisText = GetComponent<Text>();
    }

    void OnEnable()
    {
        if (stringArray.Length == 0) return;

        StartCoroutine("SwitchTextString");
    }

    IEnumerator SwitchTextString()
    {
        int i = 0;

        while(true)
        {
            thisText.text = stringArray[i];
            yield return new WaitForSeconds(spanTime);

            i++;

            if(i > stringArray.Length -1)
            {
                i = 0;
            }
        }
    }

    void OnDisable()
    {
        StopCoroutine("SwitchTextString");
    }

}
