using MyPlayer;

public interface IReloadable
{
    public void Reload(Entity wielder);
    public bool shouldReload { get; }
}