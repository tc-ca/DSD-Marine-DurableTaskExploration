namespace Common.Messaging.Events
{
    public class VesselRegistryCreated : Event
    {
        public string RegistrationId { get; set; }
    }
}
