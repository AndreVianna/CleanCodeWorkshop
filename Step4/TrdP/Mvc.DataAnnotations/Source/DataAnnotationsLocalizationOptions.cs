using System;
using TrdP.Localization.Abstractions;

namespace TrdP.Mvc.DataAnnotations.Localization
{
    public class DataAnnotationsLocalizationOptions
    {
        public Func<Type, IStringLocalizerFactory, IStringLocalizer> DataAnnotationLocalizerProvider { get; set; }
    }
}