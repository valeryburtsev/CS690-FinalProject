namespace FlowerShop;

public class StaffRepository : JsonRepository<StaffMember>
{
    public StaffRepository() : base("staff.json")
    {
        SeedIfEmpty(new[]
        {
            new StaffMember { Username = "owner", PasswordHash = "owner",   Name = "Grace", Role = Role.Owner },
            new StaffMember { Username = "florist",   PasswordHash = "florist", Name = "Sam",   Role = Role.Florist },
            new StaffMember { Username = "driver", PasswordHash = "driver",  Name = "Marco", Role = Role.Driver }
        });
    }

    public StaffMember? FindByUsername(string username)
    {
        return GetAll().FirstOrDefault(
            s => s.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
    }
}