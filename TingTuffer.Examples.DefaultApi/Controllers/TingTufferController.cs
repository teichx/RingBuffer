using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using TingTuffer.Base;
using TingTuffer.Examples.DefaultApi.Dto;

namespace TingTuffer.Examples.DefaultApi.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class TingTufferController : ControllerBase
    {
        public const string TopicName = "amq.direct";
        public const string QueueWithRingBuffer = "queue.with.ting.tuffer";
        public const string QueueWithoutRingBuffer = "queue.without.ting.tuffer";

        readonly IRingBufferBase<IModel> RingBufferRabbitMQ;
        public TingTufferController(IRingBufferBase<IModel> ringBufferRabbitMQ)
            => RingBufferRabbitMQ = ringBufferRabbitMQ;

        [HttpGet]
        public IActionResult With()
        {
            using var item = RingBufferRabbitMQ.GetItem();

            var response = Publish(item.Item, true, Guid.NewGuid());

            return response.Published
                ? Ok(response)
                : BadRequest(response);
        }

        [HttpGet]
        public IActionResult Without()
        {
            using var item = RingBufferRabbitMQ.GetItem();
            var response = Publish(item.Item, false, Guid.NewGuid());

            return response.Published
                ? Ok(response)
                : BadRequest(response);
        }

        static ResponseDto Publish(IModel model, bool useTingTuffer, Guid id)
        {
            var routingKey = useTingTuffer
                ? QueueWithRingBuffer
                : QueueWithoutRingBuffer;
            try
            {
                var body = id.ToByteArray();

                model.BasicPublish(
                    exchange: TopicName,
                    routingKey: routingKey,
                    mandatory: true,
                    basicProperties: null,
                    body: body
                );

                Console.WriteLine($"[{DateTime.UtcNow}] Published [{(useTingTuffer ?  'x' : ' ')}] ring buffer {id}");

                return new ResponseDto(id, true);
            }
            catch (Exception e)
            {
                Console.WriteLine($"[{DateTime.UtcNow}] Exception on publishing in [{routingKey} queue] message {id} | when exception {e.Message}");
                return new ResponseDto(id, false);
            }
        }
    }
}