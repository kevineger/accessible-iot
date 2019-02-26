using Microsoft.Extensions.Options;

public interface IMapsRepository {
    // TODO
}

public class MapsRepository : IMapsRepository
{
    private string subscriptionKey;

    public MapsRepository(IOptions<AzureMapsOptions> options)
    {
        subscriptionKey =  options.Value.SubscriptionKey;
    }
}