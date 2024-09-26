using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonScaler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private float scale = 0.85f;

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.DOKill();
        transform.DOScale(scale, 0.15f).SetUpdate(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        transform.DOScale(Vector3.one, 0.15f).SetUpdate(true);
        AudioManager.Instance.PlaySFX("Button_Click");
    }
}