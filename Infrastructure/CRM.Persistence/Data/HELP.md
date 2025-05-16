## Add Migration
| Action | Command |
| --- | --- |
| Add-Migration | EntityFrameworkCore\Add-Migration MigrationName -Context CrmContext -Project CRM.Persistence -StartupProject CRM.WebApi -Verbose -o Data/Migrations |

## Remove Migration
| Action | Command |
| --- | --- |
| Remove-Migration | EntityFrameworkCore\Remove-Migration -Context CrmContext -Project CRM.Persistence -StartupProject CRM.WebApi -Verbose |

## Update Database
| Action | Command |
| --- | --- |
| Update-Database | EntityFrameworkCore\Update-Database -Context CrmContext -Project CRM.Persistence -StartupProject CRM.WebApi -Verbose |