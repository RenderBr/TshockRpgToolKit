﻿using OTAPI.Tile;
using System;
using TShockAPI;

namespace Banking
{
	public class BlockMinedEventArgs : EventArgs
	{
		public TSPlayer Player { get; private set; }
		public int TileX { get; private set; }
		public int TileY { get; private set; }
		public ITile Tile { get; private set; }

		public BlockMinedEventArgs(TSPlayer player, int tileX, int tileY, ITile tile)
		{
			Player = player;
			TileX = tileX;
			TileY = tileY;
			Tile = tile;
		}
	}
}