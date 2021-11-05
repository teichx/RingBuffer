using RabbitMQ.Client;
using TingTuffer.Base;
using TingTuffer.Examples.DefaultApi.Controllers;
using TingTuffer.Examples.DefaultApi.RingBuffer;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add services to the container.
PrepararFilas();
var ringBufferRabbitMQ = new RingBufferRabbitMQ();
builder.Services.AddSingleton<IRingBufferBase<IModel>>(ringBufferRabbitMQ);

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "DefaultApi v1"));
}


app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();

void PrepararFilas()
{
    using var conexao = new ConnectionFactory()
        .CreateConnection()
        .CreateModel();

    conexao.QueueDeclare(TingTufferController.QueueWithRingBuffer, false, true, true, null);
    conexao.QueueBind(TingTufferController.QueueWithRingBuffer, TingTufferController.TopicName, TingTufferController.QueueWithRingBuffer, null);

    conexao.QueueDeclare(TingTufferController.QueueWithoutRingBuffer, false, true, true, null);
    conexao.QueueBind(TingTufferController.QueueWithoutRingBuffer, TingTufferController.TopicName, TingTufferController.QueueWithoutRingBuffer, null);
}
