using UnityEngine;

public class ChangingLight : MonoBehaviour
{
    private void OnEnable()
    {
        Invoke(nameof(Disable), 1f);
    }

    private void Disable()
    {
        transform.SetParent(null);
        PoolingManager.Despawn(gameObject);
    }

    public void SetTarget(Transform targetSet)
    {
        transform.SetParent(targetSet);
    }
}
