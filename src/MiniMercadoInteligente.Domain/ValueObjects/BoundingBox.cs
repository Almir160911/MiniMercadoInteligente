namespace MiniMercadoInteligente.Domain.ValueObjects;

public sealed class BoundingBox
{
    public decimal X { get; private set; }
    public decimal Y { get; private set; }
    public decimal Width { get; private set; }
    public decimal Height { get; private set; }

    public BoundingBox(decimal x, decimal y, decimal width, decimal height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }
}