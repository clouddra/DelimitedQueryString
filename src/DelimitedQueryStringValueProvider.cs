﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Primitives;

namespace DelimitedQueryString
{
    public class DelimitedQueryStringValueProvider : QueryStringValueProvider
    {
        
        private readonly HashSet<string> _keys;
        private readonly string _separator;
        private readonly IQueryCollection _values;

        public DelimitedQueryStringValueProvider(IQueryCollection values, CultureInfo culture)
            : base(null, values, culture)
        {
        }

        public DelimitedQueryStringValueProvider(string key, IQueryCollection values, string separator)
            : this(new List<string> { key }, values, separator)
        {
        }

        public DelimitedQueryStringValueProvider(IEnumerable<string> keys, IQueryCollection values, string separator)
            : base(BindingSource.Query, values, CultureInfo.InvariantCulture)
        {
            _keys = new HashSet<string>(keys);
            _values = values;
            _separator = separator;
        }

        public override ValueProviderResult GetValue(string key)
        {
            var result = base.GetValue(key);

            if (result != ValueProviderResult.None &&
                result.Values.Any(x => x.IndexOf(_separator, StringComparison.OrdinalIgnoreCase) > 0))
            {
                var splitValues = new StringValues(result.Values
                    .SelectMany(x => x.Split(new[] { _separator }, StringSplitOptions.None)).ToArray());

                return new ValueProviderResult(splitValues, result.Culture);
            }

            return result;
        }
    }
}
