namespace FlowerShop.Tests;

public class AuthServiceTests
{
    private readonly InMemoryRepository<StaffMember> _staffRepo = new();
    private readonly AuthService _service;

    public AuthServiceTests()
    {
        _service = new AuthService(_staffRepo);

        _staffRepo.Add(new StaffMember { Username = "owner", PasswordHash = "owner",   Name = "Grace", Role = Role.Owner });
        _staffRepo.Add(new StaffMember { Username = "florist",   PasswordHash = "florist", Name = "maria",   Role = Role.Florist });
        _staffRepo.Add(new StaffMember { Username = "driver", PasswordHash = "driver",  Name = "john", Role = Role.Driver });
    }

    [Fact]
    public void TryLogin_ValidCredentials_ReturnsStaffMember()
    {
        var user = _service.TryLogin("owner", "owner");

        Assert.NotNull(user);
        Assert.Equal("Grace",     user!.Name);
        Assert.Equal(Role.Owner,  user.Role);
    }

    [Fact]
    public void TryLogin_WrongPassword_ReturnsNull()
    {
        var user = _service.TryLogin("owner", "wrong");

        Assert.Null(user);
    }

    [Fact]
    public void TryLogin_UnknownUsername_ReturnsNull()
    {
        var user = _service.TryLogin("nobody", "anything");

        Assert.Null(user);
    }

    [Fact]
    public void TryLogin_PasswordIsCaseSensitive()
    {
        // PasswordHash is "owner"; passing "Owner" should not match.
        var user = _service.TryLogin("owner", "Owner");

        Assert.Null(user);
    }

    [Fact]
    public void TryLogin_EmptyPassword_ReturnsNull()
    {
        var user = _service.TryLogin("owner", "");

        Assert.Null(user);
    }

    [Fact]
    public void TryLogin_EmptyUsername_ReturnsNull()
    {
        var user = _service.TryLogin("", "owner");

        Assert.Null(user);
    }

    [Fact]
    public void TryLogin_AllThreeDemoAccounts_Succeed()
    {
        var owner   = _service.TryLogin("owner", "owner");
        var florist = _service.TryLogin("florist",   "florist");
        var driver  = _service.TryLogin("driver", "driver");

        Assert.NotNull(owner);
        Assert.Equal(Role.Owner,   owner!.Role);

        Assert.NotNull(florist);
        Assert.Equal(Role.Florist, florist!.Role);

        Assert.NotNull(driver);
        Assert.Equal(Role.Driver,  driver!.Role);
    }

    [Fact]
    public void TryLogin_ReturnedUserHasUsernamePreserved()
    {
        var user = _service.TryLogin("florist", "florist");

        Assert.NotNull(user);
        Assert.Equal("florist", user!.Username);
    }
}