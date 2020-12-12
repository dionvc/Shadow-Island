using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnimationProperties
{
    public string propertyName;
    public int propertyId;
    public string[] animationNames;
    public int[] animationHashCodes;
    public void InitializeHashCodes()
    {
        animationHashCodes = new int[animationNames.Length];
        for(int i = 0; i < animationNames.Length; i++)
        {
            animationHashCodes[i] = Animator.StringToHash(animationNames[i]);
        }
    }
}

public class AnimationPropertiesPool : MonoBehaviour
{

    #region Singleton
    public static AnimationPropertiesPool Instance = null;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }
    #endregion Singleton

    [SerializeField] List<AnimationProperties> animationProperties;
    public Dictionary<int, AnimationProperties> animationDictionary { get; private set; }
    // Start is called before the first frame update
    void Start()
    {
        animationDictionary = new Dictionary<int, AnimationProperties>();
        for(int i = 0; i < animationProperties.Count; i++)
        {
            animationProperties[i].InitializeHashCodes();
            animationDictionary[animationProperties[i].propertyId] = animationProperties[i];
        }
    }
}
