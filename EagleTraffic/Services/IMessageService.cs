namespace EagleTraffic.Services
{
    public interface IMessageService
    {
        public void Send<T>(T message, string topic);
    }
}
