using Microsoft.International.Converters.TraditionalChineseToSimplifiedConverter;
using System.Collections.Generic;

namespace ERS.Common
{
    public class FontConversionHelper
    {
        public static List<string> ChangeByList(List<string> data)
        {
            for(int i = 0; i < data.Count; i++)
            {
                if (string.IsNullOrEmpty(data[i])) continue;
                data[i] = ChineseConverter.Convert(data[i], ChineseConversionDirection.TraditionalToSimplified);
                data[i] = data[i].Replace("（", "(").Replace("）", ")");
            }
            return data;
        }
        public static string ChangeBySingle(string data)
        {
            if (string.IsNullOrEmpty(data)) return data;
            return ChineseConverter.Convert(data, ChineseConversionDirection.TraditionalToSimplified).Replace("（", "(").Replace("）", ")");
        }
    }
}
