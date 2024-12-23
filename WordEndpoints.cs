using Microsoft.EntityFrameworkCore;

namespace CraftOfWord.Endpoints
{
    public static class WordEndpoints
    {
        public static void RegisterWordEndpoints(this WebApplication app)
        {
            // Add word
            app.MapPost("/words", async (Word word, CraftOfWordContext db) =>
            {
                var roundExists = await db.Round.AnyAsync(r => r.Id == word.RoundId);
                var userExists = await db.User.AnyAsync(u => u.Id == word.UserId);

                if (!roundExists || !userExists)
                {
                    return Results.NotFound();
                }

                db.Word.Add(word);
                await db.SaveChangesAsync();

                return Results.Created($"/words/{word.Id}", word);
            })
            .WithOpenApi(operation =>
            {
                operation.Summary = "AddWord";
                operation.Description = "Adds a new word to a round by a user.";
                return operation;
            })
            .Produces<Word>(StatusCodes.Status201Created, "application/json")
            .Produces(StatusCodes.Status404NotFound);

            // Get word data
            app.MapGet("/words/{id}", async (int id, CraftOfWordContext db) =>
            {
                var word = await db.Word.FindAsync(id);

                if (word == null)
                {
                    return Results.NotFound();
                }

                return Results.Ok(word);
            })
            .WithOpenApi(operation =>
            {
                operation.Summary = "GetWordData";
                operation.Description = "Retrieves data of a specific word.";
                return operation;
            })
            .Produces<Word>(StatusCodes.Status200OK, "application/json")
            .Produces(StatusCodes.Status404NotFound);
        }
    }
}
