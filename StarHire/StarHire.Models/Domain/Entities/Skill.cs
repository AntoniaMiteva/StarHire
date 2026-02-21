using StarHire.Models.Domain.Entities;
using System.ComponentModel.DataAnnotations;

public class Skill
{
    public Guid Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string NormalizedName { get; set; } = string.Empty;

    public ICollection<UserSkill> UserSkills { get; set; } = new List<UserSkill>();
}