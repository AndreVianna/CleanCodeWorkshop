using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;

namespace XPenC.WebApp.Controllers.Tests.TestDoubles
{
    public class DummyWebHostEnvironment : IWebHostEnvironment
    {
        public virtual IFileProvider WebRootFileProvider { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public virtual string WebRootPath { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public virtual string EnvironmentName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public virtual string ApplicationName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public virtual string ContentRootPath { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public virtual IFileProvider ContentRootFileProvider { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}