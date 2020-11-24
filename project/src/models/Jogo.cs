using System;
using Godot;
using SocketIOClient;
namespace project.src.models
{
    public class Jogo
    {
        public SocketIO Client { get; set; }

        public async void init()
        {
            await Client.EmitAsync("terminou-carregar", "");
        }
    }
}