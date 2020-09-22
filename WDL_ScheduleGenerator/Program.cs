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
            LeagueSchedule ls = null;
            Random rng = null;
            bool scheduleComplete = false;
            int overallAttemptCounter = 1;            

            while(!scheduleComplete)
            {
                rng = new Random();
                ls = new LeagueSchedule();
                scheduleComplete = ls.GenerateLeagueSchedule(rng);
                overallAttemptCounter++;
            }
            Console.WriteLine($"\nCOMPLETED ENTIRE SCHEDULE in {overallAttemptCounter} attempts.\n\n");

            ls.DisplayLeagueSchedule();
        }
    }
}
