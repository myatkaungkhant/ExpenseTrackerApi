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
// app.MapGet("api/expenses",() =>
// {
//     return Results.Ok(expenses);
// });
#pragma warning restore ASP0022 // Route conflict detected between route handlers

// Get expenses by ID
app.MapGet("api/expenses/{id}",(int id) =>
{
    var expense = expenses.FirstOrDefault(e => e.Id == id);
    IResult result;
    if (expense == null)
    {
        result = Results.NotFound(
            new {error = "Expense not found"}
        );
    }else
    {
        result = Results.Ok(expense);
    }
    return result;
});

// Filter expenses by category
app.MapGet("api/expenses",(string? category) =>
{
    IEnumerable<Expenses> filterExpenses = expenses;
    if (!string.IsNullOrWhiteSpace(category))
    {
        filterExpenses = filterExpenses.Where(
            e => string.Equals(
                e.Category,
                category,
                StringComparison.OrdinalIgnoreCase
            )
        );
    }

    return Results.Ok(filterExpenses.ToList());
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

// Delete an expense
app.MapDelete("api/expenses/{id}",(int id) =>
{
    var expense = expenses.FirstOrDefault(e => e.Id == id);

    if (expense == null)
    {
        return Results.NotFound(
            new {error = $"Expense with ID {id} not found"}
        );
    }

    expenses.Remove(expense);

    return Results.NoContent();
});

// Update an expense
app.MapPut("api/expenses/{id}",(int id, Expenses updatedExpense) =>
{
    var existExpense = expenses.FirstOrDefault(e => e.Id == id);

    if(existExpense == null)
    {
        return Results.NotFound(
            new {error = $"Expense with ID {id} is not found."}
        );
    }

    // Add Rules
    // Rule 1: Title should not be empty
    if (string.IsNullOrWhiteSpace(updatedExpense.Title))
    {
        return Results.BadRequest(
            new {error = "Title is required"}
        );
    }

    // Rule 2: Amount should not be empty
    if (updatedExpense.Amount == null)
    {
        return Results.BadRequest(
            new {error = "Amount is required"}
        );
    }

    // Rule 3: Amount must be greater than 0
    if (updatedExpense.Amount <= 0)
    {
        return Results.BadRequest(
            new {error = "Amount must be a positive number"}
        );
    }

    // Rule 4: Category should not be empty
    if (string.IsNullOrWhiteSpace(updatedExpense.Category))
    {
        return Results.BadRequest(
            new {error = "Category is required"}
        ); 
    }

    existExpense.Title = updatedExpense.Title;
    existExpense.Amount = updatedExpense.Amount;
    existExpense.Category = updatedExpense.Category;

    return Results.Ok(existExpense);
});

app.Run();
