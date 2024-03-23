﻿using Grpc.Core;
using ProtoBuf;
using ProtoBuf.Grpc.Configuration;
using RIN.InternalAPI.Models;

namespace RIN.InternalAPI
{
    [Service("RIN.GameServerAPI")]
    public interface IGameServerAPI
    {
        public ValueTask<PingResp> Ping(PingReq req);
        public ValueTask<CharacterAndBattleframeVisuals> GetCharacterAndBattleframeVisuals(CharacterID req);
        public ValueTask<Empty> SaveCharacterGameSessionData(GameSessionData req);
        public Task Listen(Empty req, IServerStreamWriter<Event> responseStream, ServerCallContext context);
    }

    [ProtoContract]
    public class Empty()
    {
    }
}
