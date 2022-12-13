namespace Digger
{
    public interface ICreature
    {
        string GetImageFileName();
        int GetDrawingPriority();
        CreatureCommand Act(int x, int y, Game gameSession);
        bool DeadInConflict(ICreature conflictedObject, Game gameSession);
    }
}