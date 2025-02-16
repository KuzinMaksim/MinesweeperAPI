using MinesweeperAPI.Models;

namespace MinesweeperAPI.Interfeces
{
    public interface IGameBusinessLogic
    {
        public GameInfoResponse NewGame(NewGameRequest request);
        public GameInfoResponse GameTurn(GameTurnRequest request);
    }
}
