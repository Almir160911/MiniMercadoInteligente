namespace MiniMercadoInteligente.Domain.Entities; 
public class Area{
public Guid AreaId{get;set;}

public string Name{get;set;}=default!;
public string AreaCode{get;set;}=default!;
public string Type{get;set;}="Shelf";
public string SensorId{get;set;}=default!;
public string CameraId{get;set;}=default!;
public double ConfidenceBase{get;set;}=0.7;
}