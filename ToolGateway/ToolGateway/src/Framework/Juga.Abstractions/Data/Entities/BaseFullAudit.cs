using Juga.Abstractions.Data.AuditProperties;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Juga.Abstractions.Data.Entities;

public abstract class BaseFullAudit : IHasId, IHasFullAudit
{
    public long Id { get; set; }
    public DateTime CreatedDate { get; set; }

    [MaxLength(64)]
    [Column(TypeName = "varchar(64)")]
    public string CreatedAt { get; set; }

    [MaxLength(64)]
    [Column(TypeName = "varchar(64)")]
    public string CreatedBy { get; set; }

    [MaxLength(64)]
    [Column(TypeName = "varchar(64)")]
    public string CreatedByUserCode { get; set; }

    public DateTime? UpdatedDate { get; set; }

    [MaxLength(64)]
    [Column(TypeName = "varchar(64)")]
    public string UpdatedAt { get; set; }

    [MaxLength(64)]
    [Column(TypeName = "varchar(64)")]
    public string UpdatedBy { get; set; }

    [MaxLength(64)]
    [Column(TypeName = "varchar(64)")]
    public string UpdatedByUserCode { get; set; }
}