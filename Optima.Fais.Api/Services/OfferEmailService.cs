
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Optima.Fais.Data;
using Optima.Fais.Dto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Services
{
    public class OfferEmailService : BackgroundService
    {
        private readonly IOfferService _offerService;
        private readonly IEmailSender _emailSender;

        public OfferEmailService(IServiceProvider services, IOfferService offerService, IEmailSender emailSender)
        {
            Services = services;
            _offerService = offerService;
            _emailSender = emailSender;
        }

        public IServiceProvider Services { get; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await _offerService.SearchNewEmailOfferAsync();
                }
                catch (Exception ex)
                {
                    string s = ex.ToString();
                }
                await Task.Delay(600000, stoppingToken);
            }


        }
    }
}
