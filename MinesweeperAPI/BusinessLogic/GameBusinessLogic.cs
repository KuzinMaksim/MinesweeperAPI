using MinesweeperAPI.Interfeces;
using MinesweeperAPI.Models;
using System;
using System.Collections.Concurrent;

namespace MinesweeperAPI.BusinessLogic
{
    public class GameBusinessLogic : IGameBusinessLogic
    {
        private ConcurrentBag<GameInfoResponse> _games;
        private ConcurrentDictionary<Guid, string[][]> _originalFields;

        private readonly Random _random;
        public GameBusinessLogic()
        {
            _games = new ConcurrentBag<GameInfoResponse>();
            _originalFields = new ConcurrentDictionary<Guid, string[][]>();

            _random = new Random();
        }
        public GameInfoResponse GameTurn(GameTurnRequest request)
        {
            var game = _games.FirstOrDefault(g => g.GameId == request.GameID);
            if (game == null)
                throw new Exception("Игра не найдена");

            if (game.Completed)
                throw new Exception("Игра уже завершена");

            if (!_originalFields.TryGetValue(request.GameID, out var originalField))
                throw new Exception("Ошибка получения поля игры");

            if (game.Field[request.Row][request.Column] != " ")
                throw new Exception("Ячейка уже открыта");

            if (originalField[request.Row][request.Column] == "X")
            {
                game.Completed = true;
                game.Field = originalField;
                return game;
            }

            RevealCell(originalField, game.Field, request.Row, request.Column, game.Width, game.Height);

            if (IsGameCompleted(game.Field, originalField, game.Width, game.Height))
            {
                RevealMines(game.Field, originalField, game.Width, game.Height);
                game.Completed = true;
            }


            return game;
        }

        public GameInfoResponse NewGame(NewGameRequest request)
        {
            if (request.Width < 2 || request.Width > 30)
                throw new Exception("Ширина поля должна быть не менее 2 и не более 30");

            if (request.Height < 2 || request.Height > 30)
                throw new Exception("Высота поля должна быть не менее 2 и не более 30");

            int maxMinesCount = request.Width * request.Height - 1;
            if (request.MinesCount > maxMinesCount)
                throw new Exception($"Количество мин должно быть не менее 1 и не более {maxMinesCount}");

            var gameId = Guid.NewGuid();
            string[][] field = new string[request.Height][];
            for (int i = 0; i < request.Height; i++)
            {
                field[i] = new string[request.Width];
                for (int j = 0; j < request.Width; j++)
                {
                    field[i][j] = " ";
                }
            }

            var fieldCopy = field.Select(row => row.ToArray()).ToArray();
            var originalField = CreateOriginalField(fieldCopy, request.Width, request.Height, request.MinesCount);

            var game = new GameInfoResponse
            {
                GameId = gameId,
                Field = field,
                Completed = false,
                Height = request.Height,
                Width = request.Width,
                MinesCount = request.MinesCount
            };

            _games.Add(game);
            if (!_originalFields.TryAdd(game.GameId, originalField))
                throw new Exception("Ошибка при добавлении поля игры");

            return game;
        }

        private string[][] CreateOriginalField(string[][] field, int width, int height, int minesCount)
        {
            int placedMines = 0;
            while (placedMines < minesCount)
            {
                int row = _random.Next(height);
                int col = _random.Next(width);

                if (field[row][col] == "X")
                    continue;

                field[row][col] = "X";
                placedMines++;
            }
            return CalculateNumbers(field, width, height);
        }

        private string[][] CalculateNumbers(string[][] field, int width, int height)
        {
            int[] dx = { -1, -1, -1, 0, 1, 1, 1, 0 };
            int[] dy = { -1, 0, 1, 1, 1, 0, -1, -1 };

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    if (field[row][col] == "X") continue;

                    int count = 0;

                    for (int i = 0; i < 8; i++)
                    {
                        int newRow = row + dy[i];
                        int newCol = col + dx[i];

                        if (newRow >= 0 && newRow < height && newCol >= 0 && newCol < width && field[newRow][newCol] == "X")
                        {
                            count++;
                        }
                    }

                    field[row][col] = count.ToString();
                }
            }
            return field;
        }

        private void RevealCell(string[][] originalField, string[][] field, int row, int col, int width, int height)
        {
            if (field[row][col] != " ")
                return;

            field[row][col] = originalField[row][col];

            if (originalField[row][col] == "0")
            {
                int[] dx = { -1, -1, -1, 0, 1, 1, 1, 0 };
                int[] dy = { -1, 0, 1, 1, 1, 0, -1, -1 };

                for (int i = 0; i < 8; i++)
                {
                    int newRow = row + dy[i];
                    int newCol = col + dx[i];

                    if (newRow >= 0 && newRow < height && newCol >= 0 && newCol < width)
                    {
                        RevealCell(originalField, field, newRow, newCol, width, height);
                    }
                }
            }
        }

        private bool IsGameCompleted(string[][] field, string[][] originalField, int width, int height)
        {
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    if (originalField[row][col] != "X" && field[row][col] == " ")
                        return false;
                }
            }
            return true;
        }

        private void RevealMines(string[][] field, string[][] originalField, int width, int height)
        {
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    if (originalField[row][col] == "X")
                    {
                        field[row][col] = "M";
                    }
                }
            }
        }
    }
}