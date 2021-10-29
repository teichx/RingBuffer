using RingBuffer.Base;
using RingBuffer.Examples.Api.Publisher;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IRingBufferBase<Publisher>>(x => new RingBufferPublisher());
builder.Services.AddScoped<Publisher>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapGet("/ao/ba", async (IRingBufferBase<Publisher> ringBufferBase) =>
{
    using var item = ringBufferBase.GetItem();

    return await ExecuteOperation(item.Item);
});

app.MapGet("/foo/bar", async (Publisher publisher) 
    => await ExecuteOperation(publisher));

async ValueTask<bool> ExecuteOperation(Publisher publisher)
    => await publisher.Publish(Guid.NewGuid());


app.Run();
