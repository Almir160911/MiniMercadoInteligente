namespace MiniMercadoInteligente.Domain.Entities; public class ProductPrice{
    public Guid ProductPriceId{get;set;}
    public Guid ProductId{get;set;}
    public decimal Price{get;set;}
    public string Currency{get;set;}="BRL";
    public DateTime EffectiveAtUtc{get;set;}=DateTime.UtcNow;

    
    public bool Active{get;set;}=true;}