using System;
using System.Collections.Generic;

namespace Lab2DOTA2
{
    public class AppDbContext
    {
        private int _nextPlayerId = 1;
        private int _nextGameId = 1;

        public List<PlayerEntity> Players { get; } = new List<PlayerEntity>();
        public List<GameEntity> Games { get; } = new List<GameEntity>();

        public AppDbContext()
        {
            SeedData();
        }

        public int GetNextPlayerId() => _nextPlayerId++;
        public int GetNextGameId() => _nextGameId++;

        private void SeedData()
        {
            Players.Add(new PlayerEntity
            {
                Id = GetNextPlayerId(),
                UserName = "Arteezy",
                AccountType = nameof(StandardAccount),
                CurrentRating = 1000,
                ConsecutiveWins = 0
            });

            Players.Add(new PlayerEntity
            {
                Id = GetNextPlayerId(),
                UserName = "Dendi",
                AccountType = nameof(HalfLossAccount),
                CurrentRating = 1000,
                ConsecutiveWins = 0
            });

            Players.Add(new PlayerEntity
            {
                Id = GetNextPlayerId(),
                UserName = "stariy_bog",
                AccountType = nameof(StreakBonusAccount),
                CurrentRating = 1000,
                ConsecutiveWins = 0
            });
        }
    }
}