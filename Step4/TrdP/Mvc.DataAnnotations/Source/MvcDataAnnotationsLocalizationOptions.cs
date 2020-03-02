using System;
using TrdP.Localization.Abstractions;

namespace TrdP.Mvc.DataAnnotations.Localization
{
    public class MvcDataAnnotationsLocalizationOptions
    {
        public Func<Type, IStringLocalizerFactory, IStringLocalizer> DataAnnotationLocalizerProvider { get; set; }
    }
}