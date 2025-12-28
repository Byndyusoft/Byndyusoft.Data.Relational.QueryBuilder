using Byndyusoft.Data.Relational.QueryBuilder.Sample.DataAccess.Repositories;
using Byndyusoft.Data.Relational.QueryBuilder.Sample.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace Byndyusoft.Data.Relational.QueryBuilder.Sample.Controllers
{
    [ApiController]
    [Route("api/money")]
    public class MoneyController : ControllerBase
    {
        private readonly IDbSessionFactory _dbSessionFactory;

        public MoneyController(IDbSessionFactory dbSessionFactory)
        {
            _dbSessionFactory = dbSessionFactory;
        }

        [HttpGet]
        public async Task<ActionResult<Money[]>> GetAsync(
            [FromServices] MoneyRepository moneyRepository,
            CancellationToken cancellationToken)
        {
            await using var session = await _dbSessionFactory.CreateSessionAsync(cancellationToken);
            return await moneyRepository.GetAllAsync(cancellationToken);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Money>> GetByIdAsync(
            [FromRoute] long id,
            [FromServices] MoneyRepository moneyRepository,
            CancellationToken cancellationToken)
        {
            await using var session = await _dbSessionFactory.CreateSessionAsync(cancellationToken);

            var money = await moneyRepository.GetByIdAsync(id, cancellationToken);
            if (money == null)
                return NotFound();

            return money;
        }

        [HttpGet("findByCurrency/{currency}")]
        public async Task<ActionResult<Money[]>> GetByNameAsync(
            [FromRoute] Currency currency,
            [FromServices] MoneyRepository moneyRepository,
            CancellationToken cancellationToken)
        {
            await using var session = await _dbSessionFactory.CreateSessionAsync(cancellationToken);
            return await moneyRepository.GetByCurrencyAsync(currency, cancellationToken);
        }

        [HttpPost]
        public async Task<ActionResult<Money>> InsertAsync(
            [FromBody] Money money,
            [FromServices] MoneyRepository moneyRepository,
            CancellationToken cancellationToken)
        {
            await using var session = await _dbSessionFactory.CreateCommittableSessionAsync(cancellationToken);
            await moneyRepository.InsertAsync(money, cancellationToken);
            await session.CommitAsync(cancellationToken);
            return money;
        }

        [HttpPut]
        public async Task<ActionResult> UpdateAsync(
            [FromBody] Money money,
            [FromServices] MoneyRepository moneyRepository,
            CancellationToken cancellationToken)
        {
            await using var session = await _dbSessionFactory.CreateCommittableSessionAsync(cancellationToken);
            await moneyRepository.UpdateAsync(money, cancellationToken);
            await session.CommitAsync(cancellationToken);
            return Ok();
        }
    }
}