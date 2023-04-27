public class ColledgeBuilding : Building
{
    public float priceMultiplier = 1f;
    public override void Update()
    {
        var period = TimeManager.Instance.GetCurrentAction();
        if (people.Count > 0 && (period == PeriodAction.FreeTime|| period == PeriodAction.Sleep))
        {
            ReleasePeople();
        }
    }

    public override void ReleasePeople()
    {
        for (int i = 0; i < people.Count; i++)
        {
            IdentityManager.Instance.money += 10f * priceMultiplier;
        }
        base.ReleasePeople();
    }
}
