using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net;
using EagleTraffic.Models;
using EagleTraffic.Services;

namespace EagleTraffic.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EagleBotsController : ControllerBase
    {
        private readonly IEagleTrafficService _eagleTrafficService;
        private readonly IMessageService _messageService;
        private const string messageTopic = "EagleBots";

        public EagleBotsController(IEagleTrafficService eagleTrafficService, IMessageService messageService)
        {
            _eagleTrafficService = eagleTrafficService;
            _messageService = messageService;
        }

        // GET: api/EagleBots
        [HttpGet]
        public async Task<IEnumerable<EagleBot>> Get()
        {
            return await _eagleTrafficService.GetAllEagleBots();            
        }

        // PUT api/EagleBots
        [HttpPut]
        public async Task Put([FromBody] EagleBot value)
        {
            if (!ModelState.IsValid)
            { 
                RedirectToAction("error");
            }

            _messageService.Send(value, messageTopic);

            if (_eagleTrafficService.IsEagleBotExisting(value.Id.Value))
            {
                await _eagleTrafficService.UpdateEagleBot(value);
            }
            else
            {
                await _eagleTrafficService.AddEagleBot(value);
            }
        }

        [Route("/error")]
        [ApiExplorerSettings(IgnoreApi=true)]
        public IActionResult HandleError() => Problem();
    }
}
