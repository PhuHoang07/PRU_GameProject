using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] Sprite heartFull;
    [SerializeField] Sprite heartEmpty;
    [SerializeField] List<Image> hearts;

    public void UpdateUI(float hp)
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
