namespace FlowerShop;

public class StaffRepository
{
    private readonly List<StaffMember> _staff = new()
    {
        new StaffMember { Username = "owner", Password = "owner",   Name = "Owner", Role = Role.Owner },
        new StaffMember { Username = "florist",   Password = "florist", Name = "Florist",   Role = Role.Florist },
        new StaffMember { Username = "driver", Password = "driver",  Name = "Driver", Role = Role.Driver }
    };

    public StaffMember? FindByUsername(string username)
    {
        return _staff.FirstOrDefault(
            s => s.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
    }
}