using System;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace XPenC.WebApp.Controllers.Tests.TestDoubles
{
    public class DummyCompositeMetadataDetailsProvider : ICompositeMetadataDetailsProvider
    {
        public void CreateBindingMetadata(BindingMetadataProviderContext context) => throw new NotImplementedException();
        public void CreateDisplayMetadata(DisplayMetadataProviderContext context) => throw new NotImplementedException();
        public void CreateValidationMetadata(ValidationMetadataProviderContext context) => throw new NotImplementedException();
    }
}