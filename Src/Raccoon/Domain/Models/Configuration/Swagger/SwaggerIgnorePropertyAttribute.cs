using System;

namespace Domain.Models.Configuration
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SwaggerIgnorePropertyAttribute : Attribute
    {
    }
}
