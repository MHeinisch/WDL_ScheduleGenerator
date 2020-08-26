using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WDL_ScheduleGenerator
{
    class Team
    {
        public int teamNumber;
        public int numberWithinDivision;
        public string managerName;
        public SortedDictionary<int, Team> weekNumberByOpposingTeam;

        public Team(int teamNumber, string managerName)
        {
            this.teamNumber = teamNumber;
            this.numberWithinDivision = teamNumber % 3 != 0 ? teamNumber % 3 : 3;
            this.managerName = managerName;

            weekNumberByOpposingTeam = new SortedDictionary<int, Team>();

            for (int i = 1; i <= 13; i++)
            {
                weekNumberByOpposingTeam.Add(i, null);
            }
        }

        public void SetDivisionalMatchups(Dictionary<int, Team> allTeams)
        {
            int multiplier = (teamNumber - 1) / 3;
            int adder = 3 * multiplier + 1;

            //Team firstDivisionalOpponent = allTeams[1 + adder];
            //Team secondDivisionalOpponent = allTeams[2 + adder];

            Team firstDivisionalOpponent;
            Team secondDivisionalOpponent;

            // set division matches
            if (numberWithinDivision == 1)
            {
                firstDivisionalOpponent = allTeams[1 + adder];
                secondDivisionalOpponent = allTeams[2 + adder];

                // week 1, team 2
                weekNumberByOpposingTeam[1] = firstDivisionalOpponent;
                // week 2, team 3
                weekNumberByOpposingTeam[2] = secondDivisionalOpponent;

                // week 12, team 2
                weekNumberByOpposingTeam[12] = firstDivisionalOpponent;
                // week 13, team 3
                weekNumberByOpposingTeam[13] = secondDivisionalOpponent;
            }
            else if (numberWithinDivision == 2)
            {
                firstDivisionalOpponent = allTeams[adder];
                secondDivisionalOpponent = allTeams[2 + adder];

                // week 1, team 1
                weekNumberByOpposingTeam[1] = firstDivisionalOpponent;
                // week 3, team 3
                weekNumberByOpposingTeam[3] = secondDivisionalOpponent;

                // week 11, team 3
                weekNumberByOpposingTeam[11] = secondDivisionalOpponent;
                // week 12, team 1
                weekNumberByOpposingTeam[12] = firstDivisionalOpponent;
            }
            else
            {
                firstDivisionalOpponent = allTeams[adder];
                secondDivisionalOpponent = allTeams[1 + adder];

                // week 2, team 1
                weekNumberByOpposingTeam[2] = firstDivisionalOpponent;
                // week 3, team 2
                weekNumberByOpposingTeam[3] = secondDivisionalOpponent;

                // week 11, team 2
                weekNumberByOpposingTeam[11] = secondDivisionalOpponent;
                // week 13, team 1
                weekNumberByOpposingTeam[13] = firstDivisionalOpponent;
            }
        }

        public void GenerateRemainingRandomMatchups(Dictionary<int, Team> allTeams)
        {
            Random rng = new Random();
            List<int> existingMatchupTeamNumbers = GetListOfTeamNumbersThatAlreadyHaveMatchups();
            List<int> teamNumbersToAddForMatchups = GetListOfTeamNumbersToAddMatchupsFor(allTeams, existingMatchupTeamNumbers);

            // iterate through weekNumberByOpposingTeam
            // if .Value == null...
                // pick random teamNumber from teamNumbersToAddForMatchups
                // add matchup to this team's schedule
                // add corresponding matchup to opposing team's schedule
                // remove selected randomNumber from teamNumbersToAddForMatchups

            int weekCounter = 1;
            foreach(KeyValuePair<int, Team> team in weekNumberByOpposingTeam)
            {
                if(team.Value == null)
                {
                    int randomTeamNumberIndex = rng.Next(teamNumbersToAddForMatchups.Count);
                    team.Value.weekNumberByOpposingTeam[weekCounter] = allTeams[teamNumbersToAddForMatchups[randomTeamNumberIndex]];
                    allTeams[teamNumbersToAddForMatchups[randomTeamNumberIndex]].weekNumberByOpposingTeam[weekCounter] = this;
                }
                weekCounter++;
            }


        }

        private List<int> GetListOfTeamNumbersThatAlreadyHaveMatchups()
        {
            List<int> existingMatchupTeamNumbers = new List<int>() { teamNumber };

            foreach (KeyValuePair<int, Team> team in weekNumberByOpposingTeam)
            {
                if (team.Value != null)
                {
                    if (!existingMatchupTeamNumbers.Contains(team.Value.teamNumber))
                    {
                        existingMatchupTeamNumbers.Add(team.Value.teamNumber);
                    }
                }
            }

            return existingMatchupTeamNumbers;
        }

        private List<int> GetListOfTeamNumbersToAddMatchupsFor(Dictionary<int, Team> allTeams, List<int> existingMatchupTeamNumbers)
        {
            List<int> remainingMatchupsAsTeamNumbers = new List<int>();

            for (int i = 1; i <= allTeams.Count; i++)
            {
                if (!existingMatchupTeamNumbers.Contains(i))
                {
                    remainingMatchupsAsTeamNumbers.Add(i);
                }
            }

            return remainingMatchupsAsTeamNumbers;
        }
    }
}
