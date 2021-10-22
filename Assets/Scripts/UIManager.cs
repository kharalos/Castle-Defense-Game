using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    GameManager gm;

    public Image castleHealthBar;
    public TextMeshProUGUI castleHealthText, enemyNumber, coinNumber;
    public GameObject deathMenu;
    public Toggle musicToggle;
    public Button archer1, archer2, archer3, damageButton;
    bool archer1bought, archer2bought, archer3bought;
    void Start()
    {
        if (GameObject.Find("GameManager").GetComponent<GameManager>())
            gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        else
        {
            Debug.LogError("Game Manager could not be found.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        castleHealthBar.fillAmount = gm.health / 100;
        castleHealthText.text = gm.health.ToString();
        enemyNumber.text = FindObjectOfType<GameManager>().slainEnemies.ToString();
        coinNumber.text = FindObjectOfType<GameManager>().coin.ToString();

        ArcherButtons();
        if (gm.coin >= 30)
        {
            damageButton.interactable = true;
        }
        else
            damageButton.interactable = false;
    }
    void ArcherButtons()
    {
        if (gm.coin >= 20)
        {
            if (!archer1bought)
                archer1.interactable = true;
            else
                archer1.interactable = false;
            if (!archer2bought)
                archer2.interactable = true;
            else
                archer2.interactable = false;
            if (!archer3bought)
                archer3.interactable = true;
            else
                archer3.interactable = false;
        }
        else
        {
            archer1.interactable = false;
            archer2.interactable = false;
            archer3.interactable = false;
        }
    }
    public void MusicToggle()
    {
        FindObjectOfType<AudioManager>().Mute("Theme Music");
    }
    public void OpenDeathMenu()
    {
        deathMenu.SetActive(true);
    }
    public void ArcherButton1()
    {
        gm.ChangeCoinAmount(-20);
        gm.ActivateArcher(0);
        FindObjectOfType<AudioManager>().Play("Buy Sound");
        archer1bought = true;
    }
    public void ArcherButton2()
    {
        gm.ChangeCoinAmount(-20);
        gm.ActivateArcher(1);
        FindObjectOfType<AudioManager>().Play("Buy Sound");
        archer2bought = true;
    }
    public void ArcherButton3()
    {
        gm.ChangeCoinAmount(-20);
        gm.ActivateArcher(2);
        FindObjectOfType<AudioManager>().Play("Buy Sound");
        archer3bought = true;
    }
    public void DamageButton()
    {
        gm.IncreaseDamage(1);
        gm.ChangeCoinAmount(-30);
    }
}
