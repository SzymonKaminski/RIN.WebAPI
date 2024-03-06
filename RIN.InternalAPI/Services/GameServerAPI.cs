using Grpc.Core;
using Npgsql;
using RIN.Core.DB;
using RIN.InternalAPI.Models;

namespace RIN.InternalAPI.Services
{
    public class GameServerAPI : IGameServerAPI
    {
        private readonly DB DB;
        private readonly ILogger<GameServerAPI> Logger;

        public GameServerAPI(DB db, ILogger<GameServerAPI> logger)
        {
            DB     = db;
            Logger = logger;
        }

        public async ValueTask<PingResp> Ping(PingReq req)
        {
            var resp = new PingResp
            {
                ClientSentTime   = req.SentTime,
                ServerReciveTime = DateTime.UtcNow
            };

            return resp;
        }

        public async ValueTask<CharacterAndBattleframeVisuals> GetCharacterAndBattleframeVisuals(CharacterID req)
        {
            var result    = await DB.GetBasicCharacterAndVisualData(req.ID);
            var bfVisuals = PlayerBattleframeVisuals.CreateDefault();

            var resp = new CharacterAndBattleframeVisuals
            {
                CharacterInfo      = result.Item1,
                CharacterVisuals   = result.Item2,
                BattleframeVisuals = bfVisuals
            };

            return resp;
        }

        public async Task Listen(EmptyReq req, IServerStreamWriter<Event> responseStream, ServerCallContext context)
        {
            await using var connection = new NpgsqlConnection(DB.ConnStr);
            await connection.OpenAsync();

            await using var cmd = new NpgsqlCommand("LISTEN events", connection);
            await cmd.ExecuteNonQueryAsync();

            connection.Notification += async (_, e) =>
            {
                var evt = e.Payload.Split(':');
                await responseStream.WriteAsync(new Event { Type = evt[0], Payload = evt[1] });
            };

            while (!context.CancellationToken.IsCancellationRequested)
            {
                await connection.WaitAsync(context.CancellationToken);
            }
        }
    }
}
