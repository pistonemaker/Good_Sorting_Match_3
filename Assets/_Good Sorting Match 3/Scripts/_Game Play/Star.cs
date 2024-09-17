using DG.Tweening;
using UnityEngine;

public class Star : MonoBehaviour
{
    public void MoveStarToUI(Transform target, float jumpPower = 0.75f, int numJumps = 1, float duration = 1f)
    {
        transform.DOJump(target.position, jumpPower, numJumps, duration).SetEase(Ease.InOutQuad).OnComplete(() => 
        {
            this.PostEvent(EventID.On_Update_Star_Text, 1);
            PoolingManager.Despawn(gameObject);
        });
    }
}
