using System.Windows;
using System.Windows.Forms;

namespace Digger
{
    class Terrain : ICreature
    {
        public string GetImageFileName() => "Terrain.png";

        public int GetDrawingPriority() => 0;

        public CreatureCommand Act(int x, int y, Game gameSession) => new CreatureCommand();

        public bool DeadInConflict(ICreature conflictedObject, Game gameSession) =>
            conflictedObject is Player || conflictedObject is Sack;
    }

    class Player : ICreature
    {
        public string GetImageFileName() => "Digger.png";

        public int GetDrawingPriority() => 1;

        public CreatureCommand Act(int x, int y, Game gameSession)
        {
            switch (gameSession.KeyPressed)
            {
                case Keys.Left when CanPlayerGoToPoint(x - 1, y, gameSession):
                    return new CreatureCommand { DeltaX = -1 };
                case Keys.Right when CanPlayerGoToPoint(x + 1, y, gameSession):
                    return new CreatureCommand { DeltaX = 1 };
                case Keys.Up when CanPlayerGoToPoint(x, y - 1, gameSession):
                    return new CreatureCommand { DeltaY = -1 };
                case Keys.Down when CanPlayerGoToPoint(x, y + 1, gameSession):
                    return new CreatureCommand { DeltaY = 1 };
                default:
                    return new CreatureCommand();
            }
        }

        public bool DeadInConflict(ICreature conflictedObject, Game gameSession)
        {
            switch (conflictedObject)
            {
                case Terrain _:
                    conflictedObject.DeadInConflict((ICreature)this, gameSession);
                    break;
                case Sack _:
                    gameSession.IsOver = true;
                    return true;
                case Monster _:
                    gameSession.IsOver = true;
                    return true;
            }

            return false;
        }

        private bool CanPlayerGoToPoint(int x, int y, Game gameSession)
        {
            return x >= 0 && y >= 0
                          && x < gameSession.MapWidth && y < gameSession.MapHeight
                          && !(gameSession.Map[x, y] is Sack);
        }
    }

    internal class Sack : ICreature
    {
        private int _dropTime = 0;

        public string GetImageFileName() => "Sack.png";

        public int GetDrawingPriority() => 2;

        public CreatureCommand Act(int x, int y, Game gameSession)
        {
            if (CanDown(x, y, gameSession))
            {
                _dropTime++;
                return new CreatureCommand { DeltaY = 1 };
            }

            if (_dropTime > 1)
                return new CreatureCommand { TransformTo = new Gold() };
            _dropTime = 0;
            return new CreatureCommand();
        }

        public bool CanDown(int x, int y, Game gameSession) =>
            y + 1 < gameSession.MapHeight && (gameSession.Map[x, y + 1] is null ||
                                              (_dropTime > 1 && gameSession.Map[x, y + 1] is Player));


        public bool DeadInConflict(ICreature conflictedObject, Game gameSession) => false;
    }

    class Gold : ICreature
    {
        public string GetImageFileName() => "Gold.png";

        public int GetDrawingPriority() => 3;

        public CreatureCommand Act(int x, int y, Game gameState) => new CreatureCommand();

        public bool DeadInConflict(ICreature conflictedObject, Game gameSession)
        {
            switch (conflictedObject)
            {   
                case Player _:
                    gameSession.Scores += 10;
                    return true;
                case Monster _:
                    return true;
                default:
                    return false;
            }
        }
    }

    class Monster : ICreature
    {
        public string GetImageFileName() => "Monster.png";

        public int GetDrawingPriority() => 4;

        public CreatureCommand Act(int x, int y, Game gameSession)
        {
            var playerPosition = GetPlayerPosition(gameSession);
            if (playerPosition == new Point(-1, -1))
                return new CreatureCommand();
            var deltaX = playerPosition.X - x;
            var deltaY = playerPosition.Y - y;

            if (deltaX > 0 && CheckPossibilityMove(x + 1, y, gameSession))
                return new CreatureCommand { DeltaX = 1 };
            if (deltaX < 0 && CheckPossibilityMove(x - 1, y, gameSession))
                return new CreatureCommand { DeltaX = -1 };
            if (deltaY >= 0 && CheckPossibilityMove(x, y + 1, gameSession))
                return new CreatureCommand { DeltaY = 1 };
            if (deltaY <= 0 && CheckPossibilityMove(x, y - 1, gameSession))
                return new CreatureCommand { DeltaY = -1 };

            return new CreatureCommand();
        }


        public bool DeadInConflict(ICreature conflictedObject, Game gameSession)
        {
            switch (conflictedObject)
            {
                case Monster _:
                    return true;
                case Gold _:
                    return false;
                case Sack _:
                    return true;
            }

            return false;
        }

        private bool CheckPossibilityMove(int x, int y, Game gameSession)
        {
            if (x < 0 || y < 0 || x >= gameSession.MapWidth || y >= gameSession.MapHeight)
                return false;
            switch (gameSession.Map[x, y])
            {
                case Terrain _:
                    return false;
                case Sack _:
                    return false;
                case Monster _:
                    return false;
            }

            return true;
        }

        private Point GetPlayerPosition(Game gameSession)
        {
            for (var i = 0; i < gameSession.MapWidth; i++)
            {
                for (var j = 0; j < gameSession.MapHeight; j++)
                {
                    if (gameSession.Map[i, j] is Player)
                        return new Point(i, j);
                }
            }

            return new Point(-1, -1);
        }
    }
}