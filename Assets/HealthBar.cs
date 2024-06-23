using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] Sprite heartFull;
    [SerializeField] Sprite heartEmpty;
    [SerializeField] List<Image> hearts;

    private void Awake()
    {
        PlayerDie.onHPLost += UpdateUI;
    }

    private void OnDestroy()
    {
        PlayerDie.onHPLost -= UpdateUI;
    }

    private void UpdateUI(float hp)
    {
        for (int i = 0; i < hearts.Count; i++)
        {
            if (i <= hp - 1)
            {
                hearts[i].sprite = heartFull;
            }
            else
            {
                hearts[i].sprite = heartEmpty;
            }
        }

    }
}
