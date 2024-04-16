using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using test_bot.Models;

namespace test_bot.Brokers.UserBroker
{
    internal interface IUserBroker
    {
        Task<bool> RegisterUserAsync(TgUser user);
        Task<Role?> LoginAsync(long tgId);
    }
}
