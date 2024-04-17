using test_bot.Models;

namespace test_bot.Brokers.QuestionBrokers;

internal interface IQuestionBroker
{
    Task<bool> InsertQuestionAsync(Question question);
    Task<bool> DeleteQuestionAsync(int questionId);
    Task<bool> UpdateQuestionAsync(Question question);
    Task<List<Question>> SelectQuestionsBySubjectIdAsync(int subjectId);


}
