
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
    public class RequestEmailService : BackgroundService
    {
        private readonly IRequestValidationService _requestValidationService;
        private readonly IEmailSender _emailSender;

        public RequestEmailService(IServiceProvider services, IRequestValidationService requestValidationService, IEmailSender emailSender)
        {
            Services = services;
            _requestValidationService = requestValidationService;
            _emailSender = emailSender;
        }

        public IServiceProvider Services { get; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
			List<Model.EmailRequestStatus> employeesList = new List<Model.EmailRequestStatus>();

			while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await _requestValidationService.SearchNewEmailL4RequestAsync();
                    await _requestValidationService.SearchNewEmailNeedOrderBudgetAsync();
                    await _requestValidationService.SearchNewEmailNeedOrderResponseBudgetAsync();
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
