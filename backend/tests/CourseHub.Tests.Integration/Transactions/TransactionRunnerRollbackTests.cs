using CourseHub.Application.Interfaces;
using CourseHub.Domain.Entities;
using CourseHub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CourseHub.Tests.Integration.Transactions;

public sealed class TransactionRunnerRollbackTests : IClassFixture<ApiFactory>
{
    private readonly ApiFactory _factory;

    public TransactionRunnerRollbackTests(ApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task ExecuteAsync_WhenActionWritesThenThrows_RollsBackCommittedChanges()
    {
        await _factory.ResetDbAsync();

        using var scope = _factory.Services.CreateScope();
        var runner = scope.ServiceProvider.GetRequiredService<ITransactionRunner>();
        var db = scope.ServiceProvider.GetRequiredService<CourseHubDbContext>();

        Assert.Equal(0, await db.Courses.CountAsync());

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            runner.ExecuteAsync(async ct =>
            {
                db.Courses.Add(new Course
                {
                    CourseCode = "TX-ROLLBACK-1",
                    Title = "Rollback Test",
                    Description = "Should be rolled back"
                });

                await db.SaveChangesAsync(ct);

                throw new InvalidOperationException("Boom");
            }));

        Assert.Equal(0, await db.Courses.CountAsync());
    }
}
