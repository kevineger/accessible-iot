using Microsoft.Extensions.Options;

public interface IAzureMapsRepository {
    // TODO
}

public class AzureMapsRepository : IAzureMapsRepository
{
    private string subscriptionKey;

    public AzureMapsRepository(IOptions<AzureMapsOptions> options)
    {
        subscriptionKey =  options.Value.SubscriptionKey;
    }
}