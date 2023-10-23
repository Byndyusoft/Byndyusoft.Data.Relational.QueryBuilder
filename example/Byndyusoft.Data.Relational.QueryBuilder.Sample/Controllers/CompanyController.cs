using System.Threading;
using System.Threading.Tasks;
using Byndyusoft.Data.Relational.QueryBuilder.Sample.DataAccess.Repositories;
using Byndyusoft.Data.Relational.QueryBuilder.Sample.Domain.Dtos;
using Byndyusoft.Data.Relational.QueryBuilder.Sample.Domain.Entities;
using Byndyusoft.Data.Relational.QueryBuilder.Sample.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Byndyusoft.Data.Relational.QueryBuilder.Sample.Controllers
{
    [ApiController]
    [Route("api/companies")]
    public class CompanyController : ControllerBase
    {
        private readonly IDbSessionFactory _dbSessionFactory;

        public CompanyController(IDbSessionFactory dbSessionFactory)
        {
            _dbSessionFactory = dbSessionFactory;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Company>> GetByIdAsync(
            [FromRoute]long id, 
            [FromServices]CompanyRepository companyRepository, 
            CancellationToken cancellationToken)
        {
            await using var session = await _dbSessionFactory.CreateSessionAsync(cancellationToken);

            var company = await companyRepository.GetByIdAsync(id, cancellationToken);
            if (company == null)
                return NotFound();

            return company;
        }

        [HttpGet("findByName/{name}")]
        public async Task<ActionResult<Company[]>> GetByNameAsync(
            [FromRoute] string name,
            [FromServices] CompanyRepository companyRepository,
            CancellationToken cancellationToken)
        {
            await using var session = await _dbSessionFactory.CreateSessionAsync(cancellationToken);
            return await companyRepository.GetByNameAsync(name, cancellationToken);
        }

        [HttpGet("getReport")]
        public async Task<ActionResult<CompanyReportDto[]>> GetReportAsync(
            [FromServices] CompanyReportDtoRepository companyReportDtoRepository,
            CancellationToken cancellationToken)
        {
            await using var session = await _dbSessionFactory.CreateSessionAsync(cancellationToken);
            return await companyReportDtoRepository.GetAsync(cancellationToken);
        }

        [HttpPost]
        public async Task<ActionResult<Company>> InsertAsync(
            [FromBody] Company company,
            [FromServices] CompanyRepository companyRepository,
            CancellationToken cancellationToken)
        {
            await using var session = await _dbSessionFactory.CreateCommittableSessionAsync(cancellationToken);
            await companyRepository.InsertAsync(company, cancellationToken);
            await session.CommitAsync(cancellationToken);
            return company;
        }

        [HttpPut]
        public async Task<ActionResult<long>> UpdateAsync(
            [FromBody] Company company,
            [FromServices] CompanyRepository companyRepository,
            CancellationToken cancellationToken)
        {
            await using var session = await _dbSessionFactory.CreateCommittableSessionAsync(cancellationToken);
            await companyRepository.UpdateAsync(company, cancellationToken);
            await session.CommitAsync(cancellationToken);
            return Ok();
        }

        [HttpPost("setInn")]
        public async Task<ActionResult> UpdateInnAsync(
            [FromBody] UpdateInnRequestDto requestDto,
            [FromServices] CompanyRepository companyRepository,
            CancellationToken cancellationToken)
        {
            await using var session = await _dbSessionFactory.CreateCommittableSessionAsync(cancellationToken);
            await companyRepository.UpdateInnAsync(requestDto.Id, requestDto.Inn, cancellationToken);
            await session.CommitAsync(cancellationToken);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(
            [FromRoute] long id,
            [FromServices] CompanyRepository companyRepository,
            CancellationToken cancellationToken)
        {
            await using var session = await _dbSessionFactory.CreateCommittableSessionAsync(cancellationToken);
            await companyRepository.DeleteByIdAsync(id, cancellationToken);
            await session.CommitAsync(cancellationToken);
            return Ok();
        }
    }
}
