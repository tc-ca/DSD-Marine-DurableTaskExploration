namespace GatewayApi.Sagas.FirstRegistry
{
    /// <summary>
    /// Combination of output from the two saga participants.
    /// </summary>
    public class SagaOutput
    {
        public string OfficialNumber { get; set; }
       
        public string RegistryId { get; set; }

        public bool Succeeded { get; set; }

        public bool IsDataConsistent { get; set; }
    }
}
