using System;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace XPenC.WebApp.Tests.TestDoubles
{
    internal class DummyCompositeMetadataDetailsProvider : ICompositeMetadataDetailsProvider
    {
        public void CreateBindingMetadata(BindingMetadataProviderContext context) => throw new NotImplementedException();
        public void CreateDisplayMetadata(DisplayMetadataProviderContext context) => throw new NotImplementedException();
        public void CreateValidationMetadata(ValidationMetadataProviderContext context) => throw new NotImplementedException();
    }
}