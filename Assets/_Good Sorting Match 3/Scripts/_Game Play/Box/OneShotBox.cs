using UnityEngine;

public class OneShotBox : Box
{
    public int curHP;
    public int maxHP;
    public HealthBar healthBar;

    protected override void OnEnable()
    {
        base.OnEnable();
        canPutItem = false;
        healthBar = transform.GetChild(0).Find("Health Bar Canvas").GetComponent<HealthBar>();
    }

    protected override void AssignItem()
    {
        int rowNumber = rows.Count;
        if (rowNumber == 0)
        {
            return;
        }

        SetFrontRow(rows[0]);

        if (rowNumber > 1)
        {
            SetBackRow(rows[1]);

            for (int i = rowNumber - 1; i >= 1; i--)
            {
                rows[i].DeactiveRow();
            }
        }
    }

    protected override void SetBackRow(BoxRow boxRow)
    {
        backRow = boxRow;
        backRow.GrayRow();
        backRow.DeactiveRow();
    }

    protected override void SetSpecialBoxData(BoxData boxData)
    {
        base.SetSpecialBoxData(boxData);

        itemPositionInRow = 1;
        curHP = maxHP = boxData.hp;
        CreateHealthBar(maxHP);
    }

    protected override void CheckIfFrontRowEmpty(int id)
    {
        base.CheckIfFrontRowEmpty(id);

        if (id != boxID)
        {
            return;
        }

        if (frontRow.IsEmpty && rows.Count > 1)
        {
            PoolingManager.Despawn(frontRow.gameObject);
            rows.Remove(frontRow);
            SetFrontRow(backRow);
            curRowID++;
            
            if (rows.Count > 1)
            {
                SetBackRow(rows[1]);
            }
            else
            {
                backRow = null;
            }
        }
        
        DecreaseHP();
    }

    private void CreateHealthBar(int hp)
    {
        healthBar.healthText.text = hp.ToString();

        for (int i = 0; i < hp; i++)
        {
            var bar = PoolingManager.Spawn(GameManager.Instance.hpBarPrefab, transform.position, Quaternion.identity);
            bar.transform.SetParent(healthBar.healthBarParent);
            bar.transform.localScale = Vector3.one;
            healthBar.bars.Add(bar);
        }
    }

    private void DecreaseHP()
    {
        curHP--;
        healthBar.DecreaseHP(curHP);
    }

    public override void SaveBoxData(BoxData boxData)
    {
        base.SaveBoxData(boxData);
        boxData.hp = maxHP;
    }
}