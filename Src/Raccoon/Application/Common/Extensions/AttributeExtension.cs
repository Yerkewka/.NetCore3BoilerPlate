﻿using System;
using System.Linq;

namespace Application.Common.Extensions
{
    public static class AttributeExtension
    {
        public static TValue GetAttributeValue<TAttribute, TValue>(this Type type, Func<TAttribute, TValue> valueSelector)
            where TAttribute : Attribute
        {
            if (type.GetCustomAttributes(typeof(TAttribute), true).FirstOrDefault() is TAttribute attr)
                return valueSelector(attr);

            return default;
        }
    }
}
