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
app.MapGet("api/expenses",
            (string? category,int page = 1, int pageSize = 10,
            string? sortBy = "Id",string? sortOrder = "asc") =>
{
    if (page < 1)
    {
        return Results.BadRequest(
            new {error = "Page must be at least 1."}
        );
    }

    if (pageSize <1 || pageSize > 100)
    {
        return Results.BadRequest(
            new {error = "PageSize must be between 1 and 100."}
        );
    }

    var sortField = (sortBy ?? "Id").Trim().ToLowerInvariant();
    var order = (sortOrder ?? "asc").Trim().ToLowerInvariant();

    if (sortField != "id" &&
        sortField != "title" &&
        sortField != "amount")
    {
        return Results.BadRequest(
            new {error = "SortBy must be id,title or amount."}
        );
    }

    if (order != "asc" && order != "desc")
    {
        return Results.BadRequest(
            new {error = "SortOrder must be asc or desc."}
        );
    }

    IEnumerable<Expenses> query = expenses;
    if (!string.IsNullOrWhiteSpace(category))
    {
        query = query.Where(
            e => string.Equals(
                e.Category,
                category,
                StringComparison.OrdinalIgnoreCase
            )
        );
    }

    var totalCount = query.Count();

    if (sortField == "amount")
    {
        query = order == "asc"
        ? query.OrderBy(e => e.Amount)
        : query.OrderByDescending(e => e.Amount);
    }
    else if (sortField == "title")
    {
        query = order == "asc"
        ? query.OrderBy(e => e.Title)
        : query.OrderByDescending(e => e.Title);
    }
    else if (sortField == "id")
    {
        query = order == "asc"
        ? query.OrderBy(e => e.Id)
        : query.OrderByDescending(e => e.Id);
    }

    var pageExpenses = query
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToList();

    return Results.Ok(
        new
        {
            page,
            pageSize,
            totalCount,
            totalPages = (int)Math.Ceiling(
                totalCount / (double) pageSize
            ),
            sortBy = sortField,
            sortOrder = order,
            data = pageExpenses
        }
    );
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
