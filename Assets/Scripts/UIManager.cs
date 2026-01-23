using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    private static GameManager Gm => GameManager.Instance;

    public Image castleHealthBar, jumpFiller;
    public TextMeshProUGUI castleHealthText, enemyNumber, coinNumber;
    public GameObject deathMenu;
    public Toggle musicToggle;
    public Button archer1, archer2, archer3, damageButton, rangeButton, jumpButton;
    public Button archerFasten1, archerFasten2, archerFasten3;
    public Button archerDamage1, archerDamage2, archerDamage3;
    
    private bool _archer1Bought, _archer2Bought, _archer3Bought;

    // Update is called once per frame
    void Update()
    {
        castleHealthBar.fillAmount = Gm.health / 100;
        castleHealthText.text = Gm.health.ToString(CultureInfo.InvariantCulture);
        enemyNumber.text = GameManager.Instance.slainEnemies.ToString();
        coinNumber.text = GameManager.Instance.coin.ToString();

        ArcherButtons();
        damageButton.interactable = Gm.coin >= 30;
        rangeButton.interactable = Gm.coin >= 40;
    }
    
    void ArcherButtons()
    {
        if (Gm.coin >= 20)
        {
            archer1.interactable = !_archer1Bought;
            archer2.interactable = !_archer2Bought;
            archer3.interactable = !_archer3Bought;
        }
        else
        {
            archer1.interactable = false;
            archer2.interactable = false;
            archer3.interactable = false;
        }
        
        archer1.gameObject.SetActive(!_archer1Bought);
        archer2.gameObject.SetActive(!_archer2Bought);
        archer3.gameObject.SetActive(!_archer3Bought);
        
        archerFasten1.gameObject.SetActive(_archer1Bought);
        archerFasten2.gameObject.SetActive(_archer2Bought);
        archerFasten3.gameObject.SetActive(_archer3Bought);
        
        archerDamage1.gameObject.SetActive(_archer1Bought);
        archerDamage2.gameObject.SetActive(_archer2Bought);
        archerDamage3.gameObject.SetActive(_archer3Bought);

        if (Gm.coin >= 40)
        {
            archerFasten1.interactable = _archer1Bought;
            archerFasten2.interactable = _archer2Bought;
            archerFasten3.interactable = _archer3Bought;
        }
        else
        {
            archerFasten1.interactable = false;
            archerFasten2.interactable = false;
            archerFasten3.interactable = false;
        }
        
        if (Gm.coin >= 50)
        {
            archerDamage1.interactable = _archer1Bought;
            archerDamage2.interactable = _archer2Bought;
            archerDamage3.interactable = _archer3Bought;
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
        AudioManager.Instance.Mute(ClipType.ThemeMusic);
    }
    public void OpenDeathMenu()
    {
        deathMenu.SetActive(true);
    }
    public void ArcherButton1()
    {
        Gm.ChangeCoinAmount(-20);
        Gm.ActivateArcher(0);
        AudioManager.Instance.Play(ClipType.BuySound);
        _archer1Bought = true;
    }
    public void ArcherButton2()
    {
        Gm.ChangeCoinAmount(-20);
        Gm.ActivateArcher(1);
        AudioManager.Instance.Play(ClipType.BuySound);
        _archer2Bought = true;
    }
    public void ArcherButton3()
    {
        Gm.ChangeCoinAmount(-20);
        Gm.ActivateArcher(2);
        AudioManager.Instance.Play(ClipType.BuySound);
        _archer3Bought = true;
    }
    public void ArcherFastenButton(int archerIndex)
    {
        Gm.ChangeCoinAmount(-40);
        Gm.ActivateFastenArcher(archerIndex);
        AudioManager.Instance.Play(ClipType.BuySound);
    }
    
    public void ArcherDamageButton(int archerIndex)
    {
        Gm.ChangeCoinAmount(-50);
        Gm.ActivateDamageArcher(archerIndex);
        AudioManager.Instance.Play(ClipType.BuySound);
    }
    
    public void DamageButton()
    {
        Gm.IncreaseDamage(1);
        Gm.ChangeCoinAmount(-30);
    }
    
    public void RangeButton()
    {
        Gm.IncreaseRange();
        Gm.ChangeCoinAmount(-30);
    }

    public void StartJumpCooldown(float cooldownTime)
    {
        StartCoroutine(JumpCooldown(cooldownTime));
    }
    
    public IEnumerator JumpCooldown(float cooldownTime)
    {
        jumpButton.interactable = false;
        jumpFiller.fillAmount = 1;
        var elapsed = 0f;
        while (elapsed < cooldownTime)
        {
            elapsed += Time.deltaTime;
            jumpFiller.fillAmount = 1 - (elapsed / cooldownTime);
            yield return null;
        }
        
        jumpFiller.fillAmount = 0;
        jumpButton.interactable = true;
    }
}
