namespace EdgeGateway
{
    public class MqttOptions
    {
        public string ClientId { get; set; } = " ";
        public string Server { get; set; } = " ";
        public int Port { get; set; }
        public bool CleanSession { get; set; }
    }
}
