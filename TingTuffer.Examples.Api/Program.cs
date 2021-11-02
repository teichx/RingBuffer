using TingTuffer.Base;
using TingTuffer.Examples.Api.Publisher;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var tingTuffer = new TingTufferPublisher();
builder.Services.AddSingleton<IRingBufferBase<Publisher>>(tingTuffer);
builder.Services.AddScoped<Publisher>();

Publisher.CreateQueue();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapGet("/with/ting-tuffer", async (IRingBufferBase<Publisher> TingTufferBase) =>
{
    using var item = TingTufferBase.GetItem();

    return await item.Item.PublishWithTingTuffer(Guid.NewGuid());
});

app.MapGet("/without/ting-tuffer", async (Publisher publisher) 
    => await publisher.PublishWithoutTingTuffer(Guid.NewGuid()));


app.Run();
