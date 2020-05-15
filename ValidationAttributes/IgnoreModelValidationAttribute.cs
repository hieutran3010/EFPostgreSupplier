namespace EFPostgresEngagement.ValidationAttributes
{
    using System;

    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoreModelValidationAttribute : Attribute
    {
    }
}
