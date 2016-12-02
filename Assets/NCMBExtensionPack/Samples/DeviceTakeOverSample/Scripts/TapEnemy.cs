using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(Button))]
public class TapEnemy : MonoBehaviour
{
    public int currentHitPoint = 30;
    public int baseHitPoint = 30;
    public int currentLevel = 1;

    public bool isDead = false;

    public Text hitPointText;
    public Text levelText;
    //public Image thisImage;
    private ShakeButton shakeButton;

    public DeviceTakeOverSample main;

    void Start()
    {
        SetHitPointext(currentHitPoint);
        SetLevelText(currentLevel);

        main.IsEnemyDead = false;

        shakeButton = GetComponent<ShakeButton>();
        shakeButton.EnableShake();
    }

    public void TapHit()
    {
        if (isDead) return;

        currentHitPoint -= 10;
        SetHitPointext(currentHitPoint);

        if (currentHitPoint <= 0)
        {
            isDead = true;
            //onDeath();
            Invoke("Respawn", 1f);

            shakeButton.DisableShake();
            GiveExp(30* currentLevel);

            main.IsEnemyDead = true;
            gameObject.SetActive(false);
            
        }
        else
        {
            GiveExp(5 * currentLevel);
        }
    }

    public int GetLevel()
    {
        return currentLevel;
    }

    public void SetLevel(int level)
    {
        currentLevel = level;
    }

    public void SetHitPoint(int hitPoint)
    {
        currentHitPoint = hitPoint;
    }

    public int GetHitPoint()
    {
        return currentHitPoint;
    }

    void SetHitPointext(int hitPoint)
    {
        hitPointText.text = "HP: " + hitPoint.ToString();
    }

    void SetLevelText(int level)
    {
        levelText.text = "Lv: " + level.ToString();
    }

    void GiveExp(int exp)
    {
        main.GiveExp(transform.position, exp);
    }

    public void Clear()
    {
        currentLevel = 1;
        SetLevelText(currentLevel);

        currentHitPoint = 30;
        SetHitPointext(currentHitPoint);

        isDead = false;
        gameObject.SetActive(true);
    }

    void Respawn()
    {
        currentLevel += 1;
        SetLevelText(currentLevel);

        currentHitPoint = baseHitPoint * currentLevel;
        SetHitPointext(currentHitPoint);

        shakeButton.EnableShake();

        isDead = false;

        main.IsEnemyDead = false;
        gameObject.SetActive(true);
    }
}
