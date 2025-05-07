using UnityEngine;
using UnityEngine.UI;

public class MenuItems : MonoBehaviour
{
    [SerializeField] private Slider VolumeSlider;

    public void Exit()
    {
        Application.Quit();
    }

    public void Settings()
    {
        Camera.main.GetComponent<Animator>().SetBool("Settings", true);
    }

    public void Back()
    {
        Camera.main.GetComponent<Animator>().SetBool("Settings", false);
    }

    public void VolumeChanged()
    {
        AudioListener.volume = VolumeSlider.value;
    }

    public void Start()
    {
        VolumeSlider.value = PlayerPrefs.GetFloat("Volume", 1);
    }
}
