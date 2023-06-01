using System;
using Spalce.Common.Classes;

namespace Spalce.Catalog.Service.Entities;

public class Sale : IEntity
{
    public Guid Id { get; set; }
    public string Price { get; set; }
}
