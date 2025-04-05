using flutter_chat_backend.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// 添加日志记录
builder.Logging.AddConsole();

builder.WebHost.UseUrls("http://*:5133");

// 添加 SignalR 服务
builder.Services.AddSignalR();

builder.Services.AddCors();

builder.Services.AddControllers();

var app = builder.Build();

app.UseCors((opt =>
    opt.AllowAnyHeader()
    .AllowAnyMethod()
    .AllowAnyOrigin()
));

// 全局异常处理中间件
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "text/plain";

        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        if (exceptionHandlerPathFeature?.Error != null)
        {
            app.Logger.LogError(exceptionHandlerPathFeature.Error, "发生未处理的异常");
            await context.Response.WriteAsync("发生了内部服务器错误，请稍后再试。");
        }
    });
});


app.UseAuthorization();

app.MapHub<ChatHub>("/chatHub");

app.MapControllers();

app.Run();