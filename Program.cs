using ExpenseTrackerApi.Models;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

var expenses = new List<Expenses>
{
    new() {
            Id = 1,
            Title = "Lunch",
            Amount = 850,
            Category = "Food"
        },
    new() {
            Id = 2,
            Title = "Train",
            Amount = 320,
            Category = "Transport"
        }
};

// Get all expenses
app.MapGet("api/expenses",() =>
{
    return Results.Ok(expenses);
});

// Post a new expense
app.MapPost("api/expenses", (Expenses expense) =>
{
    // Add Rules
    // Rule 1: Title should not be empty
    if (string.IsNullOrWhiteSpace(expense.Title))
    {
        return Results.BadRequest(
            new {error = "Title is required"}
        );
    }

    // Rule 2: Amount should not be empty
    if (expense.Amount == null)
    {
        return Results.BadRequest(
            new {error = "Amount is required"}
        );
    }

    // Rule 3: Amount must be greater than 0
    if (expense.Amount <= 0)
    {
        return Results.BadRequest(
            new {error = "Amount must be a positive number"}
        );
    }

    // Rule 4: Category should not be empty
    if (string.IsNullOrWhiteSpace(expense.Category))
    {
        return Results.BadRequest(
            new {error = "Category is required"}
        ); 
    }

    expense.Id = expenses.Count == 0 ? 1 : expenses.Max(e => e.Id) + 1;

    expenses.Add(expense);
    return Results.Created(
        $"api/expenses/{expense.Id}",expense
    );
});

app.Run();
