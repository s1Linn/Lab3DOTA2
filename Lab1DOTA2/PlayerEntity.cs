using System;

namespace Lab2DOTA2
{
    public class PlayerEntity
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string AccountType { get; set; }
        public int CurrentRating { get; set; }
        public int ConsecutiveWins { get; set; }
    }
}