using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestAPIWebhook.Models;
using System.Text;

namespace RestAPIWebhook.Controllers
{
    [ApiController]
    [Route("api/webhook")]
    public class WebhookController : ControllerBase
    {
        private static readonly List<Subscription> Subscriptions = new();

        [HttpPost("subscribe")]
        public IActionResult Subscribe([FromBody] Subscription subscription)
        {
            if (string.IsNullOrWhiteSpace(subscription.CallbackUrl))
            {
                return BadRequest("Callback URL is required.");
            }

            Subscriptions.Add(subscription);
            return Ok(new { Message = "Subscribed successfully", subscription.Id });
        }

        [HttpGet("subscriptions")]
        public IActionResult GetSubscriptions()
        {
            return Ok(Subscriptions);
        }

        [HttpPost("simulate/{id}")]
        public async Task<IActionResult> Simulate(Guid id)
        {
            var subscription = Subscriptions.FirstOrDefault(s => s.Id == id);
            if (subscription == null)
            {
                return NotFound("Subscription not found.");
            }

            using var httpClient = new HttpClient();
            var payload = new { Event = subscription.EventName, Message = "This is a test event." };
            var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

            try
            {
                var response = await httpClient.PostAsync(subscription.CallbackUrl, content);
                return Ok(new { Message = "Event sent", Status = response.StatusCode });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error sending event", Error = ex.Message });
            }
        }
    }
}
