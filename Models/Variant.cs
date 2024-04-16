using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test_bot.Models
{
    internal class Variant
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int QuestionId { get; set; }
        public bool IsCorrect { get; set; }
    }
}
