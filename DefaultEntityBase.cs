namespace EFPostgresEngagement
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Abstract;
    using DataAnnotationAttributes;
    using ValidationAttributes;

    public class DefaultEntityBase : IEntityBase
    {
        [Key]
        public Guid Id { get; set; }

        [DateTimeNowDefault]
        public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.Now;

        [Required]
        [IgnoreModelValidation]
        public string CreatedBy { get; set; }

        [DateTimeNowDefault]
        public DateTimeOffset ModifiedOn { get; set; } = DateTimeOffset.Now;

        [Required]
        [IgnoreModelValidation]
        public string ModifiedBy { get; set; }
    }
}