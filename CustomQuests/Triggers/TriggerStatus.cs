namespace CustomQuests.Triggers
{
    public enum TriggerStatus
    {
        Running,
        Success,
        Fail,
    }

    public static class TriggerStatusExtensions
    {
        public static TriggerStatus ToTriggerStatus(this bool value)
        {
            return value ? TriggerStatus.Success : TriggerStatus.Running;
        }
    }
}
