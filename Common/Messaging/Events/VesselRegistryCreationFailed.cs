namespace Common.Messaging.Events
{
    public class VesselRegistryCreationFailed : Event
    {
        public string Reason { get; set; }
    }
}
