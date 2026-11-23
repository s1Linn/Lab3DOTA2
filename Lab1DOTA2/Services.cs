using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab2DOTA2
{
    public interface IAccountService
    {
        PlayerEntity CreateAccount(string userName, string accountType, int initialRating = 1000);
        List<PlayerEntity> ReadAccounts();
        PlayerEntity ReadAccountById(int id);
    }

    public interface IGameService
    {
        void CreateGame(int winnerId, int loserId, BaseGame game);
        List<GameEntity> ReadGames();
        List<GameEntity> ReadPlayerGamesByPlayerId(int playerId);
    }

    public class AccountService : IAccountService
    {
        private readonly IPlayerRepository _playerRepository;

        public AccountService(IPlayerRepository playerRepository)
        {
            _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
        }

        public PlayerEntity CreateAccount(string userName, string accountType, int initialRating = 1000)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException("Ім'я гравця не може бути порожнім!", nameof(userName));

            if (initialRating < 1)
                initialRating = 1;

            var entity = new PlayerEntity
            {
                UserName = userName,
                AccountType = string.IsNullOrWhiteSpace(accountType) ? nameof(StandardAccount) : accountType,
                CurrentRating = initialRating,
                ConsecutiveWins = 0
            };

            _playerRepository.Create(entity);
            return entity;
        }

        public List<PlayerEntity> ReadAccounts()
        {
            return _playerRepository.ReadAll();
        }

        public PlayerEntity ReadAccountById(int id)
        {
            return _playerRepository.ReadById(id);
        }
    }

    public class GameService : IGameService
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly IGameRepository _gameRepository;

        public GameService(IPlayerRepository playerRepository, IGameRepository gameRepository)
        {
            _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
            _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
        }

        public void CreateGame(int winnerId, int loserId, BaseGame game)
        {
            if (game == null)
                throw new ArgumentNullException(nameof(game));

            var winnerEntity = _playerRepository.ReadById(winnerId)
                ?? throw new KeyNotFoundException($"Переможця з Id = {winnerId} не знайдено.");

            var loserEntity = _playerRepository.ReadById(loserId)
                ?? throw new KeyNotFoundException($"Програвшого з Id = {loserId} не знайдено.");

            GameAccount winnerAccount = BuildAccountFromEntity(winnerEntity);
            GameAccount loserAccount = BuildAccountFromEntity(loserEntity);

            int winnerRatingBefore = winnerAccount.CurrentRating;
            int loserRatingBefore = loserAccount.CurrentRating;

            winnerAccount.WinGame(loserAccount, game);
            loserAccount.LoseGame(winnerAccount, game);

            int winnerDelta = winnerAccount.CurrentRating - winnerRatingBefore;
            int loserDelta = loserAccount.CurrentRating - loserRatingBefore;

            winnerEntity.CurrentRating = winnerAccount.CurrentRating;
            winnerEntity.ConsecutiveWins = winnerAccount is StreakBonusAccount streakWin
                ? streakWin.ConsecutiveWins
                : 0;

            loserEntity.CurrentRating = loserAccount.CurrentRating;
            loserEntity.ConsecutiveWins = 0;

            _playerRepository.Update(winnerEntity);
            _playerRepository.Update(loserEntity);

            var now = DateTime.Now;
            string gameTypeName = game.GetType().Name;

            _gameRepository.Create(new GameEntity
            {
                PlayerId = winnerEntity.Id,
                OpponentId = loserEntity.Id,
                GameType = gameTypeName,
                RatingDelta = winnerDelta,
                IsWin = true,
                PlayedAt = now
            });

            _gameRepository.Create(new GameEntity
            {
                PlayerId = loserEntity.Id,
                OpponentId = winnerEntity.Id,
                GameType = gameTypeName,
                RatingDelta = loserDelta,
                IsWin = false,
                PlayedAt = now
            });
        }

        public List<GameEntity> ReadGames()
        {
            return _gameRepository.ReadAll();
        }

        public List<GameEntity> ReadPlayerGamesByPlayerId(int playerId)
        {
            return _gameRepository.ReadAll()
                .Where(g => g.PlayerId == playerId)
                .ToList();
        }

        private static GameAccount BuildAccountFromEntity(PlayerEntity entity)
        {
            switch (entity.AccountType)
            {
                case nameof(HalfLossAccount):
                    return new HalfLossAccount(entity.UserName, entity.CurrentRating);

                case nameof(StreakBonusAccount):
                    var streakAcc = new StreakBonusAccount(entity.UserName, entity.CurrentRating);
                    streakAcc.ConsecutiveWins = entity.ConsecutiveWins;
                    return streakAcc;

                default:
                    return new StandardAccount(entity.UserName, entity.CurrentRating);
            }
        }
    }
}