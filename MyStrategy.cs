using System;
using Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeRacing2015.DevKit.CSharpCgdk {
    public sealed class MyStrategy : IStrategy {
		private Car _self;
		private World _world;
		private Game _game; 
		private Move _move;
		private double lastX1 = 0;
		private double lastX2 = 0;
		private double lastX3 = 0;

		private double lastY1 = 0;
		private double lastY2 = 0;
		private double lastY3 = 0;

		private Direction sDirection;

		private int activeTicks = 0;
		private bool rearMove = false;

		private TileType _topTile {
			get { return _world.TilesXY [(int)(_self.X / _game.TrackTileSize)] [(int)(_self.Y / _game.TrackTileSize) - 1];}
		}

		private TileType _rightTile {
			get { return _world.TilesXY [(int)(_self.X / _game.TrackTileSize) + 1] [(int)(_self.Y / _game.TrackTileSize)];}
		}

		private TileType _botTile {
			get { return _world.TilesXY [(int)(_self.X / _game.TrackTileSize)] [(int)(_self.Y / _game.TrackTileSize) + 1];}
		}

		private TileType _leftTile {
			get { return _world.TilesXY [(int)(_self.X / _game.TrackTileSize) - 1] [(int)(_self.Y / _game.TrackTileSize)];}
		}

		private void TurnWheel()
		{
			if (sDirection == Direction.Up) {
				_move.WheelTurn = ((_self.Angle + Math.PI / 2) / 2);
			}

			if (sDirection == Direction.Right) {
				_move.WheelTurn = (_self.Angle / 2);
			}

			if (sDirection == Direction.Down) {
				_move.WheelTurn = ((_self.Angle - Math.PI / 2) / 2);
			}

			if (sDirection == Direction.Left) {
				if (Math.Abs (_self.Angle - Math.PI) < Math.PI) 
					_move.WheelTurn = ((_self.Angle - Math.PI) / 2);
				else
					_move.WheelTurn = ((_self.Angle + Math.PI) / 2);
			}
			_move.WheelTurn *= (rearMove) ? 1 : -1;
		}

		public void CheckTurn()
		{
			if (sDirection == Direction.Up && _topTile == TileType.LeftTopCorner)
				sDirection = Direction.Right;

			if (sDirection == Direction.Right && _rightTile == TileType.RightTopCorner)
				sDirection = Direction.Down;

			if (sDirection == Direction.Down && _botTile == TileType.RightBottomCorner)
				sDirection = Direction.Left;

			if (sDirection == Direction.Left && _leftTile == TileType.LeftBottomCorner)
				sDirection = Direction.Up;
		}

		private void CheckStuck()
		{
			if (lastX1 == lastX3 && lastY1 == lastY3 && _world.Tick > _game.InitialFreezeDurationTicks) {
					activeTicks = 100;
					rearMove = true;
			} else
				rearMove = false;
		}

        public void Move(Car self, World world, Game game, Move move) {
			lastX1 = lastX2;
			lastX2 = lastX3;
			lastX3 = Math.Round(self.X, 2);
			lastY1 = lastY2;
			lastY2 = lastY3;
			lastY3 = Math.Round(self.Y, 2);

			this._game = game;
			this._move = move;
			this._self = self;
			this._world = world;

			if (rearMove) {
				move.EnginePower = -1.0D;
				move.WheelTurn = -1;
			}

			if (activeTicks > 0) {
				activeTicks --;
				return;
			}

            move.EnginePower = 1.0D;
			move.IsThrowProjectile = false;
			move.IsSpillOil = false;

			if (world.Tick > game.InitialFreezeDurationTicks && world.Tick < game.InitialFreezeDurationTicks + 10) {
				move.WheelTurn = -15;
			}

			if (world.Tick > game.InitialFreezeDurationTicks && world.Tick < game.InitialFreezeDurationTicks + 20) {
				move.IsSpillOil = true;
				move.WheelTurn = 0;
				sDirection = world.StartingDirection;
			}

			if (world.Tick > game.InitialFreezeDurationTicks && world.Tick < game.InitialFreezeDurationTicks + 10 && move.WheelTurn == 0) {
				move.WheelTurn = 15;
			}

            if (world.Tick > game.InitialFreezeDurationTicks) {
                move.IsUseNitro = true;
				CheckTurn ();
				TurnWheel ();
				CheckStuck ();
            }
        }
    }
}