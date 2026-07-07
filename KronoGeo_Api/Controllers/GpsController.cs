using KronoGeo_Api.Applications.MediatR.Commands.Gps;
using KronoGeo_Api.Models.Model.DTO;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Serilog.Core;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace KronoGeo_Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class GpsController (ILogger<AuthenticateController> logger,
        IMediator mediaR) : ControllerBase
    {
        #region private properties
        private readonly ILogger<AuthenticateController> _logger = logger;
        private readonly IMediator _mediaR = mediaR;
        #endregion

        #region public action methods
        // GET: api/<GpsController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<GpsController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<GpsController>
        [HttpPost("Save")]
        public async Task<IActionResult> SaveLocalisations([FromBody] LocalisationGroupDTO value)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid model state.");
                }

                var command = new AddLocalisationsCommand() { LocalisationGroup = value };
                
                var result = await _mediaR.Send(command);

                return Ok(result);
            }
            catch (Exception ex)
            { 
                _logger.LogError(ex, "Erreur lors de la sauvegarde des localisations.");
                return this.Problem("Error while saving localisations.");
            }
        }

        // PUT api/<GpsController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<GpsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
        #endregion
    }
}
}
