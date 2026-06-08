namespace FlowerShop;
public class FlowerRequirementRepository : JsonRepository<FlowerRequirement>
{
    public FlowerRequirementRepository() : base("flower-requirements.json") { }
}