using DG.Tweening;
using UnityEngine;

public class BasePanel : MonoBehaviour
{
    protected virtual void OnEnable()
    {
        BlockClick();
        OpenPanel();
        LoadButtonAndImage();
        SetListener();
    }

    protected void BlockClick()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.BlockClick();
        }
        
        else if (HomeManager.Instance != null)
        {
            HomeManager.Instance.BlockClick();
        }
    }

    protected void DeBlockClick(float time)
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.DeBlockClick(time);
        }
        
        else if (HomeManager.Instance != null)
        {
            HomeManager.Instance.DeBlockClick(time);
        }
    }

    protected void OpenPanel()
    {
        transform.localScale = Vector3.one * 1.3f;
        transform.DOScale(Vector3.one, 0.15f);
    }

    protected virtual void ClosePanel(float time)
    {
        DeBlockClick(time);
        gameObject.SetActive(false);
    }

    protected virtual void LoadButtonAndImage()
    {
        
    }

    protected virtual void SetListener()
    {
        
    }

    protected void Reset()
    {
        LoadButtonAndImage();
    }
}