public class OneShotBox : Box
{
    public int curHP;
    public int maxHP;
    public HealthBar healthBar;
    
    private void OnEnable()
    {
        maxItemPositionInRow = 1;
        healthBar = transform.Find("Health Bar Canvas").GetComponent<HealthBar>();
    }

    public override int SetSpecialData()
    {
        return maxHP;
    }
}
