using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using UniRockz.Domain.Asteroids.Queries;
using UniRockz.Domain.Asteroids.Responses;
using UniRockz.Domain.Utils;
using UniRockz.Repository.Asteroids;
using UniRockz.Repository.Asteroids.Entities;

namespace UniRockz.Domain.Asteroids.Handlers
{
    public class GetAsteroidHandler :
        IRequestHandler<GetAsteroidByIdQuery, AsteroidInfoResponse>,
        IRequestHandler<GetAllAsteroidsQuery, IEnumerable<AsteroidInfoResponse>>
    {
        private readonly IAsteroidRepository _repository;
        private readonly IMapper _mapper;

        public GetAsteroidHandler(IAsteroidRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public Task<AsteroidInfoResponse> Handle(GetAsteroidByIdQuery request, CancellationToken cancellationToken)
        {
            return _repository.GetByIdAsync(request.Id, cancellationToken)
                .Map(_mapper.Map<AsteroidInfoResponse>);
        }

        public Task<IEnumerable<AsteroidInfoResponse>> Handle(GetAllAsteroidsQuery request, CancellationToken cancellationToken)
        {
            return _repository.GetAllAsync(_mapper.Map<Expression<Func<AsteroidInfo, bool>>>(request.Filter), cancellationToken)
                .Map(_mapper.Map<IEnumerable<AsteroidInfoResponse>>);
        }
    }
}
