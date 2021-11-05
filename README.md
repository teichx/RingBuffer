# TingTuffer - a ring buffer for .net

- [para a versão em portugues, clique aqui](./README-pt-BR.md)

## Concept

The purpose of this library is not to name precisely the implemented method, this can change. Independent if you know has "ring buffer", "circular buffer", "circular queue" or "cyclic buffer". This is not the main point, because it refers to the way in which it was implemented internally, and like any library, this should not be the main point, but rather, how to use it, and for what purposes.

### How it works

The main concept involving this data structure is to leave data that will be used in the future ready for use.
- When instantiating the class, the items are prepared for use.
- An item cannot be used in two places at the same time.
- If an item is no longer reusable after use, it must be replaced by another.
- Should a circular use, example: If an item is used, and can be reused, it should only be used again after using all the other items in the buffer.

## Purpose

The purpose of a ring buffer depends on its context, but the most common uses are:
- Use to reserve specific and reusable memory ​​spaces (Which is generally not a problem for most applications).
- Use to manager connections (And it was thinking about that, that this library was created).

### About connection manager

Using ring buffer for connection management can significantly speed up connections to external providers, if you have a communication with a messaging provider, such as rabbitmq, AWS simple queue service, or a connection to bucket S3, and even connections to the database, (if you are working with an API that initiates a new connection on every request, like dependency injection with `AddContext`, or `AddScoped`).

## How to use

There are some more common usages, in case you want to see in detail, in this repository you can find implementations that satisfy the conditions below in more detail.

### How to use with RabbitMQ

1. Inherit the `RingBufferBase` in your class, and implement `CreateFactory` and `Validate` methods.

``` csharp
using RabbitMQ.Client;
using TingTuffer.Base;

public class RingBufferRabbitMQ : RingBufferBase<IModel>
{
    public RingBufferRabbitMQ() { }
    public RingBufferRabbitMQ(int size) : base(size) { }

    protected override IModel CreateFactory()
      => new ConnectionFactory()
          .CreateConnection()
          .CreateModel();

    protected override bool Validate(IModel item)
      => item is not null && item.IsOpen;
}
```

2. Prepare `Program.cs` for dependency injection.

    2.1 In this step, is most important to create the instance before add in `AddSingleton`, because this way you guarantee that at application startup, the buffer will be filled. Otherwise, the filling would be during the first attempt to use

``` csharp
var ringBufferRabbitMQ = new RingBufferRabbitMQ();
builder.Services.AddSingleton<IRingBufferBase<IModel>>(ringBufferRabbitMQ);
```

3. Use where you need

    3.1 Receive in the constructor by dependency injection

``` csharp
readonly IRingBufferBase<IModel> RingBufferRabbitMQ;

public TingTufferController(IRingBufferBase<IModel> ringBufferRabbitMQ)
    => RingBufferRabbitMQ = ringBufferRabbitMQ;

```
  - 3.2 Use in your method.

``` csharp
[HttpPost]
public IActionResult PublishInQueue([FromBody] dynamic body)
{
    using var item = RingBufferRabbitMQ.GetItem();

    item.Item.BasicPublish(TopicName, QueueWithRingBuffer, true, null, JsonSerializer.SerializeToUtf8Bytes(body));

    return Ok();
}
```

