using System.Collections;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    private static GameManager Gm => GameManager.Instance;
    
    public ShopData shopData;
    public Image castleHealthBar, jumpFiller;
    public TextMeshProUGUI castleHealthText, enemyNumber, coinNumber;
    public GameObject deathMenu;
    public ShopButton archer1, archer2, archer3, damageButton, rangeButton, jumpButton, archerFasten1,
        archerFasten2, archerFasten3,archerDamage1, archerDamage2, archerDamage3;
    
    private bool _archer1Bought, _archer2Bought, _archer3Bought;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        archer1.SetCost(shopData.ArcherCost);
        archer2.SetCost(shopData.ArcherCost);
        archer3.SetCost(shopData.ArcherCost);
        damageButton.SetCost(shopData.HeroDamageBoostCost);
        rangeButton.SetCost(shopData.HeroRangeBoostCost);
        archerFasten1.SetCost(shopData.ArcherSpeedBoostCost);
        archerFasten2.SetCost(shopData.ArcherSpeedBoostCost);
        archerFasten3.SetCost(shopData.ArcherSpeedBoostCost);
        archerDamage1.SetCost(shopData.ArcherDamageBoostCost);
        archerDamage2.SetCost(shopData.ArcherDamageBoostCost);
        archerDamage3.SetCost(shopData.ArcherDamageBoostCost); 
        
        archer1.AddListener(() => OnArcherButton(0));
        archer2.AddListener(() => OnArcherButton(1));
        archer3.AddListener(() => OnArcherButton(2));
        archerFasten1.AddListener(() => OnArcherFastenButton(0));
        archerFasten2.AddListener(() => OnArcherFastenButton(1));
        archerFasten3.AddListener(() => OnArcherFastenButton(2));
        archerDamage1.AddListener(() => OnArcherDamageButton(0));
        archerDamage2.AddListener(() => OnArcherDamageButton(1));
        archerDamage3.AddListener(() => OnArcherDamageButton(2));
        damageButton.AddListener(OnDamageButton);
        rangeButton.AddListener(OnRangeButton);
    }
    
    public void SetHealth(float health)
    {
        castleHealthBar.fillAmount = health / 100f;
        castleHealthText.text = health.ToString(CultureInfo.InvariantCulture);
    }
    
    public void SetEnemyNumber(int number)
    {
        enemyNumber.text = number.ToString();
    }

    public void SetCoin(int coin)
    {
        coinNumber.text = coin.ToString();
        UpdateButtons(coin);
    }
    
    public void UpdateButtons(int coin)
    {
        damageButton.SetInteractability(coin >= shopData.HeroDamageBoostCost);
        rangeButton.SetInteractability(coin >= shopData.HeroRangeBoostCost);

        if (coin >= shopData.ArcherCost)
        {
            archer1.SetInteractability(!_archer1Bought);
            archer2.SetInteractability(!_archer2Bought);
            archer3.SetInteractability(!_archer3Bought);
        }
        else
        {
            archer1.SetInteractability(false);
            archer2.SetInteractability(false);
            archer3.SetInteractability(false);
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

        if (coin >= shopData.ArcherSpeedBoostCost)
        {
            archerFasten1.SetInteractability(_archer1Bought);
            archerFasten2.SetInteractability(_archer2Bought);
            archerFasten3.SetInteractability(_archer3Bought);
        }
        else
        {
            archerFasten1.SetInteractability(false);
            archerFasten2.SetInteractability(false);
            archerFasten3.SetInteractability(false);
        }
        
        if (coin >= shopData.ArcherDamageBoostCost)
        {
            archerDamage1.SetInteractability(_archer1Bought);
            archerDamage2.SetInteractability(_archer2Bought);
            archerDamage3.SetInteractability(_archer3Bought);
        }
        else
        {
            archerDamage1.SetInteractability(false);
            archerDamage2.SetInteractability(false);
            archerDamage3.SetInteractability(false);
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
    
    private void OnArcherButton(int index)
    {
        if (index == 0) _archer1Bought = true;
        else if (index == 1) _archer2Bought = true;
        else if (index == 2) _archer3Bought = true;
        
        Gm.ActivateArcher(index);
        Gm.ChangeCoinAmount(-shopData.ArcherCost);
        AudioManager.Instance.Play(ClipType.BuySound);
    }

    private void OnArcherFastenButton(int archerIndex)
    {
        Gm.ActivateFastenArcher(archerIndex);
        Gm.ChangeCoinAmount(-shopData.ArcherSpeedBoostCost);
        AudioManager.Instance.Play(ClipType.BuySound);
    }
    
    private void OnArcherDamageButton(int archerIndex)
    {
        Gm.ActivateDamageArcher(archerIndex);
        Gm.ChangeCoinAmount(-shopData.ArcherDamageBoostCost);
        AudioManager.Instance.Play(ClipType.BuySound);
    }
    
    private void OnDamageButton()
    {
        Gm.IncreaseDamage(1);
        Gm.ChangeCoinAmount(-shopData.HeroDamageBoostCost);
        AudioManager.Instance.Play(ClipType.BuySound);
    }
    
    private void OnRangeButton()
    {
        Gm.IncreaseRange();
        Gm.ChangeCoinAmount(-shopData.HeroRangeBoostCost);
        AudioManager.Instance.Play(ClipType.BuySound);
    }

    public void StartJumpCooldown(float cooldownTime)
    {
        StartCoroutine(JumpCooldown(cooldownTime));
    }

    private IEnumerator JumpCooldown(float cooldownTime)
    {
        jumpButton.SetInteractability(false);
        jumpFiller.fillAmount = 1;
        var elapsed = 0f;
        while (elapsed < cooldownTime)
        {
            elapsed += Time.deltaTime;
            jumpFiller.fillAmount = 1 - (elapsed / cooldownTime);
            yield return null;
        }
        
        jumpFiller.fillAmount = 0f;
        jumpButton.SetInteractability(true);
    }
}
