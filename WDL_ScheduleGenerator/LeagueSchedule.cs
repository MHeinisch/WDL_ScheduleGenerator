using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WDL_ScheduleGenerator
{
    class LeagueSchedule
    {
        public SortedDictionary<int, Team> teamNumberByTeamLeagueSchedule;
        private string[] managerNames;
        // private List<Team> allTeams;

        public LeagueSchedule()
        {
            teamNumberByTeamLeagueSchedule = new SortedDictionary<int, Team>();
            managerNames = new string[]
            {
                "Brian T",
                "Mike T",
                "Mike H",

                "Kyle G",
                "Jake S",
                "Devin R",

                "Natalie K",
                "Andy B",
                "Paul G",

                "Ian J",
                "Jake D",
                "Perronne"
            };

            // allTeams = new List<Team>();
            
            for (int i = 1; i <= 12; i++)
            {
                Team team = new Team(i, managerNames[i - 1]);
                // allTeams.Add(team);

                teamNumberByTeamLeagueSchedule.Add(team.teamNumber, team);
            }
        }

        public bool GenerateLeagueSchedule(Random rng)
        {
            Console.WriteLine("Starting league schedule generation...");

            foreach (KeyValuePair<int, Team> team in teamNumberByTeamLeagueSchedule)
            {
                team.Value.SetDivisionalMatchups(teamNumberByTeamLeagueSchedule);
            }

            bool continuedSuccess = true;

            foreach (KeyValuePair<int, Team> team in teamNumberByTeamLeagueSchedule)
            {
                SortedDictionary<int, int?> generatedScheduleForCurrentTeam = team.Value.GenerateRemainingRandomMatchups(teamNumberByTeamLeagueSchedule, rng);

                // confirm generated schedule is valid, and react if so
                continuedSuccess = !generatedScheduleForCurrentTeam.Values.Contains(null);

                if(continuedSuccess)
                {
                    // assign returned team to appropriate thing in leagueSchedule                    
                    teamNumberByTeamLeagueSchedule[team.Value.teamNumber].weekNumberByOpposingTeamNumber = generatedScheduleForCurrentTeam;

                    // iterate over that returned team, and apply all matchups to existing leagueSchedule
                    int weekCounter = 1;
                    foreach(KeyValuePair<int, int?> generatedMatchup in generatedScheduleForCurrentTeam)
                    {                      
                        Team teamInQuestion = teamNumberByTeamLeagueSchedule[(int)generatedMatchup.Value];
                        
                        if (!teamInQuestion.weekNumberByOpposingTeamNumber[generatedMatchup.Key].HasValue)
                        {
                            teamInQuestion.weekNumberByOpposingTeamNumber[weekCounter] = team.Value.teamNumber;
                        }

                        weekCounter++;
                    }
                }
                else
                {
                    Console.WriteLine("RESTARTING...\n\n");
                    return false;
                }
            }

            return true;
        }

        public void DisplayLeagueSchedule()
        {
            foreach(KeyValuePair<int, Team> team in teamNumberByTeamLeagueSchedule)
            {
                team.Value.DisplaySchedule(teamNumberByTeamLeagueSchedule);
            }

            Console.ReadLine();
        }
    }
}
