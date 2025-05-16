using CRM.Domain.Enums;
using CRM.Persistence.Common.Repositories;
using Shouldly;

namespace CRM.Persistence.UnitTests.Tests.Common.Repositories;

public class AccessRequirementsTests
{
    [Fact]
    public void Constructor_WhenCalled_ShouldResetToDefault()
    {
        // Arrange

        // Act
        var accessRequirements = new AccessRequirements();

        // Assert
        accessRequirements.GetRequirment().ShouldBe(GroupRightsEnum.Read);
        accessRequirements.IsSet.ShouldBeFalse();
    }

    [Fact]
    public void SetRequirement_WhenCalled_WithValidValue_ShouldSetRequirementAndMarkIsSetTrue()
    {
        // Arrange
        var accessRequirements = new AccessRequirements();

        // Act
        accessRequirements.SetRequirement(GroupRightsEnum.ReadWrite);

        // Assert
        accessRequirements.GetRequirment().ShouldBe(GroupRightsEnum.ReadWrite);
        accessRequirements.IsSet.ShouldBeTrue();
    }

    [Fact]
    public void SetRequirement_WhenCalled_WithNone_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var accessRequirements = new AccessRequirements();

        // Act
        Action act = () => accessRequirements.SetRequirement(GroupRightsEnum.None);

        // Assert
        InvalidOperationException exception = act.ShouldThrow<InvalidOperationException>();
        exception.Message.ShouldBe("Access Requirement 'None' is invalid");
    }

    [Fact]
    public void Reset_WhenCalled_ShouldSetRequirementToReadAndMarkIsSetFalse()
    {
        // Arrange
        var accessRequirements = new AccessRequirements();

        accessRequirements.SetRequirement(GroupRightsEnum.ReadWrite);
        accessRequirements.IsSet.ShouldBeTrue();

        // Act
        accessRequirements.Reset();

        // Assert
        accessRequirements.GetRequirment().ShouldBe(GroupRightsEnum.Read);
        accessRequirements.IsSet.ShouldBeFalse();
    }
}
