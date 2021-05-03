using System;

public abstract class Asset : IDisposable
{
    public abstract bool IsLoaded { get; }


    public abstract void Load();
    
    public abstract void Unload();


    public virtual Asset Copy()
    {
        return this;
    }
        
    public void Dispose()
    {
        Unload();
    }
}