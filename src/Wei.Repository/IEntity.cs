using System;

namespace Wei.Repository
{
    public interface IEntity : IEntity<int>
    {
    }

    public interface IEntity<TPrimaryKey> : ITrack
    {
        TPrimaryKey Id { get; set; }
    }
}
