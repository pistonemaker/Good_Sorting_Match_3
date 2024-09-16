using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public List<GameObject> bars;
    public Transform healthBarParent;
    public TextMeshProUGUI healthText;

    private void OnEnable()
    {
        healthBarParent = transform.Find("Bars Group").transform;
        healthText = transform.Find("Health Text").GetComponent<TextMeshProUGUI>();
    }

    public void DecreaseHP(int hp)
    {
        healthText.text = hp.ToString();
        PoolingManager.Despawn(bars[hp]);
        bars.RemoveAt(hp);
    }
}
