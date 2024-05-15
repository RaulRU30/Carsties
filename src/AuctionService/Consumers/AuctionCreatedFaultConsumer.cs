using Contracts;
using MassTransit;

namespace AuctionService;

public class AuctionCreatedFaultConsumer : IConsumer<Fault<AuctionCreated>>
{
    public async Task Consume(ConsumeContext<Fault<AuctionCreated>> context)
    {
        Console.WriteLine("---> Auction created fault: {0}", context.Message.Message.Id);

        var exception = context.Message.Exceptions.First();

        if (exception.ExceptionType == "System.ArgumentException")
        {
            context.Message.Message.Model = "FooBar";
            await context.Publish<AuctionCreated>(context.Message.Message);
        }
        else
        {
            Console.WriteLine("Unhandled exception: {0}", exception.ExceptionType);
        }
    }
}
