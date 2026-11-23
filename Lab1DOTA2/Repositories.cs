using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab2DOTA2
{
    public interface IPlayerRepository
    {
        void Create(PlayerEntity player);
        PlayerEntity ReadById(int id);
        List<PlayerEntity> ReadAll();
        void Update(PlayerEntity player);
        void Delete(int id);
    }

    public interface IGameRepository
    {
        void Create(GameEntity game);
        GameEntity ReadById(int id);
        List<GameEntity> ReadAll();
        void Update(GameEntity game);
        void Delete(int id);
    }

    public class PlayerRepository : IPlayerRepository
    {
        private readonly AppDbContext _context;

        public PlayerRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void Create(PlayerEntity player)
        {
            if (player == null)
                throw new ArgumentNullException(nameof(player));

            if (player.Id == 0)
                player.Id = _context.GetNextPlayerId();

            _context.Players.Add(player);
        }

        public PlayerEntity ReadById(int id)
        {
            return _context.Players.FirstOrDefault(p => p.Id == id);
        }

        public List<PlayerEntity> ReadAll()
        {
            return _context.Players.ToList();
        }

        public void Update(PlayerEntity player)
        {
            if (player == null)
                throw new ArgumentNullException(nameof(player));

            var existing = ReadById(player.Id);
            if (existing == null)
                throw new KeyNotFoundException($"Гравця з Id = {player.Id} не знайдено.");

            existing.UserName = player.UserName;
            existing.AccountType = player.AccountType;
            existing.CurrentRating = player.CurrentRating;
            existing.ConsecutiveWins = player.ConsecutiveWins;
        }

        public void Delete(int id)
        {
            var existing = ReadById(id);
            if (existing != null)
            {
                _context.Players.Remove(existing);
            }
        }
    }

    public class GameRepository : IGameRepository
    {
        private readonly AppDbContext _context;

        public GameRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void Create(GameEntity game)
        {
            if (game == null)
                throw new ArgumentNullException(nameof(game));

            if (game.Id == 0)
                game.Id = _context.GetNextGameId();

            _context.Games.Add(game);
        }

        public GameEntity ReadById(int id)
        {
            return _context.Games.FirstOrDefault(g => g.Id == id);
        }

        public List<GameEntity> ReadAll()
        {
            return _context.Games.ToList();
        }

        public void Update(GameEntity game)
        {
            if (game == null)
                throw new ArgumentNullException(nameof(game));

            var existing = ReadById(game.Id);
            if (existing == null)
                throw new KeyNotFoundException($"Гру з Id = {game.Id} не знайдено.");

            existing.PlayerId = game.PlayerId;
            existing.OpponentId = game.OpponentId;
            existing.GameType = game.GameType;
            existing.RatingDelta = game.RatingDelta;
            existing.IsWin = game.IsWin;
            existing.PlayedAt = game.PlayedAt;
        }

        public void Delete(int id)
        {
            var existing = ReadById(id);
            if (existing != null)
            {
                _context.Games.Remove(existing);
            }
        }
    }
}