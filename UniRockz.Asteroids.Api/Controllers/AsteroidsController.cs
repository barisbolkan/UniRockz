using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Mvc;
using UniRockz.Domain.Asteroids.Queries;
using UniRockz.Domain.Asteroids.Responses;
using UniRockz.Repository.Asteroids.Entities;
using MongoDB.Bson;
using System.Linq;
using System;

namespace UniRockz.Asteroids.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AsteroidsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AsteroidsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [EnableQuery()]
        public async Task<IActionResult> Get(ODataQueryOptions<AsteroidInfoResponse> opts)
        {
            var result = await _mediator.Send(new GetAllAsteroidsQuery()
            {
                Filter = opts.GetFilter<AsteroidInfoResponse>()
            });

            return Ok(result);
        }

        [HttpGet("{asteroidId}")]
        public async Task<IActionResult> GetById(string asteroidId)
        {
            var result = await _mediator.Send(new GetAsteroidByIdQuery(asteroidId));
            return Ok(result);
        }
    }

    public static class ODataQueryOptionsExtensions
    {
        public static Expression<Func<T, bool>> GetFilter<T>(this ODataQueryOptions<T> options)
        {
            IQueryable query = options.Filter?
                .ApplyTo(Enumerable.Empty<T>().AsQueryable(), new ODataQuerySettings());
         
            var call = query?.Expression as MethodCallExpression;
            if (call != null && call.Method.Name == nameof(Queryable.Where) && call.Method.DeclaringType == typeof(Queryable))
            {
                var predicate = ((UnaryExpression)call.Arguments[1]).Operand;
                return (Expression<Func<T, bool>>)predicate;
            }

            return _ => true;
        }
    }
}
