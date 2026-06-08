namespace FlowerShop;
public class ArrangementRepository : JsonRepository<Arrangement>
{
    public ArrangementRepository() : base("arrangements.json") { }
}