using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Data.Enteties;

[Index(nameof(ClientName), IsUnique = true)]

public class ClientEntity
{
    [Key]

    public string Id { get; set; } = null!;

    public string ClientName { get; set; } = null!;


    public virtual ICollection<ProjectEntity> Projects { get; set; } = [];
}
