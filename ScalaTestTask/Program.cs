using Newtonsoft.Json;
using ScalaTestTask.extensions;
using ScalaTestTask.models;
using ScalaTestTask.services.implementations;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext();
builder.Services.AddTransient<OilPriceStatistics>();
builder.Services.AddTransient<DateTimeRangeValidation>();
var app = builder.Build();

// add seed data to db
using (var scope = app.Services.CreateScope())
{
    AppDBContext context = scope.ServiceProvider.GetRequiredService<AppDBContext>();
    var seedData = new OilDataCSV(new Uri(app.Configuration["DataBase:URI"]), app.Configuration["DataBase:CsvPath"]);
    context.Init(seedData.OilPrices);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/statistics/price", All);
app.MapGet("/statistics/price/{date:datetime}", PriceByDate);
app.MapGet("/statistics/avgprice/{start_date}/{end_date}", AveragePrice);
app.MapGet("/statistics/minmaxprice/{start_date}/{end_date}", MinMaxPrice);

app.Run();

// Map delegate handlers
IResult All()
{
    var oilStats = app.Services.GetRequiredService<OilPriceStatistics>();
    return Results.Ok(oilStats.AllOilRecords());
}
IResult PriceByDate(DateTime date) 
{
    var oilStats = app.Services.GetRequiredService<OilPriceStatistics>();

    if (oilStats.PriceByDate(date) is decimal price)
    {
        return Results.Ok(price);
    }
    return Results.NotFound();
}
IResult AveragePrice(string startDateString, string endDateString) 
{
    DateTime startDate, endDate;
    try
    {
        var oilStats = app.Services.GetRequiredService<OilPriceStatistics>();
        var validation = app.Services.GetRequiredService<DateTimeRangeValidation>();

        var dateRange = validation.Validate(startDateString, endDateString);
        var avgPrice =  oilStats.AveragePrice(dateRange);
        return Results.Ok(avgPrice);
    }    
    catch (FormatException ex)
    {
        return Results.BadRequest(new { statusCode = StatusCodes.Status400BadRequest, error = ex.Message });
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { statusCode = StatusCodes.Status400BadRequest, error = ex.Message });
    }
}
IResult MinMaxPrice(string startDateString, string endDateString) 
{
    DateTime startDate, endDate;
    try
    {
        // TODO add validation service
        startDate = DateTime.Parse(startDateString);
        endDate = DateTime.Parse(endDateString);
        if (startDate > endDate)
        {
            throw new ArgumentException("Incorrect date order");
        }
        var oilStats = app.Services.GetRequiredService<OilPriceStatistics>();
        var avgPrice = oilStats.MinMaxPrice(new DateTimeRange { Start = startDate, End = endDate });
        return Results.Ok(avgPrice);
    }
    catch (FormatException ex)
    {
        return Results.BadRequest(new { statusCode = StatusCodes.Status400BadRequest, error = ex.Message });
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { statusCode = StatusCodes.Status400BadRequest, error = ex.Message });
    }
}
