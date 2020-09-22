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
        public SortedDictionary<int, int?> weekNumberByOpposingTeamNumber;

        public Team(int teamNumber, string managerName)
        {
            this.teamNumber = teamNumber;
            this.numberWithinDivision = teamNumber % 3 != 0 ? teamNumber % 3 : 3;
            this.managerName = managerName;

            weekNumberByOpposingTeamNumber = new SortedDictionary<int, int?>();

            for (int i = 1; i <= 13; i++)
            {
                weekNumberByOpposingTeamNumber.Add(i, null);
            }
        }

        public void SetDivisionalMatchups(SortedDictionary<int, Team> allTeams)
        {
            int multiplier = (teamNumber - 1) / 3;
            int adder = 3 * multiplier + 1;


            Team firstDivisionalOpponent;
            Team secondDivisionalOpponent;

            if (numberWithinDivision == 1)
            {
                firstDivisionalOpponent = allTeams[1 + adder];
                secondDivisionalOpponent = allTeams[2 + adder];

                weekNumberByOpposingTeamNumber[1] = firstDivisionalOpponent.teamNumber;
                weekNumberByOpposingTeamNumber[2] = secondDivisionalOpponent.teamNumber;
                weekNumberByOpposingTeamNumber[12] = firstDivisionalOpponent.teamNumber;
                weekNumberByOpposingTeamNumber[13] = secondDivisionalOpponent.teamNumber;
            }
            else if (numberWithinDivision == 2)
            {
                firstDivisionalOpponent = allTeams[adder];
                secondDivisionalOpponent = allTeams[2 + adder];

                weekNumberByOpposingTeamNumber[1] = firstDivisionalOpponent.teamNumber;
                weekNumberByOpposingTeamNumber[3] = secondDivisionalOpponent.teamNumber;
                weekNumberByOpposingTeamNumber[11] = secondDivisionalOpponent.teamNumber;
                weekNumberByOpposingTeamNumber[12] = firstDivisionalOpponent.teamNumber;
            }
            else
            {
                firstDivisionalOpponent = allTeams[adder];
                secondDivisionalOpponent = allTeams[1 + adder];

                weekNumberByOpposingTeamNumber[2] = firstDivisionalOpponent.teamNumber;
                weekNumberByOpposingTeamNumber[3] = secondDivisionalOpponent.teamNumber;
                weekNumberByOpposingTeamNumber[11] = secondDivisionalOpponent.teamNumber;
                weekNumberByOpposingTeamNumber[13] = firstDivisionalOpponent.teamNumber;
            }
        }

        public SortedDictionary<int, int?> GenerateRemainingRandomMatchups(SortedDictionary<int, Team> allTeams, Random rng)
        {
            List<int?> existingMatchupTeamNumbers = GetListOfTeamNumbersThatAlreadyHaveMatchups();
            // TODO: refactor 'GetListOfTeamNumbersToAddMatchupsFor()' to return a list of team numbers, as opposed to a list of teams (List<Team>)
            List<int> teamsToAddForMatchupsToRemoveFrom = GetListOfTeamNumbersToAddMatchupsFor(allTeams, existingMatchupTeamNumbers).Select(t => t.teamNumber).ToList();
            int[] unalteredTeamsForMatchups = new int[teamsToAddForMatchupsToRemoveFrom.Count];
            teamsToAddForMatchupsToRemoveFrom.CopyTo(unalteredTeamsForMatchups);

            int weekCounter = 1;

            KeyValuePair<int, int?>[] temporaryMatchupsHolderArrayInitializer = new KeyValuePair<int, int?>[weekNumberByOpposingTeamNumber.Count];
            weekNumberByOpposingTeamNumber.CopyTo(temporaryMatchupsHolderArrayInitializer, 0);
            SortedDictionary<int, int?> temporaryMatchupsHolder = new SortedDictionary<int, int?>();

            foreach(KeyValuePair<int, int?> matchup in temporaryMatchupsHolderArrayInitializer)
            {
                temporaryMatchupsHolder[matchup.Key] = matchup.Value;
            }

            bool success = false;
            int attemptCounter = 1;
            while(attemptCounter < 50 && !success)
            {
                try
                {
                    foreach (KeyValuePair<int, int?> matchup in weekNumberByOpposingTeamNumber)
                    {
                        if (matchup.Value == null)
                        {
                            int innerAttemptCounter = 1;
                            bool innerSuccess = false;

                            while(innerAttemptCounter < 50 && !innerSuccess)
                            {
                                int randomTeamNumberIndex = rng.Next(teamsToAddForMatchupsToRemoveFrom.Count);
                                int potentialMatchupOpponentTeamNumber = teamsToAddForMatchupsToRemoveFrom[randomTeamNumberIndex];

                                if (!allTeams[potentialMatchupOpponentTeamNumber].weekNumberByOpposingTeamNumber[weekCounter].HasValue)
                                {
                                    temporaryMatchupsHolder[weekCounter] = teamsToAddForMatchupsToRemoveFrom[randomTeamNumberIndex];
                                    teamsToAddForMatchupsToRemoveFrom.RemoveAt(randomTeamNumberIndex);
                                    innerSuccess = true;
                                }
                                innerAttemptCounter++;
                            }
                        }
                        weekCounter++;
                    }

                    success = !temporaryMatchupsHolder.Values.Contains(null);

                }
                catch(Exception e)
                {
                    attemptCounter++;
                    teamsToAddForMatchupsToRemoveFrom = new List<int>(unalteredTeamsForMatchups);
                    temporaryMatchupsHolder = new SortedDictionary<int, int?>();

                    foreach (KeyValuePair<int, int?> matchup in temporaryMatchupsHolderArrayInitializer)
                    {
                        temporaryMatchupsHolder[matchup.Key] = matchup.Value;
                    }

                    weekCounter = 1;
                }                
            }

            return temporaryMatchupsHolder;
        }

        public void DisplaySchedule(SortedDictionary<int, Team> allTeams)
        {
            Console.WriteLine("\n\nSchedule for " + managerName + ":");

            for (int i = 1; i <= weekNumberByOpposingTeamNumber.Count; i++)
            {
                int opposingTeamNumber = weekNumberByOpposingTeamNumber[i].HasValue ? weekNumberByOpposingTeamNumber[i].Value : -1;

                if(opposingTeamNumber != -1)
                {
                    Console.WriteLine($"Week {i}: vs {allTeams[opposingTeamNumber].managerName}");
                }
                else
                {
                    Console.WriteLine($"Week {i}: NOT SET");
                }   
            }
        }

        private List<int?> GetListOfTeamNumbersThatAlreadyHaveMatchups()
        {
            List<int?> existingMatchupTeamNumbers = new List<int?>() { teamNumber };

            foreach (KeyValuePair<int, int?> team in weekNumberByOpposingTeamNumber)
            {
                if (team.Value.HasValue)
                {
                    if (!existingMatchupTeamNumbers.Contains(team.Value))
                    {
                        existingMatchupTeamNumbers.Add(team.Value);
                    }
                }
            }

            return existingMatchupTeamNumbers;
        }

        private List<Team> GetListOfTeamNumbersToAddMatchupsFor(SortedDictionary<int, Team> allTeams, List<int?> existingMatchupTeamNumbers)
        {
            List<Team> remainingMatchups = new List<Team>();

            for (int i = 1; i <= allTeams.Count; i++)
            {
                if (!existingMatchupTeamNumbers.Contains(i))
                {
                    remainingMatchups.Add(allTeams[i]);
                }
            }

            return remainingMatchups;
        }
    }
}
