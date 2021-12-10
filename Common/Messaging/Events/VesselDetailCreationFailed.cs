namespace Common.Messaging.Events
{
    public class VesselDetailCreationFailed : Event
    {
        public string Reason { get; set; }
    }
}
