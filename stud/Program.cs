using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using stud.Common.Behaviors;
using stud.Data;

namespace stud;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Student API",
                Version = "v1"
            });
        });

        builder.Services.AddControllers();

        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

        builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAngular", policy =>
            {
                policy.WithOrigins("http://localhost:4200")
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            dbContext.Database.Migrate();
        }

        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                var exceptionHandlerFeature =
                    context.Features.Get<IExceptionHandlerFeature>();

                var exception = exceptionHandlerFeature?.Error;

                if (exception is ValidationException validationException)
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    context.Response.ContentType = "application/json";

                    var errors = validationException.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(
                            group => group.Key,
                            group => group.Select(e => e.ErrorMessage).ToArray());

                    await context.Response.WriteAsJsonAsync(new
                    {
                        Message = "Validation failed",
                        Errors = errors
                    });

                    return;
                }

                if (exception is KeyNotFoundException keyNotFoundException)
                {
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    context.Response.ContentType = "application/json";

                    await context.Response.WriteAsJsonAsync(new
                    {
                        Message = keyNotFoundException.Message
                    });

                    return;
                }

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsJsonAsync(new
                {
                    Message = "An unexpected error occurred."
                });
            });
        });

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Student API V1");
            });
        }

        app.UseHttpsRedirection();
        app.UseCors("AllowAngular");
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}
