
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
    public class OrderEmailService : BackgroundService
    {
        private readonly IOrderValidationService _orderValidationService;
        private readonly IEmailSender _emailSender;

        public OrderEmailService(IServiceProvider services, IOrderValidationService orderValidationService, IEmailSender emailSender)
        {
            Services = services;
            _orderValidationService = orderValidationService;
            _emailSender = emailSender;
        }

        public IServiceProvider Services { get; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
			List<Model.EmailOrderStatus> employeesList = new List<Model.EmailOrderStatus>();

			while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
					await _orderValidationService.SearchNewEmailB1OrderAsync();
					await _orderValidationService.SearchNewEmailL4OrderAsync();
                    await _orderValidationService.SearchNewEmailL3OrderAsync();
                    await _orderValidationService.SearchNewEmailL2OrderAsync();
                    await _orderValidationService.SearchNewEmailL1OrderAsync();
                    await _orderValidationService.SearchNewEmailS3OrderAsync();
                    await _orderValidationService.SearchNewEmailS2OrderAsync();
                    await _orderValidationService.SearchNewEmailS1OrderAsync();
                    await _orderValidationService.SearchNewEmailAcceptedOrderAsync();
                    await _orderValidationService.SearchNewEmailNeedOrderBudgetAsync();
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
