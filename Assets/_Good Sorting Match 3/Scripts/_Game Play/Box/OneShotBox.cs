public class OneShotBox : Box
{
    public int curHP;
    public int maxHP;
    public HealthBar healthBar;

    private void OnEnable()
    {
        healthBar = transform.Find("Health Bar Canvas").GetComponent<HealthBar>();
    }

    protected override void CheckForSpecialBox(BoxData boxData)
    {
        base.CheckForSpecialBox(boxData);

        itemPositionInRow = 1;
        curHP = maxHP = boxData.hp;
        CreateHealthBar(maxHP);
    }

    private void CreateHealthBar(int hp)
    {
    }

    public override void SaveBoxData(BoxData boxData)
    {
        base.SaveBoxData(boxData);
        boxData.hp = maxHP;
    }
}