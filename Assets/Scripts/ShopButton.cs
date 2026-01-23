using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopButton : MonoBehaviour
{
    [SerializeField] private TMP_Text costText;
    [SerializeField] private Button button;

    
    public void SetInteractability(bool state)
    {
        button.interactable = state;
    }
    public void SetCost(int cost)
    {
        costText.text = cost.ToString();
    }

    public void AddListener(Action action)
    {
        button.onClick.AddListener(() => action());
    }
    
    [ContextMenu("Find References")]
    public void FindRefs()
    {
        costText = GetComponentInChildren<TMP_Text>();
        button = GetComponent<Button>();
    }
}