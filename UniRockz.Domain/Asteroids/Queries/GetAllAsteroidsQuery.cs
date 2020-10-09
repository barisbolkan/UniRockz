using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MediatR;
using UniRockz.Domain.Asteroids.Responses;
using UniRockz.Repository.Asteroids.Entities;

namespace UniRockz.Domain.Asteroids.Queries
{
    public class GetAllAsteroidsQuery : IRequest<IEnumerable<AsteroidInfoResponse>>
    {
        public Expression<Func<AsteroidInfoResponse, bool>> Filter { get; set; }
    }
}
