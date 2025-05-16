namespace CRM.Domain.Enums;


[Flags]
public enum ApplicationRoleEnum
{
    None = 0,
    Admin = 1,
    SuperAdmin = Admin | 2
}
