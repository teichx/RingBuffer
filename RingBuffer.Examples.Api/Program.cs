using RingBuffer.Base;
using RingBuffer.Examples.Api.Publisher;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var ringBuffer = new RingBufferPublisher();
builder.Services.AddSingleton<IRingBufferBase<Publisher>>(ringBuffer);
builder.Services.AddScoped<Publisher>();

Publisher.CreateQueue();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapGet("/with/ring-buffer", async (IRingBufferBase<Publisher> ringBufferBase) =>
{
    using var item = ringBufferBase.GetItem();

    return await item.Item.PublishWithRingBuffer(Guid.NewGuid());
});

app.MapGet("/without/ring-buffer", async (Publisher publisher) 
    => await publisher.PublishWithoutRingBuffer(Guid.NewGuid()));


app.Run();
