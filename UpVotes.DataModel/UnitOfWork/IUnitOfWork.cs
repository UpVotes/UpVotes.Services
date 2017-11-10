using System;

namespace UpVotes.DataModel.UnitOfWork
{
    public interface IUnitOfWork :IDisposable
    {
        void Save();
    }
}
