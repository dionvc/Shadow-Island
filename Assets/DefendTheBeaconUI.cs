using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DefendTheBeaconUI : MonoBehaviour
{
    [SerializeField] Image nextWaveBar;
    [SerializeField] TextMeshProUGUI waveCount;
    [SerializeField] TextMeshProUGUI beaconText;
    [SerializeField] LoadNewGame loadNewGame;
    GameObject beacon;
    EnemyAllianceController enemyAllianceController;
    int counter = 0;

    void FixedUpdate()
    {
        nextWaveBar.fillAmount = (enemyAllianceController.timeUntilNewWave * 1.0f / enemyAllianceController.startingTimeForNewWave);
        waveCount.text = "Wave " + enemyAllianceController.wave;
        if(beacon == null)
        {
            beaconText.text = "Beacon was destroyed! Returning to main menu...";
            counter++;
            if(counter > 500)
            {
                loadNewGame.ExitToMenu();
            }
        }
    }
    public void SetBeacon(GameObject beacon)
    {
        this.beacon = beacon;
    }

    public void SetEnemyAllianceController(EnemyAllianceController enemyAllianceController)
    {
        this.enemyAllianceController = enemyAllianceController;
    }
}
