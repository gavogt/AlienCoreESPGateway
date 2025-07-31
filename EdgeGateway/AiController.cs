using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EdgeGateway
{
    [Route("api/ai/insights")]
    [ApiController]
    public class AiController : ControllerBase
    {
        public record AiInsightRequest(string Prompt);
        public record AiInsightResponse(string Answer);
        [HttpPost]
        public ActionResult<AiInsightResponse> Post([FromBody] AiInsightRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Prompt))
            {
                return BadRequest("Prompt cannot be empty.");
            }

            var answer = $"AI Insight for: {req.Prompt}";
            return Ok(new AiInsightResponse(answer));
        }
    }
}

