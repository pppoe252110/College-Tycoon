public class DormitoryBuilding : Building
{
    public override void Update()
    {
        var period = TimeManager.Instance.GetCurrentAction();
        if (people.Count > 0 && (period != PeriodAction.Sleep))
        {
            ReleasePeople();
        }
    }
}
