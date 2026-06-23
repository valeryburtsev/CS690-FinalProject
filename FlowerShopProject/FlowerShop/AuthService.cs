namespace FlowerShop;

public class AuthService
{
    private readonly Repository<StaffMember> _staffRepo;

    public AuthService(Repository<StaffMember> staffRepo)
    {
        _staffRepo = staffRepo;
    }

    public StaffMember? TryLogin(string username, string password)
    {
        var allStaff = _staffRepo.GetAll();

        StaffMember? matched = null;
        foreach (var staff in allStaff)
        {
            if (staff.Username.Equals(username, StringComparison.OrdinalIgnoreCase))
            {
                matched = staff;
                break;
            }
        }

        if (matched == null)
        {
            return null;
        }

        if (matched.PasswordHash != password)
        {
            return null;
        }

        return matched;
    }
}