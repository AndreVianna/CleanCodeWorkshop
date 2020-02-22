using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace XPenC.WebApp.Tests.TestDoubles
{
    internal class DummyApplicationBuilder : IApplicationBuilder
    {
        public virtual IServiceProvider ApplicationServices { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public virtual IDictionary<string, object> Properties => throw new NotImplementedException();

        public virtual IFeatureCollection ServerFeatures => throw new NotImplementedException();

        public virtual RequestDelegate Build() => throw new NotImplementedException();
        public virtual IApplicationBuilder New() => throw new NotImplementedException();
        public virtual IApplicationBuilder Use(Func<RequestDelegate, RequestDelegate> middleware) => throw new NotImplementedException();
    }
}