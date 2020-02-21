using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;

namespace XPenC.WebApp.Tests.TestDoubles
{
    internal class FakeApplicationBuilder : DummyApplicationBuilder
    {
        private readonly IServiceCollection _serviceCollection;

        public FakeApplicationBuilder(IServiceCollection serviceCollection)
        {
            _serviceCollection = serviceCollection;
        }

        public override IServiceProvider ApplicationServices => _serviceCollection.BuildServiceProvider();
        public override IFeatureCollection ServerFeatures { get; } = new FeatureCollection();
        public override IDictionary<string, object> Properties { get; } = new Dictionary<string, object>();

        public override IApplicationBuilder Use(Func<RequestDelegate, RequestDelegate> middleware)
        {
            return this;
        }

    }
}