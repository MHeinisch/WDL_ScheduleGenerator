using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WDL_ScheduleGenerator
{
    class LeagueSchedule
    {
        public SortedDictionary<int, Team> teamNumberByTeam;
        private string[] managerNames;
        // private List<Team> allTeams;

        public LeagueSchedule()
        {
            teamNumberByTeam = new SortedDictionary<int, Team>();
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

                teamNumberByTeam.Add(team.teamNumber, team);
            }
        }

        public bool GenerateLeagueSchedule(Random rng)
        {
            Console.WriteLine("Starting league schedule generation...");

            foreach (KeyValuePair<int, Team> team in teamNumberByTeam)
            {
                team.Value.SetDivisionalMatchups(teamNumberByTeam);
            }

            bool continuedSuccess = true;

            foreach (KeyValuePair<int, Team> team in teamNumberByTeam)
            {
                SortedDictionary<int, int?> generatedScheduleForCurrentTeam = team.Value.GenerateRemainingRandomMatchups(teamNumberByTeam, rng);

                // confirm generated schedule is valid, and react if so
                continuedSuccess = !generatedScheduleForCurrentTeam.Values.Contains(null);

                if(continuedSuccess)
                {
                    // assign returned team to appropriate thing in leagueSchedule                    
                    teamNumberByTeam[team.Value.teamNumber].weekNumberByOpposingTeamNumber = generatedScheduleForCurrentTeam;

                    // iterate over that returned team, and apply all matchups to existing leagueSchedule
                    int weekCounter = 1;
                    foreach(KeyValuePair<int, int?> generatedMatchup in generatedScheduleForCurrentTeam)
                    {                      
                        Team teamInQuestion = teamNumberByTeam[(int)generatedMatchup.Value];
                        
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
            foreach(KeyValuePair<int, Team> team in teamNumberByTeam)
            {
                team.Value.DisplaySchedule(teamNumberByTeam);
            }

            Console.ReadLine();
        }

        private int getAdder(int divisionNumber)
        {
            return (divisionNumber - 1) * 3;
        }

        public void GenerateExtraWeekMatchups(Random rng)
        {
            // As of 2021, the NFL now has 17 regular season games/18 weeks (instead of 16 games/17 weeks)
            // This means that a fantasy season should have 14 regular season weeks (1 more than before)
            // The existing generator populates matches for 13 weeks (2 against your divisional opponents, 1 against every other league member)
            // SO: This function will generate one additional week of matchups as an inter-divisional set of matchups
            // It is recommended that the new matchups take place in Week 11 (pushing the last 3 weeks back by one [where 2/3 are divisional matchups])

            // randomly pair up divisions
            List<int> divisionNumbers = new List<int>() { 1, 2, 3, 4 };
            int[] divisionPairOne = new int[2];
            int divisionOne = divisionNumbers[rng.Next(divisionNumbers.Count)];
            divisionPairOne[0] = divisionOne;
            divisionNumbers.Remove(divisionOne);

            int divisionTwo = divisionNumbers[rng.Next(divisionNumbers.Count)];
            divisionPairOne[1] = divisionTwo;
            divisionNumbers.Remove(divisionTwo);

            int[] divisionPairTwo = new int[2];
            int divisionThree = divisionNumbers[rng.Next(divisionNumbers.Count)];
            divisionPairTwo[0] = divisionThree;
            divisionNumbers.Remove(divisionThree);

            int divisionFour = divisionNumbers[0];
            divisionPairTwo[1] = divisionFour;
            divisionNumbers.Remove(divisionFour);

            // randomly pair up teams within each division
            int[] teamMatchupOne = new int[2];
            int[] teamMatchupTwo = new int[2];
            int[] teamMatchupThree = new int[2];
            List<int> teamNumbersOne = new List<int>() { 1, 2, 3 };
            List<int> teamNumbersTwo = new List<int>() { 1, 2, 3 };

            int teamOneIndex = rng.Next(teamNumbersOne.Count);
            int teamTwoIndex = rng.Next(teamNumbersTwo.Count);
            teamMatchupOne[0] = teamNumbersOne[teamOneIndex] + getAdder(divisionPairOne[0]);
            teamMatchupOne[1] = teamNumbersTwo[teamTwoIndex] + getAdder(divisionPairOne[1]);
            // teamMatchupOne[0] = teamNumbersOne[teamOneIndex] * divisionPairOne[0];
            // teamMatchupOne[1] = teamNumbersTwo[teamTwoIndex] * divisionPairOne[1];
            teamNumbersOne.RemoveAt(teamOneIndex);
            teamNumbersTwo.RemoveAt(teamTwoIndex);

            int teamThreeIndex = rng.Next(teamNumbersOne.Count);
            int teamFourIndex = rng.Next(teamNumbersTwo.Count);
            teamMatchupTwo[0] = teamNumbersOne[teamThreeIndex] + getAdder(divisionPairOne[0]);
            teamMatchupTwo[1] = teamNumbersTwo[teamFourIndex] + getAdder(divisionPairOne[1]);
            //teamMatchupTwo[0] = teamNumbersOne[teamThreeIndex] * divisionPairOne[0];
            //teamMatchupTwo[1] = teamNumbersTwo[teamFourIndex] * divisionPairOne[1];
            teamNumbersOne.RemoveAt(teamThreeIndex);
            teamNumbersTwo.RemoveAt(teamFourIndex);

            int teamFiveIndex = 0;
            int teamSixIndex = 0;
            teamMatchupThree[0] = teamNumbersOne[teamFiveIndex] + getAdder(divisionPairOne[0]);
            teamMatchupThree[1] = teamNumbersTwo[teamSixIndex] + getAdder(divisionPairOne[1]);
            //teamMatchupThree[0] = teamNumbersOne[teamFiveIndex] * divisionPairOne[0];
            //teamMatchupThree[1] = teamNumbersTwo[teamSixIndex] * divisionPairOne[1];
            teamNumbersOne.RemoveAt(teamFiveIndex);
            teamNumbersTwo.RemoveAt(teamSixIndex);


            int[] teamMatchupFour = new int[2];
            int[] teamMatchupFive = new int[2];
            int[] teamMatchupSix = new int[2];
            List<int> teamNumbersThree = new List<int>() { 1, 2, 3 };
            List<int> teamNumbersFour = new List<int>() { 1, 2, 3 };

            int teamSevenIndex = rng.Next(teamNumbersThree.Count);
            int teamEightIndex = rng.Next(teamNumbersFour.Count);
            teamMatchupFour[0] = teamNumbersThree[teamSevenIndex] + getAdder(divisionPairTwo[0]);
            teamMatchupFour[1] = teamNumbersFour[teamEightIndex] + getAdder(divisionPairTwo[1]);
            //teamMatchupFour[0] = teamNumbersThree[teamSevenIndex] * divisionPairTwo[0];
            //teamMatchupFour[1] = teamNumbersFour[teamEightIndex] * divisionPairTwo[1];
            teamNumbersThree.RemoveAt(teamSevenIndex);
            teamNumbersFour.RemoveAt(teamEightIndex);

            int teamNineIndex = rng.Next(teamNumbersThree.Count);
            int teamTenIndex = rng.Next(teamNumbersFour.Count);
            teamMatchupFive[0] = teamNumbersThree[teamNineIndex] + getAdder(divisionPairTwo[0]);
            teamMatchupFive[1] = teamNumbersFour[teamTenIndex] + getAdder(divisionPairTwo[1]);
            //teamMatchupFive[0] = teamNumbersThree[teamNineIndex] * divisionPairTwo[0];
            //teamMatchupFive[1] = teamNumbersFour[teamTenIndex] * divisionPairTwo[1];
            teamNumbersThree.RemoveAt(teamNineIndex);
            teamNumbersFour.RemoveAt(teamTenIndex);

            int teamElevenIndex = 0;
            int teamTwelveIndex = 0;
            teamMatchupSix[0] = teamNumbersThree[teamElevenIndex] + getAdder(divisionPairTwo[0]);
            teamMatchupSix[1] = teamNumbersFour[teamTwelveIndex] + getAdder(divisionPairTwo[1]);
            //teamMatchupSix[0] = teamNumbersThree[teamElevenIndex] * divisionPairTwo[0];
            //teamMatchupSix[1] = teamNumbersFour[teamTwelveIndex] * divisionPairTwo[1];
            teamNumbersThree.RemoveAt(teamElevenIndex);
            teamNumbersFour.RemoveAt(teamTwelveIndex);

            // display WEEK 11 schedule
            Console.WriteLine("=== WEEK 11 MATCHUPS ===");
            Console.WriteLine($"Team {teamNumberByTeam[teamMatchupOne[0]].managerName} vs {teamNumberByTeam[teamMatchupOne[1]].managerName}");
            Console.WriteLine($"Team {teamNumberByTeam[teamMatchupTwo[0]].managerName} vs {teamNumberByTeam[teamMatchupTwo[1]].managerName}");
            Console.WriteLine($"Team {teamNumberByTeam[teamMatchupThree[0]].managerName} vs {teamNumberByTeam[teamMatchupThree[1]].managerName}");
            Console.WriteLine($"Team {teamNumberByTeam[teamMatchupFour[0]].managerName} vs {teamNumberByTeam[teamMatchupFour[1]].managerName}");
            Console.WriteLine($"Team {teamNumberByTeam[teamMatchupFive[0]].managerName} vs {teamNumberByTeam[teamMatchupFive[1]].managerName}");
            Console.WriteLine($"Team {teamNumberByTeam[teamMatchupSix[0]].managerName} vs {teamNumberByTeam[teamMatchupSix[1]].managerName}");
            Console.ReadLine();
        }
    }
}
