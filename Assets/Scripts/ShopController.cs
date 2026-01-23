using UnityEngine;

public class ShopController : MonoBehaviour
{
    private static readonly int Left = Animator.StringToHash("Slide Left");
    private static readonly int Right = Animator.StringToHash("Slide Right");
    
    private Animator _anim;
    private GameManager _gm;
    private bool _shopIsOpen;

    private void Start()
    {
        _anim = gameObject.GetComponent<Animator>();
        _gm = GameManager.Instance;
    }
    public void SlideLeft()
    {
        if (!_shopIsOpen)
        {
            _anim.SetTrigger(Left);
            _shopIsOpen = true;
        }
        else {
            SlideRight();
        }
    }
    public void SlideRight()
    {
        if (_shopIsOpen)
        {
            _anim.SetTrigger(Right);
            _gm.Unpause();
            _shopIsOpen = false;
        }
    }

    private void ShopOpened()
    {
        _gm.Pause();
        _gm.UpdateShopItems();
    }

    private void ShopClosed()
    {
        //
    }
}
