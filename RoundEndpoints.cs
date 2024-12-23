using Microsoft.EntityFrameworkCore;

namespace CraftOfWord.Endpoints
{
    public static class RoundEndpoints
    {
        public static void RegisterRoundEndpoints(this WebApplication app)
        {
            // Add round
            app.MapPost("/tournaments/{tournamentId}/rounds", async (int tournamentId, Round round, CraftOfWordContext db) =>
            {
                var tournament = await db.Tournament.FindAsync(tournamentId);

                if (tournament == null)
                {
                    return Results.NotFound();
                }

                round.TournamentId = tournamentId;
                db.Round.Add(round);
                await db.SaveChangesAsync();

                return Results.Created($"/rounds/{round.Id}", round);
            })
            .WithOpenApi(operation =>
            {
                operation.Summary = "AddRound";
                operation.Description = "Adds a new round to a tournament.";
                return operation;
            })
            .Produces<Round>(StatusCodes.Status201Created, "application/json")
            .Produces(StatusCodes.Status404NotFound);

            // Edit round data
            app.MapPut("/rounds/{id}", async (int id, Round updatedRound, CraftOfWordContext db) =>
            {
                var round = await db.Round.FindAsync(id);

                if (round == null)
                {
                    return Results.NotFound();
                }

                round.StartTime = updatedRound.StartTime;
                round.Length = updatedRound.Length;

                await db.SaveChangesAsync();

                return Results.NoContent();
            })
            .WithOpenApi(operation =>
            {
                operation.Summary = "EditRoundData";
                operation.Description = "Edits data of a specific round.";
                return operation;
            })
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

            // Get round data
            app.MapGet("/rounds/{id}", async (int id, CraftOfWordContext db) =>
            {
                var round = await db.Round
                    .Include(r => r.Words)
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (round == null)
                {
                    return Results.NotFound();
                }

                return Results.Ok(round);
            })
            .WithOpenApi(operation =>
            {
                operation.Summary = "GetRoundData";
                operation.Description = "Retrieves data of a specific round.";
                return operation;
            })
            .Produces<Round>(StatusCodes.Status200OK, "application/json")
            .Produces(StatusCodes.Status404NotFound);
        }
    }
}
