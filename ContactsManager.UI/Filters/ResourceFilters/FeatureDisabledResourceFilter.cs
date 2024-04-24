﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDExample.Filters.ResourceFilters
{
    public class FeatureDisabledResourceFilter : IAsyncResourceFilter
    {
        private readonly ILogger<FeatureDisabledResourceFilter> _logger;
        private readonly bool _isDisabled;
        public FeatureDisabledResourceFilter(ILogger<FeatureDisabledResourceFilter> logger, bool isDisabled=true)
        {
            _logger = logger;
            _isDisabled = isDisabled;
        }

        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            //To do before logic
            _logger.LogInformation("{FilterName}.{MethodName}-before",nameof(FeatureDisabledResourceFilter),nameof(OnResourceExecutionAsync));
            if (_isDisabled)
            {
                // context.Result=new NotFoundResult(); //error 404 not found
                 context.Result=new StatusCodeResult(501); //error 501 not implemented
            }
            else
            {
                await next();
            }

            //To do after logic
            _logger.LogInformation("{FilterName}.{MethodName}-after", nameof(FeatureDisabledResourceFilter), nameof(OnResourceExecutionAsync));
        }
    }
}
