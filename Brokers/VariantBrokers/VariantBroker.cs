using test_bot.Models;

namespace test_bot.Brokers.VariantBrokers;

internal class VariantBroker : IVariantBroker
{
    public Task<bool> DeleteVariantAsync(int variantid)
    {
        
    }

    public Task<bool> InsertVariantAsync(Variant variant)
    {
        
    }

    public Task<List<Question>> SelectVariantsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateVariantAsync(Variant variant)
    {
        throw new NotImplementedException();
    }
}
