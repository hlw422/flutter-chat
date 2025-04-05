using flutter_chat_backend.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// �����־��¼
builder.Logging.AddConsole();

builder.WebHost.UseUrls("http://*:5133");

// ��� SignalR ����
builder.Services.AddSignalR();

builder.Services.AddCors();

builder.Services.AddControllers();

var app = builder.Build();

app.UseCors((opt =>
    opt.AllowAnyHeader()
    .AllowAnyMethod()
    .AllowAnyOrigin()
));

// ȫ���쳣�����м��
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "text/plain";

        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        if (exceptionHandlerPathFeature?.Error != null)
        {
            app.Logger.LogError(exceptionHandlerPathFeature.Error, "����δ������쳣");
            await context.Response.WriteAsync("�������ڲ��������������Ժ����ԡ�");
        }
    });
});


app.UseAuthorization();

app.MapHub<ChatHub>("/chatHub");

app.MapControllers();

app.Run();