using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject pauseMenu;
    public Slider soundSlider;
    public Slider SFXSlider;

    
    private void Update()
    {
        ChangeMusicVolume(soundSlider.value);
        ChangeSFXVolume(SFXSlider.value);
    }

    public void ChangeMusicVolume(float sliderValue)
    {
        AudioManager.instance.changeMusicVolumn(sliderValue);
    }
    
    public void ChangeSFXVolume(float sliderValue)
    {
        AudioManager.instance.changeSFXVolumn(sliderValue);
    }

    public void PauseGameControl()
    {
        Time.timeScale = 0;
        pauseMenu.SetActive(true);
    }

    public void ContinueGameControl()
    {
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
    }

    public void ReplayGameControl()
    {
        SceneManager.LoadScene(1);
        Time.timeScale = 1;
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene(0);
    }



}
