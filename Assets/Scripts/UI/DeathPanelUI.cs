using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeathPanelUI : MonoBehaviour
{
    [SerializeField] GameObject entityPrefab;
    [SerializeField] Button respawnButton;
    Alliance alliance;
    // Start is called before the first frame update
    void Start()
    {
        respawnButton.onClick.AddListener(delegate { OnClickRespawn(this); });
    }

    public void GiveAlliance(Alliance alliance)
    {
        this.alliance = alliance;
    }
    void OnClickRespawn(DeathPanelUI dpUI)
    {
        Instantiate(entityPrefab, alliance.spawnPoint, Quaternion.identity);
        Destroy(dpUI.gameObject);
    }
}
