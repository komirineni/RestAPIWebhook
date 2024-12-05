namespace RestAPIWebhook.Models
{
    public class Subscription
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string CallbackUrl { get; set; }
        public string EventName { get; set; }
    }
}
