//using System;
//using Microsoft.Extensions.Options;

//namespace TrdP.Mvc.DataAnnotations.Localization
//{
//    internal class DataAnnotationsLocalizationOptionsSetup : IConfigureOptions<DataAnnotationsLocalizationOptions>
//    {
//        public void Configure(DataAnnotationsLocalizationOptions options)
//        {
//            if (options == null)
//            {
//                throw new ArgumentNullException(nameof(options));
//            }

//            options.DataAnnotationLocalizerProvider = (modelType, stringLocalizerFactory) => stringLocalizerFactory.Create(modelType);
//        }
//    }
//}