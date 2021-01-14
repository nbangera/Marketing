using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.SimpleNotificationService;
using AutoMapper;
using Marketing.Api.Services;
using Marketing.Common.Messages;
using Marketing.Common.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;

namespace Marketing.Api.Controllers
{
    [ApiController]
    [Route("adverts/v1")]
    [Produces("application/json")]
    public class AdvertController : ControllerBase
    {
        private readonly IAdvertStorageService _advertStorageService;

        public AdvertController(IAdvertStorageService advertStorageService, IConfiguration configuration)
        {
            _advertStorageService = advertStorageService;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        [HttpPost]
        [Route("Create")]
        [ProducesResponseType(404)]
        [ProducesResponseType(201, Type = typeof(CreateAdvertResponse))]
        public async Task<IActionResult> Create(AdvertModel model)
        {
            string recordId;
            try
            {
                recordId = await _advertStorageService.AddAsync(model).ConfigureAwait(true);
            }
            catch (KeyNotFoundException)
            {
                return new NotFoundResult();
            }
            catch (Exception exception)
            {
                return StatusCode(500, exception.Message);
            }

            return StatusCode(201, new CreateAdvertResponse {Id = recordId});
        }

        [HttpPut]
        [Route("Confirm")]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Confirm(ConfirmAdvertModel model)
        {
            try
            {
                await _advertStorageService.ConfirmAsync(model);
                await RaiseAdvertConfirmedMessage(model);
            }
            catch (KeyNotFoundException)
            {
                return new NotFoundResult();
            }
            catch (Exception exception)
            {
                    return StatusCode(500, exception.Message);

            }

            return new OkResult();
        }

        private async Task RaiseAdvertConfirmedMessage(ConfirmAdvertModel model)
        {
            var topicArn = Configuration.GetValue<string>("TopicArn");
            var dbModel = await _advertStorageService.GetByIdAsync(model.Id);

            using (var client = new AmazonSimpleNotificationServiceClient())
            {
                var message = new AdvertConfirmedMessage
                {
                    Id = model.Id,
                    Title = dbModel.Title
                };

                var messageJson = JsonConvert.SerializeObject(message);
                await client.PublishAsync(topicArn, messageJson);
            }
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Get(string id)
        {
            try
            {
                var advert = await _advertStorageService.GetByIdAsync(id);
                return new JsonResult(advert);
            }
            catch (KeyNotFoundException)
            {
                return new NotFoundResult();
            }
            catch (Exception)
            {
                return new StatusCodeResult(500);
            }
        }

        [HttpGet]
        [Route("all")]
        [ProducesResponseType(200)]
        [EnableCors("AllOrigin")]
        public async Task<IActionResult> All()
        {
            return new JsonResult(await _advertStorageService.GetAllAsync());
        }
        
    }
}