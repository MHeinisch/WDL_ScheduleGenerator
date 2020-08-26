using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WDL_ScheduleGenerator
{
    class LeagueSchedule
    {
        public Dictionary<int, Team> teamNumberByTeamLeagueSchedule;
        private string[] managerNames;

        public LeagueSchedule()
        {
            teamNumberByTeamLeagueSchedule = new Dictionary<int, Team>();
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

            // create each team
                // give them teamNumber, numberWithinDivision, and managerName
                // add teams to leagueSchedule list
            for (int i = 1; i <= 12; i++)
            {
                Team team = new Team(i, managerNames[i - 1]);
                teamNumberByTeamLeagueSchedule.Add(team.teamNumber, team);
            }
        }

        public void GenerateLeagueSchedule()
        {
            // populate division matches for all teams
            foreach (KeyValuePair<int, Team> team in teamNumberByTeamLeagueSchedule)
            {
                team.Value.SetDivisionalMatchups(teamNumberByTeamLeagueSchedule);
            }

            // starting with Team 1, fill in the rest of the schedule
            // repeat for all teams

            foreach (KeyValuePair<int, Team> team in teamNumberByTeamLeagueSchedule)
            {
                team.Value.GenerateRemainingRandomMatchups(teamNumberByTeamLeagueSchedule);
            }
            //for (int i = 0; i < teamNumberByTeamLeagueSchedule.Count; i++)
            //{
            //    teamNumberByTeamLeagueSchedule[i].SetRemainingMatchupsInSchedule(teamNumberByTeamLeagueSchedule);
            //}

        }

    }
}
