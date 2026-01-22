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
    public Button archerFasten1, archerFasten2, archerFasten3;
    public Button archerDamage1, archerDamage2, archerDamage3;
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
        enemyNumber.text = GameManager.Instance.slainEnemies.ToString();
        coinNumber.text = GameManager.Instance.coin.ToString();

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
            archer1.interactable = !archer1bought;
            archer2.interactable = !archer2bought;
            archer3.interactable = !archer3bought;
        }
        else
        {
            archer1.interactable = false;
            archer2.interactable = false;
            archer3.interactable = false;
        }
        
        archer1.gameObject.SetActive(!archer1bought);
        archer2.gameObject.SetActive(!archer2bought);
        archer3.gameObject.SetActive(!archer3bought);
        
        archerFasten1.gameObject.SetActive(archer1bought);
        archerFasten2.gameObject.SetActive(archer2bought);
        archerFasten3.gameObject.SetActive(archer3bought);
        
        archerDamage1.gameObject.SetActive(archer1bought);
        archerDamage2.gameObject.SetActive(archer2bought);
        archerDamage3.gameObject.SetActive(archer3bought);

        if (gm.coin >= 40)
        {
            archerFasten1.interactable = archer1bought;
            archerFasten2.interactable = archer2bought;
            archerFasten3.interactable = archer3bought;
        }
        else
        {
            archerFasten1.interactable = false;
            archerFasten2.interactable = false;
            archerFasten3.interactable = false;
        }
        
        if (gm.coin >= 50)
        {
            archerDamage1.interactable = archer1bought;
            archerDamage2.interactable = archer2bought;
            archerDamage3.interactable = archer3bought;
        }
        else
        {
            archerDamage1.interactable = false;
            archerDamage2.interactable = false;
            archerDamage3.interactable = false;
        }
    }
    public void MusicToggle()
    {
        AudioManager.Instance.Mute("Theme Music");
    }
    public void OpenDeathMenu()
    {
        deathMenu.SetActive(true);
    }
    public void ArcherButton1()
    {
        gm.ChangeCoinAmount(-20);
        gm.ActivateArcher(0);
        AudioManager.Instance.Play("Buy Sound");
        archer1bought = true;
    }
    public void ArcherButton2()
    {
        gm.ChangeCoinAmount(-20);
        gm.ActivateArcher(1);
        AudioManager.Instance.Play("Buy Sound");
        archer2bought = true;
    }
    public void ArcherButton3()
    {
        gm.ChangeCoinAmount(-20);
        gm.ActivateArcher(2);
        AudioManager.Instance.Play("Buy Sound");
        archer3bought = true;
    }
    public void ArcherFastenButton(int archerIndex)
    {
        gm.ChangeCoinAmount(-40);
        gm.ActivateFastenArcher(archerIndex);
        AudioManager.Instance.Play("Buy Sound");
    }
    
    public void ArcherDamageButton(int archerIndex)
    {
        gm.ChangeCoinAmount(-50);
        gm.ActivateDamageArcher(archerIndex);
        AudioManager.Instance.Play("Buy Sound");
    }
    
    public void DamageButton()
    {
        gm.IncreaseDamage(1);
        gm.ChangeCoinAmount(-30);
    }
}
