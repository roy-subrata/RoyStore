﻿namespace Api.Entities;

public class SaleItem:BaseEntity
{
    public int SaleId { get; set; }
    public Sale Sale { get; set; } = null!;

    public string ProductId { get; set; }
    
    public Product Product { get; set; } = null!;

    public int Quantity { get; set; }
    
    public decimal UnitPrice { get; set; }
    
    public DateTime SaleDate => Sale.SaleDate;
    
}