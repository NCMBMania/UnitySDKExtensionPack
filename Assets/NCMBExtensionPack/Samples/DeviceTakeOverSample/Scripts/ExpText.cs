using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class ExpText : MonoBehaviour
{
    Text text;
    RectTransform thisTransform;
    public float fadeTime = 1f;
    public float speed = 2f;
    public Color color = Color.yellow;

    public bool IsEnable { get { return text.enabled; } }

    void Awake()
    {
        text = GetComponent<Text>();
        thisTransform = GetComponent<RectTransform>();
        text.enabled = false;
    }

    public void StartMove(Vector3 potision, int exPoint)
    {
        text.color = color;
        text.text = exPoint +  "EX";

        this.transform.position = potision;

        StartCoroutine("GoUpFadeOut");
        text.enabled = true;
    }

    IEnumerator GoUpFadeOut()
    {
        while (text.color.a > 0.5f)
        {
            text.color = Color.Lerp(text.color, Color.clear, fadeTime * Time.deltaTime);
            thisTransform.Translate(Vector3.up * speed);
            yield return null;
        }

        text.enabled = false;
    }

    public void Disable()
    {
        StopCoroutine("GoUpFadeOut");
        text.enabled = false;
    }
}
