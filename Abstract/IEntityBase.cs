namespace EFPostgresEngagement.Abstract
{
    using System;

    public interface IEntityBase
    {
        Guid Id { get; set; }
        DateTimeOffset CreatedOn { get; set; }
        string CreatedBy { get; set; }
        DateTimeOffset ModifiedOn { get; set; }
        string ModifiedBy { get; set; }
    }
}