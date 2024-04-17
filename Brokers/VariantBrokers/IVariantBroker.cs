using System.Threading.Tasks;
using test_bot.Models;

namespace test_bot.Brokers.VariantBrokers;

internal interface IVariantBroker
{
    Task<bool> InsertVariantAsync(Variant variant);
    Task<bool> DeleteVariantAsync(int variantid);
    Task<bool> UpdateVariantAsync(Variant variant);
    Task<List<Question>> SelectVariantsAsync();
}
