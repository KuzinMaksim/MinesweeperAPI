using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Writers;
using MinesweeperAPI.Interfeces;
using MinesweeperAPI.Models;

namespace MinesweeperAPI.Controllers
{
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly IGameBusinessLogic _gameLogic;
        public GameController(IGameBusinessLogic gameLogic) { _gameLogic = gameLogic; }

        [HttpPost("new")]
        public ActionResult<GameInfoResponse> NewGame([FromBody] NewGameRequest request)
        {
            try
            {
                var response = _gameLogic.NewGame(request);
                return Ok(response);
            }
            catch(Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("turn")]
        public ActionResult<GameInfoResponse> PlayersTurn([FromBody] GameTurnRequest request)
        {
            try
            {
                var response = _gameLogic.GameTurn(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
