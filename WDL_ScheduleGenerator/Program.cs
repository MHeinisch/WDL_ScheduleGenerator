using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WDL_ScheduleGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            LeagueSchedule ls = new LeagueSchedule();
            ls.GenerateLeagueSchedule();
        }
    }
}
