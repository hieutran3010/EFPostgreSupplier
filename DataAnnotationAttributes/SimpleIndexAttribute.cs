namespace EFPostgreSupplier.DataAnnotationAttributes
{
    using System;

    public class SimpleIndexAttribute : Attribute
    {
        public string AdditionalColumns { get; set; }
    }
}