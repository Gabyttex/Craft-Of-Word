using Microsoft.EntityFrameworkCore;

namespace CraftOfWord.Endpoints
{
    public static class TournamentEndpoints
    {
        public static void RegisterTournamentEndpoints(this WebApplication app)
        {
            // Create new tournament
            app.MapPost("/tournaments", async (Tournament tournament, CraftOfWordContext db) =>
            {
                db.Tournament.Add(tournament);
                await db.SaveChangesAsync();

                return Results.Created($"/tournaments/{tournament.Id}", tournament);
            })
            .WithOpenApi(operation =>
            {
                operation.Summary = "CreateTournament";
                operation.Description = "Creates a new tournament.";
                return operation;
            })
            .Produces<Tournament>(StatusCodes.Status201Created, "application/json")
            .ProducesValidationProblem(StatusCodes.Status400BadRequest);

            // View tournament details
            app.MapGet("/tournaments/{id}", async (int id, CraftOfWordContext db) =>
            {
                var tournament = await db.Tournament
                    .Include(t => t.Participants)
                    .Include(t => t.Rounds)
                    .FirstOrDefaultAsync(t => t.Id == id);

                if (tournament == null)
                {
                    return Results.NotFound();
                }

                return Results.Ok(tournament);
            })
            .WithOpenApi(operation =>
            {
                operation.Summary = "ViewTournamentDetails";
                operation.Description = "Retrieves details of a specific tournament.";
                return operation;
            })
            .Produces<Tournament>(StatusCodes.Status200OK, "application/json")
            .Produces(StatusCodes.Status404NotFound);

            // Get tournament round words
            app.MapGet("/tournaments/{id}/rounds/{roundId}/words", async (int id, int roundId, CraftOfWordContext db) =>
            {
                var tournament = await db.Tournament
                    .FirstOrDefaultAsync(t => t.Id == id);

                if (tournament == null)
                {
                    return Results.NotFound();
                }

                var round = await db.Round
                    .Include(r => r.Words)
                    .FirstOrDefaultAsync(r => r.Id == roundId && r.TournamentId == tournament.Id);

                if (round == null)
                {
                    return Results.NotFound();
                }

                return Results.Ok(round.Words);
            })
            .WithOpenApi(operation =>
            {
                operation.Summary = "GetTournamentRoundWords";
                operation.Description = "Retrieves list of words of a specific round in the tournament.";
                return operation;
            })
            .Produces<List<Word>>(StatusCodes.Status200OK, "application/json")
            .Produces(StatusCodes.Status404NotFound);

            // Update tournament with winner
            app.MapPut("/tournaments/{id}/winner", async (int id, int winnerId, CraftOfWordContext db) =>
            {
                var tournament = await db.Tournament.FindAsync(id);
                var user = await db.User.FindAsync(winnerId);

                if (tournament == null || user == null)
                {
                    return Results.NotFound();
                }

                tournament.WinnerId = winnerId;
                await db.SaveChangesAsync();

                return Results.NoContent();
            })
            .WithOpenApi(operation =>
            {
                operation.Summary = "UpdateTournamentWinner";
                operation.Description = "Updates a tournament with the winner.";
                return operation;
            })
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

            // Add participant to tournament (modified to accept a single userId)
            app.MapPost("/tournaments/{id}/participants", async (int id, int userId, CraftOfWordContext db) =>
            {
                var tournament = await db.Tournament.Include(t => t.Participants).FirstOrDefaultAsync(t => t.Id == id);
                var user = await db.User.FindAsync(userId);

                if (tournament == null || user == null)
                {
                    return Results.NotFound();
                }

                if (!tournament.Participants.Contains(user))
                {
                    tournament.Participants.Add(user);
                    await db.SaveChangesAsync();
                }

                return Results.NoContent();
            })
            .WithOpenApi(operation =>
            {
                operation.Summary = "AddParticipantToTournament";
                operation.Description = "Adds a participant to a tournament.";
                return operation;
            })
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

            // Delete a tournament
            app.MapDelete("/tournaments/{id}", async (int id, CraftOfWordContext db) =>
            {
                var tournament = await db.Tournament.FindAsync(id);

                if (tournament == null)
                {
                    return Results.NotFound();
                }

                db.Tournament.Remove(tournament);
                await db.SaveChangesAsync();

                return Results.NoContent();
            })
            .WithOpenApi(operation =>
            {
                operation.Summary = "DeleteTournament";
                operation.Description = "Deletes a tournament by ID.";
                return operation;
            })
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);
        }
    }
}
