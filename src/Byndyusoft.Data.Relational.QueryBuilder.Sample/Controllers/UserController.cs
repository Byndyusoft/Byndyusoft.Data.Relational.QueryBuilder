using System.Threading;
using System.Threading.Tasks;
using Byndyusoft.Data.Relational.QueryBuilder.Sample.DataAccess.Repositories;
using Byndyusoft.Data.Relational.QueryBuilder.Sample.Domain.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Byndyusoft.Data.Relational.QueryBuilder.Sample.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IDbSessionFactory _dbSessionFactory;

        public UserController(IDbSessionFactory dbSessionFactory)
        {
            _dbSessionFactory = dbSessionFactory;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetByIdAsync(
            [FromRoute] long id,
            [FromServices] UserDtoRepository userDtoRepository,
            CancellationToken cancellationToken)
        {
            await using var session = await _dbSessionFactory.CreateSessionAsync(cancellationToken);

            var userDto = await userDtoRepository.GetByIdAsync(id, cancellationToken);
            if (userDto == null)
                return NotFound();

            return userDto;
        }

        [HttpGet("/getAlternatively/{id}")]
        public async Task<ActionResult<UserDto>> GetAlternativelyByIdAsync(
            [FromRoute] long id,
            [FromServices] UserDtoRepository userDtoRepository,
            CancellationToken cancellationToken)
        {
            await using var session = await _dbSessionFactory.CreateSessionAsync(cancellationToken);

            var userDto = await userDtoRepository.GetByIdAlternativelyAsync(id, cancellationToken);
            if (userDto == null)
                return NotFound();

            return userDto;
        }
    }
}