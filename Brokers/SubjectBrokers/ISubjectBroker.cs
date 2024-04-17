using test_bot.Models;

namespace test_bot.Brokers.SubjectBrokers;

internal interface ISubjectBroker
{
    Task<bool> InsertSubjectAsync(string name);
    Task<bool> DeleteSubjectAsync(int subjectId);
    Task<List<Subject>> SelectSubjectAsync();

}
