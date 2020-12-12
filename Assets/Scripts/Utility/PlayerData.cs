using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    [System.Serializable]
    struct KeyBind
    {
        public string bindingName;
        public KeyCode keycode;
        public KeyBind(string bindingName, KeyCode keycode)
        {
            this.bindingName = bindingName;
            this.keycode = keycode;
        }
    }
    public Dictionary<string, KeyCode> keybinds { get; private set; }
    [SerializeField] List<KeyBind> keybindings = new List<KeyBind>();

    public float volSFX { get; private set; }
    public float volMusic { get; private set; }

    public static PlayerData Instance = null;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        keybinds = new Dictionary<string, KeyCode>();
        LoadKeybindings();
        DontDestroyOnLoad(this);
    }

    void Update()
    {
        if (Input.GetKeyDown(this.keybinds["Fullscreen"]))
        {
            ToggleFullScreen(!Screen.fullScreen);
        }
    }

    public void SetKey(string bindingName, KeyCode keycode)
    {
        keybinds[bindingName] = keycode;
    }

    public void OnApplicationQuit()
    {
        foreach(KeyValuePair<string, KeyCode> kvp in keybinds)
        {
            PlayerPrefs.SetString(kvp.Key, kvp.Value.ToString());
        }

        PlayerPrefs.SetInt("Screenmanager Resolution Height", Screen.height);
        PlayerPrefs.SetInt("Screenmanager Resolution Width" , Screen.width );
    }

    public void LoadKeybindings()
    {
        for(int i = 0; i < keybindings.Count; i++)
        {
            keybinds[keybindings[i].bindingName] = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(keybindings[i].bindingName, keybindings[i].keycode.ToString()));
        }
    }
    public void ToggleFullScreen(bool isOn)
    {
        Screen.fullScreen = isOn;
        int s = 0;
        if(isOn == true)
        {
            s = 1;
        }
        PlayerPrefs.SetInt("Screenmanager Is Fullscreen mode", s);
    }

    public void SetMusicVolume(System.Single volume)
    {
        volMusic = volume;
    }

    public void SetSFXVolume(System.Single volume)
    {
        volSFX = volume;
    }
}
