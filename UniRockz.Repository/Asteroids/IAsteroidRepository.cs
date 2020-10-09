using System;
using UniRockz.Repository.Asteroids.Entities;
using UniRockz.Repository.Core;

namespace UniRockz.Repository.Asteroids
{
    public interface IAsteroidRepository
        : IRepository<AsteroidInfo>
    { }
}
