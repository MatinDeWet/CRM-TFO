using CRM.Application.Common.Security.Contracts;
using CRM.Domain.Enums;
using CRM.Persistence.Common.Repositories;
using CRM.Persistence.Common.Repositories.Enums;
using Microsoft.EntityFrameworkCore;
using Moq;
using Shouldly;

namespace CRM.Persistence.UnitTests.Tests.Common.Repositories;
public class JudgedCommandsDummyEntity { }

public class JudgedCommandsTests
{
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private Mock<DbContext> CreateDbContextMock()
    {
        DbContextOptions<DbContext> options = new DbContextOptionsBuilder<DbContext>().Options;
        var dbContextMock = new Mock<DbContext>(options);

        dbContextMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        return dbContextMock;
    }

    [Fact]
    public async Task InsertAsync_WhenAccessGrantedForSuperAdmin_ShouldCallAddWithoutProtectedCheck()
    {
        // Arrange
        Mock<DbContext> dbContextMock = CreateDbContextMock();
        var identityMock = new Mock<IIdentityInfo>();

        identityMock.Setup(x => x.HasRole(ApplicationRoleEnum.SuperAdmin)).Returns(true);
        identityMock.Setup(x => x.GetIdentityId()).Returns(999);

        var requirements = new AccessRequirements();

        var protections = new List<IProtected>();

        var judgedCommands = new JudgedCommands<DbContext>(
            dbContextMock.Object,
            identityMock.Object,
            requirements,
            protections);
        var dummy = new JudgedCommandsDummyEntity();

        // Act
        await judgedCommands.InsertAsync(dummy, _cancellationToken);

        // Assert
        dbContextMock.Verify(x => x.Add(dummy), Times.Once);

        requirements.IsSet.ShouldBeFalse();

        requirements.GetRequirment().ShouldBe(GroupRightsEnum.Read);
    }

    [Fact]
    public async Task InsertAsync_WhenAccessGrantedViaProtection_ShouldCallAdd()
    {
        // Arrange
        Mock<DbContext> dbContextMock = CreateDbContextMock();
        var identityMock = new Mock<IIdentityInfo>();

        identityMock.Setup(x => x.HasRole(ApplicationRoleEnum.SuperAdmin)).Returns(false);
        identityMock.Setup(x => x.GetIdentityId()).Returns(100);

        var requirements = new AccessRequirements();

        var protectedMock = new Mock<IProtected<JudgedCommandsDummyEntity>>();
        protectedMock.Setup(x => x.IsMatch(typeof(JudgedCommandsDummyEntity))).Returns(true);

        protectedMock
            .Setup(x => x.HasAccess(
                It.IsAny<JudgedCommandsDummyEntity>(),
                It.IsAny<int>(),
                RepositoryOperationEnum.Insert,
                It.IsAny<GroupRightsEnum>(),
                _cancellationToken))
            .ReturnsAsync(true);

        var protections = new List<IProtected> { protectedMock.Object };

        var judgedCommands = new JudgedCommands<DbContext>(
            dbContextMock.Object,
            identityMock.Object,
            requirements,
            protections);
        var dummy = new JudgedCommandsDummyEntity();

        // Act
        await judgedCommands.InsertAsync(dummy, _cancellationToken);

        // Assert
        dbContextMock.Verify(x => x.Add(dummy), Times.Once);

        requirements.IsSet.ShouldBeFalse();

        requirements.GetRequirment().ShouldBe(GroupRightsEnum.Read);

        protectedMock.Verify(x => x.HasAccess(
            dummy,
            100,
            RepositoryOperationEnum.Insert,
            It.IsAny<GroupRightsEnum>(),
            _cancellationToken), Times.Once);
    }

    [Fact]
    public async Task InsertAsync_WhenAccessDenied_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        Mock<DbContext> dbContextMock = CreateDbContextMock();
        var identityMock = new Mock<IIdentityInfo>();
        identityMock.Setup(x => x.HasRole(ApplicationRoleEnum.SuperAdmin)).Returns(false);
        identityMock.Setup(x => x.GetIdentityId()).Returns(50);

        var requirements = new AccessRequirements();

        var protectedMock = new Mock<IProtected<JudgedCommandsDummyEntity>>();
        protectedMock.Setup(x => x.IsMatch(typeof(JudgedCommandsDummyEntity))).Returns(true);
        protectedMock
            .Setup(x => x.HasAccess(
                It.IsAny<JudgedCommandsDummyEntity>(),
                It.IsAny<int>(),
                RepositoryOperationEnum.Insert,
                It.IsAny<GroupRightsEnum>(),
                _cancellationToken))
            .ReturnsAsync(false);

        var protections = new List<IProtected> { protectedMock.Object };

        var judgedCommands = new JudgedCommands<DbContext>(
            dbContextMock.Object,
            identityMock.Object,
            requirements,
            protections);
        var dummy = new JudgedCommandsDummyEntity();

        // Act
        Func<Task> act = async () => await judgedCommands.InsertAsync(dummy, _cancellationToken);

        // Assert
        await act.ShouldThrowAsync<UnauthorizedAccessException>();

        dbContextMock.Verify(x => x.Add(It.IsAny<JudgedCommandsDummyEntity>()), Times.Never);

        requirements.IsSet.ShouldBeFalse();

        requirements.GetRequirment().ShouldBe(GroupRightsEnum.Read);

        protectedMock.Verify(x => x.HasAccess(
            dummy,
            50,
            RepositoryOperationEnum.Insert,
            It.IsAny<GroupRightsEnum>(),
            _cancellationToken), Times.Once);
    }

    [Fact]
    public async Task InsertAsync_WithPersistImmediately_WhenAccessGranted_ShouldCallSaveChangesAsync()
    {
        // Arrange
        Mock<DbContext> dbContextMock = CreateDbContextMock();
        var identityMock = new Mock<IIdentityInfo>();

        identityMock.Setup(x => x.HasRole(ApplicationRoleEnum.SuperAdmin)).Returns(true);
        identityMock.Setup(x => x.GetIdentityId()).Returns(123);

        var requirements = new AccessRequirements();

        var protections = new List<IProtected>();

        var judgedCommands = new JudgedCommands<DbContext>(
            dbContextMock.Object,
            identityMock.Object,
            requirements,
            protections);
        var dummy = new JudgedCommandsDummyEntity();

        // Act
        await judgedCommands.InsertAsync(dummy, persistImmediately: true, cancellationToken: _cancellationToken);

        // Assert
        dbContextMock.Verify(x => x.Add(dummy), Times.Once);

        dbContextMock.Verify(x => x.SaveChangesAsync(_cancellationToken), Times.Once);

        requirements.IsSet.ShouldBeFalse();

        requirements.GetRequirment().ShouldBe(GroupRightsEnum.Read);
    }

    [Fact]
    public async Task UpdateAsync_WhenAccessGranted_ShouldCallUpdate()
    {
        // Arrange
        Mock<DbContext> dbContextMock = CreateDbContextMock();
        var identityMock = new Mock<IIdentityInfo>();

        identityMock.Setup(x => x.HasRole(ApplicationRoleEnum.SuperAdmin)).Returns(false);
        identityMock.Setup(x => x.GetIdentityId()).Returns(200);

        var requirements = new AccessRequirements();

        var protectedMock = new Mock<IProtected<JudgedCommandsDummyEntity>>();
        protectedMock.Setup(x => x.IsMatch(typeof(JudgedCommandsDummyEntity))).Returns(true);
        protectedMock
            .Setup(x => x.HasAccess(
                It.IsAny<JudgedCommandsDummyEntity>(),
                It.IsAny<int>(),
                RepositoryOperationEnum.Update,
                It.IsAny<GroupRightsEnum>(),
                _cancellationToken))
            .ReturnsAsync(true);

        var protections = new List<IProtected> { protectedMock.Object };

        var judgedCommands = new JudgedCommands<DbContext>(
            dbContextMock.Object,
            identityMock.Object,
            requirements,
            protections);
        var dummy = new JudgedCommandsDummyEntity();

        // Act
        await judgedCommands.UpdateAsync(dummy, _cancellationToken);

        // Assert
        dbContextMock.Verify(x => x.Update(dummy), Times.Once);

        requirements.IsSet.ShouldBeFalse();

        requirements.GetRequirment().ShouldBe(GroupRightsEnum.Read);

        protectedMock.Verify(x => x.HasAccess(
            dummy,
            200,
            RepositoryOperationEnum.Update,
            It.IsAny<GroupRightsEnum>(),
            _cancellationToken), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenAccessDenied_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        Mock<DbContext> dbContextMock = CreateDbContextMock();
        var identityMock = new Mock<IIdentityInfo>();
        identityMock.Setup(x => x.HasRole(ApplicationRoleEnum.SuperAdmin)).Returns(false);
        identityMock.Setup(x => x.GetIdentityId()).Returns(150);

        var requirements = new AccessRequirements();

        var protectedMock = new Mock<IProtected<JudgedCommandsDummyEntity>>();
        protectedMock.Setup(x => x.IsMatch(typeof(JudgedCommandsDummyEntity))).Returns(true);
        protectedMock
            .Setup(x => x.HasAccess(
                It.IsAny<JudgedCommandsDummyEntity>(),
                It.IsAny<int>(),
                RepositoryOperationEnum.Update,
                It.IsAny<GroupRightsEnum>(),
                _cancellationToken))
            .ReturnsAsync(false);

        var protections = new List<IProtected> { protectedMock.Object };

        var judgedCommands = new JudgedCommands<DbContext>(
            dbContextMock.Object,
            identityMock.Object,
            requirements,
            protections);
        var dummy = new JudgedCommandsDummyEntity();

        // Act
        Func<Task> act = async () => await judgedCommands.UpdateAsync(dummy, _cancellationToken);

        // Assert
        await act.ShouldThrowAsync<UnauthorizedAccessException>();

        dbContextMock.Verify(x => x.Update(It.IsAny<JudgedCommandsDummyEntity>()), Times.Never);

        requirements.IsSet.ShouldBeFalse();

        requirements.GetRequirment().ShouldBe(GroupRightsEnum.Read);

        protectedMock.Verify(x => x.HasAccess(
            dummy,
            150,
            RepositoryOperationEnum.Update,
            It.IsAny<GroupRightsEnum>(),
            _cancellationToken), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenAccessGranted_ShouldCallRemove()
    {
        // Arrange
        Mock<DbContext> dbContextMock = CreateDbContextMock();
        var identityMock = new Mock<IIdentityInfo>();
        identityMock.Setup(x => x.HasRole(ApplicationRoleEnum.SuperAdmin)).Returns(false);
        identityMock.Setup(x => x.GetIdentityId()).Returns(300);

        var requirements = new AccessRequirements();

        var protectedMock = new Mock<IProtected<JudgedCommandsDummyEntity>>();
        protectedMock.Setup(x => x.IsMatch(typeof(JudgedCommandsDummyEntity))).Returns(true);
        protectedMock
            .Setup(x => x.HasAccess(
                It.IsAny<JudgedCommandsDummyEntity>(),
                It.IsAny<int>(),
                RepositoryOperationEnum.Delete,
                It.IsAny<GroupRightsEnum>(),
                _cancellationToken))
            .ReturnsAsync(true);

        var protections = new List<IProtected> { protectedMock.Object };

        var judgedCommands = new JudgedCommands<DbContext>(
            dbContextMock.Object,
            identityMock.Object,
            requirements,
            protections);
        var dummy = new JudgedCommandsDummyEntity();

        // Act
        await judgedCommands.DeleteAsync(dummy, _cancellationToken);

        // Assert
        dbContextMock.Verify(x => x.Remove(dummy), Times.Once);

        requirements.IsSet.ShouldBeFalse();

        requirements.GetRequirment().ShouldBe(GroupRightsEnum.Read);

        protectedMock.Verify(x => x.HasAccess(
            dummy,
            300,
            RepositoryOperationEnum.Delete,
            It.IsAny<GroupRightsEnum>(),
            _cancellationToken), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenAccessDenied_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        Mock<DbContext> dbContextMock = CreateDbContextMock();
        var identityMock = new Mock<IIdentityInfo>();
        identityMock.Setup(x => x.HasRole(ApplicationRoleEnum.SuperAdmin)).Returns(false);
        identityMock.Setup(x => x.GetIdentityId()).Returns(400);

        var requirements = new AccessRequirements();

        var protectedMock = new Mock<IProtected<JudgedCommandsDummyEntity>>();
        protectedMock.Setup(x => x.IsMatch(typeof(JudgedCommandsDummyEntity))).Returns(true);
        protectedMock
            .Setup(x => x.HasAccess(
                It.IsAny<JudgedCommandsDummyEntity>(),
                It.IsAny<int>(),
                RepositoryOperationEnum.Delete,
                It.IsAny<GroupRightsEnum>(),
                _cancellationToken))
            .ReturnsAsync(false);

        var protections = new List<IProtected> { protectedMock.Object };

        var judgedCommands = new JudgedCommands<DbContext>(
            dbContextMock.Object,
            identityMock.Object,
            requirements,
            protections);
        var dummy = new JudgedCommandsDummyEntity();

        // Act
        Func<Task> act = async () => await judgedCommands.DeleteAsync(dummy, _cancellationToken);

        // Assert
        await act.ShouldThrowAsync<UnauthorizedAccessException>();

        dbContextMock.Verify(x => x.Remove(It.IsAny<JudgedCommandsDummyEntity>()), Times.Never);

        requirements.IsSet.ShouldBeFalse();

        requirements.GetRequirment().ShouldBe(GroupRightsEnum.Read);

        protectedMock.Verify(x => x.HasAccess(
            dummy,
            400,
            RepositoryOperationEnum.Delete,
            It.IsAny<GroupRightsEnum>(),
            _cancellationToken), Times.Once);
    }

    [Fact]
    public async Task SaveAsync_WhenCalled_ShouldCallSaveChangesAsync()
    {
        // Arrange
        Mock<DbContext> dbContextMock = CreateDbContextMock();
        var identityMock = new Mock<IIdentityInfo>();

        var requirements = new AccessRequirements();
        var protections = new List<IProtected>();

        var judgedCommands = new JudgedCommands<DbContext>(
            dbContextMock.Object,
            identityMock.Object,
            requirements,
            protections);

        // Act
        await judgedCommands.SaveAsync(_cancellationToken);

        // Assert
        dbContextMock.Verify(x => x.SaveChangesAsync(_cancellationToken), Times.Once);
    }
}
