using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using TrdP.Mvc.DataAnnotations.Localization.Tests.TestDoubles;
using TrdP.Mvc.DataAnnotations.Localization.ValidationProviders;
using Xunit;

namespace TrdP.Mvc.DataAnnotations.Localization.Tests.ValidationProviders
{
    public class MetadataDetailsProviderTests
    {
        private readonly MetadataDetailsProvider _metadataDetailsProvider;

        public MetadataDetailsProviderTests()
        {
            var stringLocalizerFactory = new FakeStringLocalizerFactory();
            _metadataDetailsProvider = new MetadataDetailsProvider(stringLocalizerFactory);
        }

        [Fact]
        public void MetadataDetailsProvider_Constructor_WithNullLocalizerFactory_ShouldThrow()
        {
            Assert.Throws<ArgumentNullException>(() => new MetadataDetailsProvider(null));
        }

        [Fact]
        public void MetadataDetailsProvider_CreateBindingMetadata_WithNullContext_ShouldThrow()
        {
            Assert.Throws<ArgumentNullException>(() => _metadataDetailsProvider.CreateBindingMetadata(null));
        }

        [Theory]
        [InlineData(nameof(TestModel.NoAttributesProperty), null)]
        [InlineData(nameof(TestModel.ReadOnlyProperty), true)]
        public void MetadataDetailsProvider_CreateBindingMetadata_ShouldPass(string propertyName, bool? expectedResult)
        {
            var (modelMetadataIdentity, modelAttributes) = GetModelInfo(propertyName);
            var context = new BindingMetadataProviderContext(modelMetadataIdentity, modelAttributes);
            _metadataDetailsProvider.CreateBindingMetadata(context);
            Assert.Equal(expectedResult, context.BindingMetadata.IsReadOnly);
        }

        [Fact]
        public void MetadataDetailsProvider_CreateValidationMetadata_WithNullContext_ShouldThrow()
        {
            Assert.Throws<ArgumentNullException>(() => _metadataDetailsProvider.CreateValidationMetadata(null));
        }

        [Theory]
        [InlineData(nameof(TestModel.NoAttributesProperty), null, 0)]
        [InlineData(nameof(TestModel.RequiredProperty), true, 1)]
        [InlineData(nameof(TestModel.StringLengthProperty), null, 1)]
        [InlineData(nameof(TestModel.FirstName), null, 3)]
        public void MetadataDetailsProvider_CreateValidationMetadata_ShouldPass(string propertyName, bool? expectedHasValidators, int expectedValidatorsCount)
        {
            var (modelMetadataIdentity, modelAttributes) = GetModelInfo(propertyName);
            var context = new ValidationMetadataProviderContext(modelMetadataIdentity, modelAttributes);
            _metadataDetailsProvider.CreateValidationMetadata(context);
            Assert.Equal(expectedHasValidators, context.ValidationMetadata.IsRequired);
            Assert.Equal(expectedValidatorsCount, context.ValidationMetadata.ValidatorMetadata.Count);
        }

        [Fact]
        public void MetadataDetailsProvider_CreateDisplayMetadata_WithNullContext_ShouldThrow()
        {
            Assert.Throws<ArgumentNullException>(() => _metadataDetailsProvider.CreateDisplayMetadata(null));
        }


        [Fact]
        public void MetadataDetailsProvider_CreateDisplayMetadata_ForType_ShouldPass()
        {
            var container = typeof(TestModel);
            var modelMetadataIdentity = ModelMetadataIdentity.ForType(container);
            var modelAttributes = ModelAttributes.GetAttributesForType(container);
            var context = new DisplayMetadataProviderContext(modelMetadataIdentity, modelAttributes);
            _metadataDetailsProvider.CreateDisplayMetadata(context);
        }

        [Theory]
        [InlineData(nameof(TestModel.NoAttributesProperty))]
        [InlineData(nameof(TestModel.ScaffoldColumnProperty))]
        [InlineData(nameof(TestModel.HiddenInputProperty))]
        [InlineData(nameof(TestModel.UiHintProperty))]
        [InlineData(nameof(TestModel.DisplayProperty), "Display", "Description", "Prompt")]
        [InlineData(nameof(TestModel.DisplayNameProperty), "DisplayName")]
        [InlineData(nameof(TestModel.DataTypeProperty))]
        [InlineData(nameof(TestModel.DisplayFormatProperty))]
        [InlineData(nameof(TestModel.RequiredProperty))]
        [InlineData(nameof(TestModel.StringLengthProperty))]
        [InlineData(nameof(TestModel.DataTypePropertyWithDisplayFormat))]
        [InlineData(nameof(TestModel.AlternateDataTypeProperty))]
        [InlineData(nameof(TestModel.DisplayNamePropertyWithNoName), "")]
        public void MetadataDetailsProvider_CreateDisplayMetadata_ForProperty_ShouldPass(string propertyName, string expectedDisplayName = null, string expectedDescription = null, string expectedPlaceholder = null)
        {
            var (modelMetadataIdentity, modelAttributes) = GetModelInfo(propertyName);
            var context = new DisplayMetadataProviderContext(modelMetadataIdentity, modelAttributes);
            _metadataDetailsProvider.CreateDisplayMetadata(context);
            Assert.Equal(expectedDisplayName ?? propertyName, context.DisplayMetadata.DisplayName?.Invoke());
            Assert.Equal(expectedDescription, context.DisplayMetadata.Description?.Invoke());
            Assert.Equal(expectedPlaceholder, context.DisplayMetadata.Placeholder?.Invoke());
        }

        [Fact]
        public void MetadataDetailsProvider_CreateDisplayMetadata_ForEnumProperty_ShouldPass()
        {
            var (modelMetadataIdentity, modelAttributes) = GetModelInfo(nameof(TestModel.TestEnum));
            var context = new DisplayMetadataProviderContext(modelMetadataIdentity, modelAttributes);
            _metadataDetailsProvider.CreateDisplayMetadata(context);
            Assert.NotEmpty(context.DisplayMetadata.EnumGroupedDisplayNamesAndValues);
            var enumAndNames = context.DisplayMetadata.EnumGroupedDisplayNamesAndValues.Select(i => i.Key).ToList();
            Assert.Equal("Name1", enumAndNames[0].Name);
            Assert.Equal("Values", enumAndNames[0].Group);
            Assert.Equal("Value2", enumAndNames[1].Name);
            Assert.Equal("", enumAndNames[1].Group);
            Assert.Equal("Name3", enumAndNames[2].Name);
            Assert.Equal("Values", enumAndNames[2].Group);
            Assert.Equal("Name4", enumAndNames[3].Name);
            Assert.Equal("", enumAndNames[3].Group);
            Assert.NotEmpty(context.DisplayMetadata.EnumNamesAndValues);
        }

        private static (ModelMetadataIdentity modelMetadataIdentity, ModelAttributes modelAttributes) GetModelInfo(
            string propertyName)
        {
            var container = typeof(TestModel);
            var property = container.GetProperty(propertyName);
            Assert.NotNull(property);
            var modelMetadataIdentity = ModelMetadataIdentity.ForProperty(property.PropertyType, property.Name, container);
            var modelAttributes = ModelAttributes.GetAttributesForProperty(container, property);
            return (modelMetadataIdentity, modelAttributes);
        }

        // ReSharper disable UnusedAutoPropertyAccessor.Local
        // ReSharper disable MemberCanBePrivate.Local
        [DisplayColumn("DisplayColumn")]
        private class TestModel
        {
            public string NoAttributesProperty { get; set; }

            [ScaffoldColumn(false)]
            public string ScaffoldColumnProperty { get; set; }

            [HiddenInput]
            public string HiddenInputProperty { get; set; }

            [UIHint(null)]
            public string UiHintProperty { get; set; }

            [Display(Name = "Display", Prompt = "Prompt", Description = "Description", Order = 3)]
            public string DisplayProperty { get; set; }

            [DisplayName("DisplayName")]
            public string DisplayNameProperty { get; set; }

            [DisplayName]
            public string DisplayNamePropertyWithNoName { get; set; }

            [DataType("CustomDataType")]
            public string DataTypeProperty { get; set; }

            [DataType(DataType.Date)]
            [DisplayFormat(DataFormatString = "yyyy-mm-dd", ApplyFormatInEditMode = true)]
            public string DataTypePropertyWithDisplayFormat { get; set; }

            [AlternateDataType]
            public string AlternateDataTypeProperty { get; set; }

            [DisplayFormat(ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true, NullDisplayText = "Null", DataFormatString = "yyyy-mm-dd", HtmlEncode = false)]
            public string DisplayFormatProperty { get; set; }

            [Required]
            public string RequiredProperty { get; set; }

            [StringLength(10)]
            public string StringLengthProperty { get; set; }

            [FirstName]
            public string FirstName { get; set; }

            [Editable(false)]
            public string ReadOnlyProperty { get; set; }

            public TestEnum TestEnum { get; set; }
        }
        // ReSharper restore MemberCanBePrivate.Local
        // ReSharper restore UnusedAutoPropertyAccessor.Local

        // ReSharper disable UnusedMember.Local
        private enum TestEnum
        {
            [Display(GroupName = "Values", Name = "Name1", Order = 2)] Value1,
            Value2,
            [Display(GroupName = "Values", Name = "Name3")] Value3,
            [Display(Name = "Name4")] Value4,
        }
        // ReSharper restore UnusedMember.Local

        private sealed class FirstNameAttribute : ValidationProviderAttribute
        {
            public override IEnumerable<ValidationAttribute> GetValidationAttributes()
            {
                return new List<ValidationAttribute>
                {
                    new RequiredAttribute(),
                    new RegularExpressionAttribute(pattern: "[A-Za-z]*"),
                    new StringLengthAttribute(maximumLength: 30)
                };
            }
        }

        private sealed class AlternateDataTypeAttribute : DataTypeAttribute
        {
            public AlternateDataTypeAttribute() : base(DataType.Date)
            {
            }
        }
    }
}