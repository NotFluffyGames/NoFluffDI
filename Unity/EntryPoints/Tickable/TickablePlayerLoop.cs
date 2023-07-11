using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace NotFluffy.NoFluffDI.Unity.Unity.EntryPoints.Tickable
{
    public class TickablePlayerLoop : IPlayerLoopItem, IDisposable
    {
        private bool disposed;
        private readonly ITickable tickable;
        
        public TickablePlayerLoop(ITickable tickable)
        {
            this.tickable = tickable;
        }

        public bool MoveNext()
        {
            if (disposed)
                return false;
            
            tickable.Tick(Time.deltaTime);
            return true;
        }

        public void Dispose()
        {
            disposed = true;
        }
    }
}