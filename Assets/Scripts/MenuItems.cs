using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuItems : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown GraphicsDropdown;
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
        PlayerPrefs.SetFloat("Volume", VolumeSlider.value);
        PlayerPrefs.Save();
    }

    public void Start()
    {
        GraphicsDropdown.ClearOptions();
        foreach(string name in QualitySettings.names)
        {
            GraphicsDropdown.options.Add(new TMP_Dropdown.OptionData(name));
        }

        GraphicsDropdown.value = PlayerPrefs.GetInt("Graphics", 0);
        QualitySettings.SetQualityLevel(GraphicsDropdown.value, true);
        Debug.Log($"Changed to graphic setting {GraphicsDropdown.value}");

        VolumeSlider.value = PlayerPrefs.GetFloat("Volume", 1);
    }


    int inStartup = 1000;
    public void Update()
    {
        float frameRate = 1.0f / Time.deltaTime;

        if(inStartup > 0 && inStartup < 990 && frameRate < 30)
        {
            QualitySettings.SetQualityLevel(0, true);
            PlayerPrefs.SetInt("Graphics", 0);
            Debug.Log("Forcing lower graphics!");
        }

        inStartup--;
    }

    public void QualityUpdate(int val)
    {
        QualitySettings.SetQualityLevel(val, true);
        PlayerPrefs.SetInt("Graphics", GraphicsDropdown.value);
        Debug.Log("Changed Quality Setting " + val);
    }
}
