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

		private Direction _nextWaypoint {
			get {
				if (Math.Abs (_self.X - _self.NextWaypointX) <= _game.TrackTileSize)
					return (_self.X < _self.NextWaypointX) ? Direction.Right : Direction.Left;
				return sDirection;
			}
		}

		private int activeTicks = 0;
		private int slowDown = 0;

		private bool rearMove = false;

		private TileType _topTile {
			get { 
				if ((int)(_self.Y / _game.TrackTileSize) > 0)
					return _world.TilesXY [(int)(_self.X / _game.TrackTileSize)] [(int)(_self.Y / _game.TrackTileSize) - 1];
				else
					return TileType.Empty;
			}
		}

		private TileType _rightTile {
			get { 
				if ((int)(_self.X / _game.TrackTileSize) < _world.Width - 1)
					return _world.TilesXY [(int)(_self.X / _game.TrackTileSize) + 1] [(int)(_self.Y / _game.TrackTileSize)];
				else
					return TileType.Empty;
			}
		}

		private TileType _botTile {
			get { 
				if ((int)(_self.Y / _game.TrackTileSize) < _world.Height - 1)
					return _world.TilesXY [(int)(_self.X / _game.TrackTileSize)] [(int)(_self.Y / _game.TrackTileSize) + 1];
				else
					return TileType.Empty;
			}
		}

		private TileType _leftTile {
			get { 
				if ((int)(_self.X / _game.TrackTileSize) > 0)
					return _world.TilesXY [(int)(_self.X / _game.TrackTileSize) - 1] [(int)(_self.Y / _game.TrackTileSize)];
				else
					return TileType.Empty;
			}
		}

		private TileType _thisTile {
			get { return _world.TilesXY [(int)(_self.X / _game.TrackTileSize)] [(int)(_self.Y / _game.TrackTileSize)];}
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
				if (Math.Abs (_self.Angle - Math.PI / 2) < Math.PI)
					_move.WheelTurn = -1 * ((_self.Angle - Math.PI / 2) / 2);
				else
					_move.WheelTurn = -1 * ((_self.Angle + Math.PI / 2) / 2);
			}

			if (sDirection == Direction.Left) {
				if (Math.Abs (_self.Angle - Math.PI) < Math.PI) 
					_move.WheelTurn = ((_self.Angle - Math.PI) / 2);
				else
					_move.WheelTurn = ((_self.Angle + Math.PI) / 2);
			}

			_move.WheelTurn = _move.WheelTurn * ((rearMove) ? 1 : -1);
		}

        private void NewTurnWheel()
        {
            if (sDirection == Direction.Down)
            {
                if (_self.Angle > 0)
                    _move.WheelTurn = -1 * (_self.Angle - Math.PI / 2);
                else if (_self.Angle < -Math.PI / 2)
                    _move.WheelTurn = -1;
                else
                    _move.WheelTurn = 1;
            }

            if (sDirection == Direction.Up)
            {
                if (Math.Abs(_self.Angle) < Math.PI / 2)
                    _move.WheelTurn = -1 * (Math.PI / 2 - Math.Abs(_self.Angle)) / 2;
                else
                    _move.WheelTurn = ((Math.Abs(_self.Angle) - Math.PI / 2) / 2);
                //				_move.WheelTurn = -1 * ((_self.Angle + Math.PI / 2) / 2);
            }

            if (sDirection == Direction.Right)
            {
                _move.WheelTurn = -1 * (_self.Angle / 2);
            }

            if (sDirection == Direction.Left)
            {
                if (Math.Abs(_self.Angle - Math.PI) < Math.PI)
                    _move.WheelTurn = -1 * ((_self.Angle - Math.PI) / 2);
                else
                    _move.WheelTurn = -1 * ((_self.Angle + Math.PI) / 2);
            }

            if (rearMove)
                _move.WheelTurn = -_move.WheelTurn;
        }

        public void CheckTurn()
		{

		if (sDirection == Direction.Up && _topTile == TileType.LeftTopCorner) {
			sDirection = Direction.Right;
		}

		if (sDirection == Direction.Up && _topTile == TileType.RightTopCorner) {
			sDirection = Direction.Left;
		}

		if (sDirection == Direction.Right && _rightTile == TileType.RightTopCorner) {
			sDirection = Direction.Down;
		}

		if (sDirection == Direction.Right && _rightTile == TileType.RightBottomCorner) {
			sDirection = Direction.Up;
		}

		if (sDirection == Direction.Down && _botTile == TileType.RightBottomCorner) {
			sDirection = Direction.Left;
		}

		if (sDirection == Direction.Down && _botTile == TileType.LeftBottomCorner) {
			sDirection = Direction.Right;
		}

		if (sDirection == Direction.Left && _leftTile == TileType.LeftBottomCorner) {
			sDirection = Direction.Up;
		}

		if (sDirection == Direction.Left && _leftTile == TileType.LeftTopCorner) {
			sDirection = Direction.Down;
		}

			if (sDirection == Direction.Up && (_topTile == TileType.Crossroads || _topTile == TileType.TopHeadedT)
			    || sDirection == Direction.Down && (_botTile == TileType.Crossroads || _botTile == TileType.BottomHeadedT)) {
				if (Math.Abs (_self.X - _self.NextWaypointX) <= _game.TrackTileSize * 2) {
					sDirection = (_self.X < _self.NextWaypointX) ? Direction.Right : Direction.Left;
				}
			}
			
			if (sDirection == Direction.Left && (_leftTile == TileType.Crossroads || _leftTile == TileType.LeftHeadedT)
			    || sDirection == Direction.Right && (_rightTile == TileType.Crossroads || _rightTile == TileType.RightHeadedT)) {
				if (Math.Abs (_self.Y - _self.NextWaypointY) <= _game.TrackTileSize * 2) {
					sDirection = (_self.Y < _self.NextWaypointY) ? Direction.Down : Direction.Up;
				}
			}
		}

		private void CheckStuck()
		{
			if (Math.Abs(lastX1 - lastX3) < 0.05 && Math.Abs(lastY1 - lastY3) < 0.05 && _world.Tick > _game.InitialFreezeDurationTicks) {
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
				move.WheelTurn = 0;//(activeTicks > 50) ? -1 : 0;
			}

			if (activeTicks == 90 && Math.Abs (lastX1 - lastX3) < 0.05 && Math.Abs (lastY1 - lastY3) < 0.05 && _world.Tick > _game.InitialFreezeDurationTicks)
				rearMove = false;

			if (activeTicks > 0) {
				activeTicks --;
				return;
			}

			if (slowDown > 0) {
				move.EnginePower = 0.5D;
			}
			else
				move.EnginePower = 1.0D;

			move.IsThrowProjectile = false;
			move.IsSpillOil = false;

			if (world.Tick > game.InitialFreezeDurationTicks && world.Tick < game.InitialFreezeDurationTicks + 10) {
				move.WheelTurn = -15;
			}

			if (world.Tick > game.InitialFreezeDurationTicks && world.Tick < game.InitialFreezeDurationTicks + 20) {
                move.IsUseNitro = true;
                //				move.IsSpillOil = true;
                //				move.WheelTurn = 0;
                sDirection = world.StartingDirection;
			}

			if (world.Tick > game.InitialFreezeDurationTicks && world.Tick < game.InitialFreezeDurationTicks + 10 && move.WheelTurn == 0) {
//				move.WheelTurn = 1;
			}

            if (world.Tick > game.InitialFreezeDurationTicks) {
				CheckTurn ();
				NewTurnWheel ();
				CheckStuck ();
            }

			if (_thisTile != TileType.Empty) {
//				saveDirection = sDirection;
			}
        }
    }
}