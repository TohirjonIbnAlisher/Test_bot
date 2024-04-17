using Npgsql;
using System.Data.Common;
using test_bot.Models;

namespace test_bot.Brokers.QuestionBrokers;

internal class QuestionBroker : IQuestionBroker
{
    public Task<bool> DeleteQuestionAsync(int subjectId)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> InsertQuestionAsync(Question question)
    {
        NpgsqlConnection npgsqlConnection = Helper.GetConnection();

        await npgsqlConnection.OpenAsync();

        using(NpgsqlCommand cmd = new NpgsqlCommand("INSERT INTO quizz_schema.question(text, subject_id, question_level) VALUES (@text, @subject_id, @level);", npgsqlConnection))
        {
            cmd.Parameters.AddWithValue("@text", question.Text);
            cmd.Parameters.AddWithValue("@subject_id", question.SubjectId);
            cmd.Parameters.AddWithValue("@level", (int)question.Level);

            int insertedRows = await cmd.ExecuteNonQueryAsync();

            await npgsqlConnection.CloseAsync();

            return insertedRows > 0;
        }
    }

    public async Task<List<Question>> SelectQuestionsBySubjectIdAsync(int subjectId)
    {
        NpgsqlConnection npgsqlConnection = Helper.GetConnection();

        await npgsqlConnection.OpenAsync();

        using (NpgsqlCommand cmd = new NpgsqlCommand("select * quizz_schema.question where subject_id = @subject_id", npgsqlConnection))
        {
                cmd.Parameters.AddWithValue(@"subject_id", subjectId);

                NpgsqlDataReader storedQuestions = await cmd.ExecuteReaderAsync();

                List<Question> questions = new List<Question>();
                while (await storedQuestions.ReadAsync())
                {
                    questions.Add(
                        new Question()
                        {
                            Id = storedQuestions.GetInt32(0),
                            Text = storedQuestions.GetString(1),
                            SubjectId = storedQuestions.GetInt32(2),
                            Level = (Level)storedQuestions.GetInt32(3),
                        }
                    );
                }

                await npgsqlConnection.CloseAsync();
                return questions;
            }
    }

    public Task<bool> UpdateQuestionAsync(Question question)
    {
        throw new NotImplementedException();
    }
}
