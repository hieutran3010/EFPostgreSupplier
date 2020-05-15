namespace EFPostgresEngagement.DataAnnotationAttributes
{
    using System;

    public class UniqueIndexAttribute : Attribute
    {
        public string AdditionalColumns { get; set; }
    }
}