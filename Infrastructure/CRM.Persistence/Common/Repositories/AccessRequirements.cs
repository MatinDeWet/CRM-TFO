using CRM.Domain.Enums;

namespace CRM.Persistence.Common.Repositories;
public class AccessRequirements
{
    private GroupRightsEnum _requirement;

    public AccessRequirements()
    {
        Reset();
    }

    /// <summary>
    /// Gets a value indicating whether the access requirement is set.
    /// </summary>
    public bool IsSet { get; private set; }

    /// <summary>
    /// Resets the access requirement to the default value (Read) and marks it as not set.
    /// </summary>
    public void Reset()
    {
        SetRequirement(GroupRightsEnum.Read);
        IsSet = false;
    }

    /// <summary>
    /// Sets the access requirement to the specified value.
    /// </summary>
    /// <param name="requirement">The required group rights for access.</param>
    /// <exception cref="InvalidOperationException">Thrown if the requirement is set to None.</exception>
    public void SetRequirement(GroupRightsEnum requirement)
    {
        if (requirement == GroupRightsEnum.None)
        {
            throw new InvalidOperationException("Access Requirement 'None' is invalid");
        }

        _requirement = requirement;
        IsSet = true;
    }

    /// <summary>
    /// Gets the current access requirement.
    /// </summary>
    /// <returns>The current group rights requirement.</returns>
    public GroupRightsEnum GetRequirment() => _requirement;
}
