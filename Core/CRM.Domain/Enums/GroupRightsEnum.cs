using System.ComponentModel.DataAnnotations;

namespace CRM.Domain.Enums;

[Flags]
public enum GroupRightsEnum
{
    [Display(Name = "None")]
    None = 0,

    [Display(Name = "Read")]
    Read = 1,

    [Display(Name = "Read & Write")]
    ReadWrite = Read | 2,

    [Display(Name = "Owner")]
    Owner = ReadWrite | 4
}
