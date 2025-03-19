using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using NBomber.CSharp;
using NBomber.Contracts;
using Xunit;

public class LoadTests
{
    [Fact]
    public void RunLoadTest()
    {
        var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();
        
        var scenario = Scenario.Create("http_scenario", async context =>
        {
            var step = await Step.Run("call-endpoint", context, async () =>
            {
                var response = await client.GetAsync("/weatherforecast");

                return response.IsSuccessStatusCode
                    ? Response.Ok()
                    : Response.Fail();
            });
            
            return Response.Ok();
        }).WithLoadSimulations(Simulation.KeepConstant(copies: 5, during: TimeSpan.FromSeconds(30)));

        NBomberRunner
            .RegisterScenarios(scenario)
            .Run();
    }
}