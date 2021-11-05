# TingTuffer - um buffer circular para .net

- [to english version click here](./README.md)

## Conceito

Primeiramente, esta biblioteca não se apega a nomear precisamente o metodo implementado. De fato, há uma maneira precisa para nomear a implementação dessa biblioteca, mas é importante ressaltar que ela poderia nomeada como "buffer anelar", "buffer circular", "fila circular" ou "buffer ciclico". Esse não é o principal ponto, principalmente, porque isso se refere ao modo em que foi implementado internamente, e assim como qualquer biblioteca, esse não deve ser o ponto principal, mas sim, como usar, e para quais finalidades.

### Como funciona

O conceito principal envolvendo esta estrutura de dados, é deixar dados que serão usados futuramente já prontos para o uso.
- Ao instanciar a classe, os items são preparados para uso.
- Um item não pode ser usado em dois lugares ao mesmo tempo.
- Caso um item depois do uso, não seja mais reaproveitavel, deve ser substituído por outro.
- Prover um uso circular, exemplo: Caso um item seja usado, e possa ser reaproveitado, ele só deve usado novamente após o uso de todos os outros items do buffer.

## Objetivo

O objetivo de um buffer circular, depende do seu contexto, mas os principais usos podem ser descritos como: 
- Uso para reservar espaços espeficos e reaproveitaveis de memória (O que de forma geral, não é um problema para a maioria das aplicações).
- Uso para gerenciar conexões (E foi pensando nisso, que esta biblioteca foi criada).

### Sobre o gerenciamento de conexões

O uso do buffer circular para gerenciamento de conexões pode acelerar significamente conexões com provedores externos, caso você tenha uma comunicação com um provedor de mensageria, como rabbitmq, simple queue service da AWS, ou uma conexão com bucket S3, e até mesmo as conexões com o banco de dados, (caso você esteja trabalhando com uma API que inicia uma nova conexão a cada requisição, como injeção de depencia com `AddContext`, ou `AddScoped`).

## Como usar

Há algumas formas de uso mais comuns, caso queira ver de forma detalhada, neste repositório podem ser encontradas implementações que satisfazem as condições abaixo com mais detalhes.

### Como usar com RabbitMQ

1. Herdar a classe responsavel por abstrair o buffer circular.

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

2. Preparar o `Program.cs` para a injeção de dependencia.

    2.1 Neste passo, é importante criar a instancia antes de adicionar no `AddSingleton`, porque desta forma, você garante que na ao iniciar sua aplicação, o seu buffer estará cheio. Do contrário, ele tentaria preenchelo após chegar a primeira requisição de uso.

``` csharp
var ringBufferRabbitMQ = new RingBufferRabbitMQ();
builder.Services.AddSingleton<IRingBufferBase<IModel>>(ringBufferRabbitMQ);
```

3. Usar onde precisar

    3.1 Receba através da injeção de depencia no seu construtor.

``` csharp
readonly IRingBufferBase<IModel> RingBufferRabbitMQ;

public TingTufferController(IRingBufferBase<IModel> ringBufferRabbitMQ)
    => RingBufferRabbitMQ = ringBufferRabbitMQ;

```
  - 3.2 Use no seu metodo.

``` csharp
[HttpPost]
public IActionResult PublishInQueue([FromBody] dynamic body)
{
    using var item = RingBufferRabbitMQ.GetItem();

    item.Item.BasicPublish(TopicName, QueueWithRingBuffer, true, null, JsonSerializer.SerializeToUtf8Bytes(body));

    return Ok();
}
```

