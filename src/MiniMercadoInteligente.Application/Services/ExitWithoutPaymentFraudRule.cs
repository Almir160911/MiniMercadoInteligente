using MiniMercadoInteligente.Domain.Entities;

namespace MiniMercadoInteligente.Application.Services;

public class ExitWithoutPaymentFraudRule : IFraudRule
{
    public string Name => "ExitWithoutPayment";

    public Task<IReadOnlyList<FraudFlag>> EvaluateAsync(FraudContext context, CancellationToken ct)
    {
        var hasItems = context.CartItems.Any();
        var paid = context.LastPayment?.Status == PaymentStatus.Paid;

        if (!hasItems || paid)
            return Task.FromResult<IReadOnlyList<FraudFlag>>(Array.Empty<FraudFlag>());

        IReadOnlyList<FraudFlag> flags =
        [
            new FraudFlag(
                "EXIT_WITHOUT_PAYMENT",
                "Sessão contém itens mas não há pagamento concluído.",
                40,
                "HIGH",
                new
                {
                    cartQty = context.CartItems.Sum(x => x.Qty),
                    paymentStatus = context.LastPayment?.Status.ToString()
                })
        ];

        return Task.FromResult(flags);
    }
}