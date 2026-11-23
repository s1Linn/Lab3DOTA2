using System;

namespace Lab2DOTA2
{
    public class GameEntity
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public int? OpponentId { get; set; }
        public string GameType { get; set; }
        public int RatingDelta { get; set; }
        public bool IsWin { get; set; }
        public DateTime PlayedAt { get; set; }
    }
}