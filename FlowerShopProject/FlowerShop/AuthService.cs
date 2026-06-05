namespace FlowerShop;
public class AuthService
{
    private readonly StaffRepository _staffRepo;

    public AuthService(StaffRepository staffRepo)
    {
        _staffRepo = staffRepo;
    }

    public StaffMember? TryLogin(string username, string password)
    {
        var staff = _staffRepo.FindByUsername(username);
        if (staff is null) return null;
        if (staff.Password != password) return null;
        return staff;
    }
}