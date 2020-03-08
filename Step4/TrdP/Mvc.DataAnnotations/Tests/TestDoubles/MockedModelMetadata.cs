using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace TrdP.Mvc.DataAnnotations.Localization.Tests.TestDoubles
{
    internal sealed class MockedModelMetadata : ModelMetadata
    {
        private IReadOnlyDictionary<object, object> _additionalValues;
        private ModelPropertyCollection _properties;
        private string _binderModelName;
        private Type _binderType;
        private BindingSource _bindingSource;
        private bool _convertEmptyStringToNull;
        private string _dataTypeName;
        private string _description;
        private string _displayFormatString;
        private string _displayName;
        private string _editFormatString;
        private ModelMetadata _elementMetadata;
        private IEnumerable<KeyValuePair<EnumGroupAndName, string>> _enumGroupedDisplayNamesAndValues;
        private IReadOnlyDictionary<string, string> _enumNamesAndValues;
        private bool _hasNonDefaultEditFormat;
        private bool _htmlEncode;
        private bool _hideSurroundingHtml;
        private bool _isBindingAllowed;
        private bool _isBindingRequired;
        private bool _isEnum;
        private bool _isFlagsEnum;
        private bool _isReadOnly;
        private bool _isRequired;
        private ModelBindingMessageProvider _modelBindingMessageProvider;
        private int _order;
        private string _placeholder;
        private string _nullDisplayText;
        private IPropertyFilterProvider _propertyFilterProvider;
        private bool _showForDisplay;
        private bool _showForEdit;
        private string _simpleDisplayProperty;
        private string _templateHint;
        private bool _validateChildren;
        private IReadOnlyList<object> _validatorMetadata;
        private Func<object, object> _propertyGetter;
        private Action<object, object> _propertySetter;

        public MockedModelMetadata(ModelMetadataIdentity modelMetadataIdentity, Action<MockedModelMetadata> setup = null) : base(modelMetadataIdentity)
        {
            setup?.Invoke(this);
        }

        public void SetAdditionalValues(IReadOnlyDictionary<object, object> value) => _additionalValues = value;
        public override IReadOnlyDictionary<object, object> AdditionalValues => _additionalValues;

        public void SetProperties(ModelPropertyCollection value) => _properties = value;
        public override ModelPropertyCollection Properties => _properties;

        public void SetBinderModelName(string value) => _binderModelName = value;
        public override string BinderModelName => _binderModelName;

        public void SetBinderType(Type value) => _binderType = value;
        public override Type BinderType => _binderType;

        public void SetBindingSource(BindingSource value) => _bindingSource = value;
        public override BindingSource BindingSource => _bindingSource;

        public void SetConvertEmptyStringToNull(bool value) => _convertEmptyStringToNull = value;
        public override bool ConvertEmptyStringToNull => _convertEmptyStringToNull;

        public void SetDataTypeName(string value) => _dataTypeName = value;
        public override string DataTypeName => _dataTypeName;

        public void SetDescription(string value) => _description = value;
        public override string Description => _description;

        public void SetDisplayFormatString(string value) => _displayFormatString = value;
        public override string DisplayFormatString => _displayFormatString;

        public void SetDisplayName(string value) => _displayName = value;
        public override string DisplayName => _displayName;

        public void SetEditFormatString(string value) => _editFormatString = value;
        public override string EditFormatString => _editFormatString;

        public void SetElementMetadata(ModelMetadata value) => _elementMetadata = value;
        public override ModelMetadata ElementMetadata => _elementMetadata;

        public void SetEnumGroupedDisplayNamesAndValues(IEnumerable<KeyValuePair<EnumGroupAndName, string>> value) => _enumGroupedDisplayNamesAndValues = value;
        public override IEnumerable<KeyValuePair<EnumGroupAndName, string>> EnumGroupedDisplayNamesAndValues => _enumGroupedDisplayNamesAndValues;

        public void SetEnumNamesAndValues(IReadOnlyDictionary<string, string> value) => _enumNamesAndValues = value;
        public override IReadOnlyDictionary<string, string> EnumNamesAndValues => _enumNamesAndValues;

        public void SetHasNonDefaultEditFormat(bool value) => _hasNonDefaultEditFormat = value;
        public override bool HasNonDefaultEditFormat => _hasNonDefaultEditFormat;

        public void SetHtmlEncode(bool value) => _htmlEncode = value;
        public override bool HtmlEncode => _htmlEncode;

        public void SetHideSurroundingHtml(bool value) => _hideSurroundingHtml = value;
        public override bool HideSurroundingHtml => _hideSurroundingHtml;

        public void SetIsBindingAllowed(bool value) => _isBindingAllowed = value;
        public override bool IsBindingAllowed => _isBindingAllowed;

        public void SetIsBindingRequired(bool value) => _isBindingRequired = value;
        public override bool IsBindingRequired => _isBindingRequired;

        public void SetIsEnum(bool value) => _isEnum = value;
        public override bool IsEnum => _isEnum;

        public void SetIsFlagsEnum(bool value) => _isFlagsEnum = value;
        public override bool IsFlagsEnum => _isFlagsEnum;

        public void SetIsReadOnly(bool value) => _isReadOnly = value;
        public override bool IsReadOnly => _isReadOnly;

        public void SetIsRequired(bool value) => _isRequired = value;
        public override bool IsRequired => _isRequired;

        public void SetModelBindingMessageProvider(ModelBindingMessageProvider value) => _modelBindingMessageProvider = value;
        public override ModelBindingMessageProvider ModelBindingMessageProvider => _modelBindingMessageProvider;

        public void SetOrder(int value) => _order = value;
        public override int Order => _order;

        public void SetPlaceholder(string value) => _placeholder = value;
        public override string Placeholder => _placeholder;

        public void SetNullDisplayText(string value) => _nullDisplayText = value;
        public override string NullDisplayText => _nullDisplayText;

        public void SetPropertyFilterProvider(IPropertyFilterProvider value) => _propertyFilterProvider = value;
        public override IPropertyFilterProvider PropertyFilterProvider => _propertyFilterProvider;

        public void SetShowForDisplay(bool value) => _showForDisplay = value;
        public override bool ShowForDisplay => _showForDisplay;

        public void SetShowForEdit(bool value) => _showForEdit = value;
        public override bool ShowForEdit => _showForEdit;

        public void SetSimpleDisplayProperty(string value) => _simpleDisplayProperty = value;
        public override string SimpleDisplayProperty => _simpleDisplayProperty;

        public void SetTemplateHint(string value) => _templateHint = value;
        public override string TemplateHint => _templateHint;

        public void SetValidateChildren(bool value) => _validateChildren = value;
        public override bool ValidateChildren => _validateChildren;

        public void SetValidatorMetadata(IReadOnlyList<object> value) => _validatorMetadata = value;
        public override IReadOnlyList<object> ValidatorMetadata => _validatorMetadata;

        public void SetPropertyGetter(Func<object, object> value) => _propertyGetter = value;
        public override Func<object, object> PropertyGetter => _propertyGetter;

        public void SetPropertySetter(Action<object, object> value) => _propertySetter = value;
        public override Action<object, object> PropertySetter => _propertySetter;
    }
}