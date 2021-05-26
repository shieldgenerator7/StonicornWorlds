using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : Manager
{
    private Player player;
    public Player Player
    {
        get => player;
        set
        {
            player = value;
            onPlayerChanged?.Invoke(player);
        }
    }
    public delegate void OnPlayerChanged(Player p);
    public event OnPlayerChanged onPlayerChanged;

    public override void setup()
    {
        if (player == null)
        {
            Player player = new Player();
            Planet planet = new Planet();
            player.planets.Add(planet);
            Player = player;
        }
    }
}
