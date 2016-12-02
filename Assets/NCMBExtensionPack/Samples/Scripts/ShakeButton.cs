using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ShakeButton : MonoBehaviour
{
    private Button button;
    private RectTransform thisTransform;
    private Vector3 originalPosition;

    private Coroutine coroutine;
    private bool isShakeAvailable;

    private void Awake()
    {
        button = this.GetComponent<Button>();
        button.onClick.AddListener(() =>
        {
            if (isShakeAvailable)
            {
                coroutine = StartCoroutine("Shake");
            }
        });

        thisTransform = this.GetComponent<RectTransform>();
        originalPosition = thisTransform.position;
    }

    public void EnableShake()
    {
        isShakeAvailable = true;
    }

    public void DisableShake()
    {
        if (coroutine != null) StopCoroutine(coroutine);
        thisTransform.position = originalPosition;
        isShakeAvailable = false;
    }

    private IEnumerator Shake()
    {
        for (int i = 0; i < 10; i++)
        {
            thisTransform.Translate(new Vector3(Random.Range(-7f, 7f), Random.Range(-2f, 2f), 0f));
            yield return new WaitForSeconds(Random.Range(0.005f, 0.01f));
        }

        thisTransform.position = originalPosition;
    }
}