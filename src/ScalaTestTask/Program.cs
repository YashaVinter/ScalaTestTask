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
builder.Services.AddTransient<AccordStatistics>();
builder.Services.AddTransient<DateTimeValidation>();
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

app.MapGet("/statistics/prices", All);
app.MapGet("/statistics/prices/{date}", PriceByDate);
app.MapGet("/statistics/prices/minmax/{start_date}/{end_date}", MinMaxPrice);
app.MapGet("/statistics/prices/average/{start_date}/{end_date}", AveragePrice);
app.MapGet("/statistics/prices/math_expectation/{start_date}/{end_date}", MathExpectationPrice);
app.MapGet("/statistics/prices/variance/{start_date}/{end_date}", PriceVariance);
app.MapGet("/statistics/prices/standard_deviation/{start_date}/{end_date}", PriceStandardDeviation);
app.MapGet("/statistics/prices/linear_regression/{start_date}/{end_date}", PriceLinearRegression);


app.Run();

// Map delegate handlers
IResult All(HttpContext context)
{
    var oilStats = app.Services.GetRequiredService<OilPriceStatistics>();
    app.Logger.LogInformation(message: LogData(context.Request.Path));
    return Results.Ok(oilStats.AllOilRecords());
}
IResult PriceByDate(HttpContext context, string dateString) 
{
    IResult result = Results.BadRequest();
    try
    {
        var validation = app.Services.GetRequiredService<DateTimeValidation>();
        DateTime date = validation.Validate(dateString);

        var oilStats = app.Services.GetRequiredService<OilPriceStatistics>();
        if (oilStats.PriceByDate(date) is decimal price)
        {
            result = Results.Ok(price);
        }    
    }
    catch (FormatException ex)
    {
        result = Results.BadRequest(new { statusCode = StatusCodes.Status400BadRequest, error = ex.Message });
    }
    app.Logger.LogInformation(message: LogData(context.Request.Path));
    return result;
}
IResult MinMaxPrice(HttpContext context, string startDateString, string endDateString)
{
    IResult result;
    try
    {
        var oilStats = app.Services.GetRequiredService<OilPriceStatistics>();
        var validation = app.Services.GetRequiredService<DateTimeValidation>();

        var dateRange = validation.Validate(startDateString, endDateString);
        var minMaxJson = oilStats.MinMaxPrice(dateRange);
        result = Results.Ok(minMaxJson);
    }
    catch (FormatException ex)
    {
        result = Results.BadRequest(new { statusCode = StatusCodes.Status400BadRequest, error = ex.Message });
    }
    catch (ArgumentException ex)
    {
        result = Results.BadRequest(new { statusCode = StatusCodes.Status400BadRequest, error = ex.Message });
    }
    app.Logger.LogInformation(message: LogData(context.Request.Path));
    return result;
}
IResult AveragePrice(HttpContext context, string startDateString, string endDateString) 
{
    IResult result;
    try
    {
        var oilStats = app.Services.GetRequiredService<OilPriceStatistics>();
        var validation = app.Services.GetRequiredService<DateTimeValidation>();

        var dateRange = validation.Validate(startDateString, endDateString);
        var avgPrice =  oilStats.AveragePrice(dateRange);
        result =  Results.Ok(avgPrice);
    }    
    catch (FormatException ex)
    {
        result =  Results.BadRequest(new { statusCode = StatusCodes.Status400BadRequest, error = ex.Message });
    }
    catch (ArgumentException ex)
    {
        result =  Results.BadRequest(new { statusCode = StatusCodes.Status400BadRequest, error = ex.Message });
    }
    app.Logger.LogInformation(message: LogData(context.Request.Path));
    return result;
}
IResult MathExpectationPrice(HttpContext context, string startDateString, string endDateString)
{
    IResult result;
    try
    {
        var oilStats = app.Services.GetRequiredService<OilPriceStatistics>();
        var validation = app.Services.GetRequiredService<DateTimeValidation>();

        var dateRange = validation.Validate(startDateString, endDateString);
        var avgPrice = oilStats.MathExpectationPrice(dateRange);
        result = Results.Ok(avgPrice);
    }
    catch (FormatException ex)
    {
        result = Results.BadRequest(new { statusCode = StatusCodes.Status400BadRequest, error = ex.Message });
    }
    catch (ArgumentException ex)
    {
        result = Results.BadRequest(new { statusCode = StatusCodes.Status400BadRequest, error = ex.Message });
    }
    app.Logger.LogInformation(message: LogData(context.Request.Path));
    return result;
}
IResult PriceVariance(HttpContext context, string startDateString, string endDateString)
{
    IResult result;
    try
    {
        var oilStats = app.Services.GetRequiredService<OilPriceStatistics>();
        var validation = app.Services.GetRequiredService<DateTimeValidation>();

        var dateRange = validation.Validate(startDateString, endDateString);
        var avgPrice = oilStats.PriceVariance(dateRange);
        result = Results.Ok(avgPrice);
    }
    catch (FormatException ex)
    {
        result = Results.BadRequest(new { statusCode = StatusCodes.Status400BadRequest, error = ex.Message });
    }
    catch (ArgumentException ex)
    {
        result = Results.BadRequest(new { statusCode = StatusCodes.Status400BadRequest, error = ex.Message });
    }
    app.Logger.LogInformation(message: LogData(context.Request.Path));
    return result;
}
IResult PriceStandardDeviation(HttpContext context, string startDateString, string endDateString)
{
    IResult result;
    try
    {
        var oilStats = app.Services.GetRequiredService<OilPriceStatistics>();
        var validation = app.Services.GetRequiredService<DateTimeValidation>();

        var dateRange = validation.Validate(startDateString, endDateString);
        var avgPrice = oilStats.PriceStandardDeviation(dateRange);
        result = Results.Ok(avgPrice);
    }
    catch (FormatException ex)
    {
        result = Results.BadRequest(new { statusCode = StatusCodes.Status400BadRequest, error = ex.Message });
    }
    catch (ArgumentException ex)
    {
        result = Results.BadRequest(new { statusCode = StatusCodes.Status400BadRequest, error = ex.Message });
    }
    app.Logger.LogInformation(message: LogData(context.Request.Path));
    return result;
}
IResult PriceLinearRegression(HttpContext context, string startDateString, string endDateString)
{
    IResult result;
    try
    {
        var oilStats = app.Services.GetRequiredService<OilPriceStatistics>();
        var validation = app.Services.GetRequiredService<DateTimeValidation>();

        var dateRange = validation.Validate(startDateString, endDateString);
        var avgPrice = oilStats.PriceLinearRegression(dateRange);
        result = Results.Ok(avgPrice);
    }
    catch (FormatException ex)
    {
        result = Results.BadRequest(new { statusCode = StatusCodes.Status400BadRequest, error = ex.Message });
    }
    catch (ArgumentException ex)
    {
        result = Results.BadRequest(new { statusCode = StatusCodes.Status400BadRequest, error = ex.Message });
    }
    app.Logger.LogInformation(message: LogData(context.Request.Path));
    return result;
}

string LogData(string requestPath) 
{
    var now = DateTime.Now;
    return $"{now.ToShortDateString()}\t{now.ToLongTimeString()}\t Request {requestPath} has been processed";
}