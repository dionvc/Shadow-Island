using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsConstructor : MonoBehaviour
{
    //Settings will include : fullscreen selector, key assignments, SFX and music volumes
    [SerializeField] GameObject keyCodeElement; //allows key entry
    [SerializeField] GameObject sliderElement; //allows slider usage
    [SerializeField] GameObject headerElement; //a header fill field
    [SerializeField] GameObject checkboxElement; //checkbox element
    string activeKeybind = null;
    GameObject activeKeybinder = null;

    // Start is called before the first frame update
    void Start()
    {
        //Keybindings
        Instantiate(headerElement, this.transform).GetComponentInChildren<TMP_Text>().SetText("Keybindings");
        foreach(KeyValuePair<string, KeyCode> kvp in PlayerData.Instance.keybinds)
        {
            GameObject keybinder = Instantiate(keyCodeElement, this.transform);
            keybinder.transform.GetChild(0).GetComponent<TMP_Text>().SetText(kvp.Key);
            keybinder.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate { OnClickKeybinder(keybinder); } );
            keybinder.transform.GetChild(1).GetComponentInChildren<TMP_Text>().SetText(kvp.Value.ToString());
        }

        //Graphical settings
        Instantiate(headerElement, this.transform).GetComponentInChildren<TMP_Text>().SetText("Graphics");

        GameObject fullscreenToggle = Instantiate(checkboxElement, this.transform); //Enable fullscreen
        fullscreenToggle.GetComponentInChildren<Toggle>().isOn = Screen.fullScreen;
        fullscreenToggle.GetComponentInChildren<Toggle>().onValueChanged.AddListener(PlayerData.Instance.ToggleFullScreen);
        fullscreenToggle.transform.GetChild(0).GetComponent<TMP_Text>().SetText("Fullscreen Mode");
        //Perhaps a combo box of preset resolutions? <- nah why bother, past me

        //Sound section
        Instantiate(headerElement, this.transform).GetComponentInChildren<TMP_Text>().SetText("Sound");

        GameObject musicVolume = Instantiate(sliderElement, this.transform); //Music Volume
        musicVolume.transform.GetChild(0).GetComponent<TMP_Text>().SetText("Music Volume");
        Slider musicSlider = musicVolume.transform.GetChild(1).GetComponent<Slider>();
        musicSlider.minValue = 0.0f;
        musicSlider.maxValue = 1.0f;
        musicSlider.onValueChanged.AddListener(PlayerData.Instance.SetMusicVolume);

        GameObject sfxVolume = Instantiate(sliderElement, this.transform); //SFX Volume
        sfxVolume.transform.GetChild(0).GetComponent<TMP_Text>().SetText("SFX Volume");
        Slider sfxSlider = musicVolume.transform.GetChild(1).GetComponent<Slider>();
        sfxSlider.minValue = 0.0f;
        sfxSlider.maxValue = 1.0f;
        musicSlider.onValueChanged.AddListener(PlayerData.Instance.SetSFXVolume);
    }

    void FixedUpdate()
    {
        if (activeKeybind != null)
        {
            foreach (KeyCode keycode in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKey(keycode))
                {
                    PlayerData.Instance.SetKey(activeKeybind, keycode);
                    activeKeybind = null;
                    activeKeybinder.transform.GetChild(1).GetComponentInChildren<TMP_Text>().SetText(keycode.ToString());
                    break;
                }
            }
        }
    }

    public void OnClickKeybinder(GameObject keybinder)
    {
        activeKeybind = keybinder.GetComponentInChildren<TMP_Text>().text;
        keybinder.transform.GetChild(1).GetComponentInChildren<TMP_Text>().SetText("|");
        activeKeybinder = keybinder;
    }
}
