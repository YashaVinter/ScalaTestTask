using Newtonsoft.Json;
using ScalaTestTask.extensions;
using ScalaTestTask.models;
using ScalaTestTask.services.implementations;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext();
builder.Services.AddTransient<OilPriceStatistics>();
var app = builder.Build();

// add seed data to db
using (var scope = app.Services.CreateScope())
{
    AppDBContext context = scope.ServiceProvider.GetRequiredService<AppDBContext>();
    // TODO read from json
    string href = @"https://data.gov.ru/sites/default/files/otkrytye_dannye_-_cena_na_neft_25.csv";
     string dataPath = @"C:\Users\User\source\repos\ScalaTestTask\resources\data\urals.csv";
    var seedData = new OilDataCSV(dataPath);
    context.Init(seedData.OilPrices);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

//app.MapGet("/weatherforecast", () =>
//{
//    var forecast = Enumerable.Range(1, 5).Select(index =>
//       new WeatherForecast
//       (
//           DateTime.Now.AddDays(index),
//           Random.Shared.Next(-20, 55),
//           summaries[Random.Shared.Next(summaries.Length)]
//       ))
//        .ToArray();
//    return forecast;
//})
//.WithName("GetWeatherForecast");

//async Task GetPrices(HttpContext context, RequestDelegate next)
//{
//    string d1 = context.Request.Query["startdate"];
//    string d2 = context.Request.Query["enddate"];
//    await next.Invoke(context);
//}
app.MapGet("/test/{string?}", (string s) => 
{
    return Results.Text(s);
});
app.MapGet("/statistics/price", All);
app.MapGet("/statistics/price/{date:datetime}", PriceByDate);
//app.Use(GetPrices); 
app.MapGet("/statistics/avgprice/{start_date}/{end_date}", AveragePrice);
app.MapGet("/statistics/minmaxprice/{start_date}/{end_date}", MinMaxPrice);

app.Run();



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
        // TODO add validation service
        startDate = DateTime.Parse(startDateString);
        endDate = DateTime.Parse(endDateString);
        if (startDate > endDate)
        {
            throw new ArgumentException("Incorrect date order");
        }
        var oilStats = app.Services.GetRequiredService<OilPriceStatistics>();
        var avgPrice =  oilStats.AveragePrice(new DateTimeRange { Start = startDate, End = endDate });
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

internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}