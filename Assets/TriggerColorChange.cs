using UnityEngine;
using System.Collections;

public class TriggerColorChange : MonoBehaviour
{
    public Color playerColor;
    public Color enemy1Color;
    public Color enemy2Color;
    public Color enemy3Color;
    public Color enemy4Color;
    
    private bool isPlayerInside = false;
    private bool isEnemy1Inside = false;
    private bool isEnemy2Inside = false;
    private bool isEnemy3Inside = false;
    private bool isEnemy4Inside = false;
    private float remainingTime = 5f;
    private Renderer objectRenderer;
    private Coroutine colorChangeCoroutine;

    private void Start()
    {
        objectRenderer = GetComponent<Renderer>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;
            CheckAndUpdateTimer();
        }
        else if (other.CompareTag("Enemy1"))
        {
            isEnemy1Inside = true;
            CheckAndUpdateTimer();
        }
        else if (other.CompareTag("Enemy2"))
        {
            isEnemy2Inside = true;
            CheckAndUpdateTimer();
        }
        else if (other.CompareTag("Enemy3"))
        {
            isEnemy3Inside = true;
            CheckAndUpdateTimer();
        }
        else if (other.CompareTag("Enemy4"))
        {
            isEnemy4Inside = true;
            CheckAndUpdateTimer();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
            ResetTimer();
        }
        else if (other.CompareTag("Enemy1"))
        {
            isEnemy1Inside = false;
            ResetTimer();
        }
        else if (other.CompareTag("Enemy2"))
        {
            isEnemy2Inside = false;
            ResetTimer();
        }
        else if (other.CompareTag("Enemy3"))
        {
            isEnemy3Inside = false;
            ResetTimer();
        }
        else if (other.CompareTag("Enemy4"))
        {
            isEnemy4Inside = false;
            ResetTimer();
        }
    }

    private void ResetTimer()
    {
        if (colorChangeCoroutine != null)
        {
            StopCoroutine(colorChangeCoroutine);
            colorChangeCoroutine = null;
        }
        remainingTime = 5f;
    }

    private bool IsAnyEnemyInside()
    {
        return isEnemy1Inside || isEnemy2Inside || isEnemy3Inside || isEnemy4Inside;
    }

    private void CheckAndUpdateTimer()
    {
        if (isPlayerInside && IsAnyEnemyInside())
        {
            if (colorChangeCoroutine != null)
            {
                StopCoroutine(colorChangeCoroutine);
                colorChangeCoroutine = null;
            }
            return;
        }

        if ((isPlayerInside || IsAnyEnemyInside()) && colorChangeCoroutine == null)
        {
            colorChangeCoroutine = StartCoroutine(ColorChangeTimer());
        }
    }

    private IEnumerator ColorChangeTimer()
    {
        while (remainingTime > 0)
        {
            if (isPlayerInside && IsAnyEnemyInside())
            {
                yield break;
            }

            yield return new WaitForSeconds(0.1f);
            remainingTime -= 0.1f;
        }

        UpdateColorAndTag();
        remainingTime = 5f;
        colorChangeCoroutine = null;
    }

    private void UpdateColorAndTag()
    {
        if (isPlayerInside && !IsAnyEnemyInside())
        {
            objectRenderer.material.color = playerColor;
            gameObject.tag = "PlayerZemin";
        }
        else if (isEnemy1Inside && !isPlayerInside)
        {
            objectRenderer.material.color = enemy1Color;
            gameObject.tag = "Enemy1Zemin";
        }
        else if (isEnemy2Inside && !isPlayerInside)
        {
            objectRenderer.material.color = enemy2Color;
            gameObject.tag = "Enemy2Zemin";
        }
        else if (isEnemy3Inside && !isPlayerInside)
        {
            objectRenderer.material.color = enemy3Color;
            gameObject.tag = "Enemy3Zemin";
        }
        else if (isEnemy4Inside && !isPlayerInside)
        {
            objectRenderer.material.color = enemy4Color;
            gameObject.tag = "Enemy4Zemin";
        }
    }
}