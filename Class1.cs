using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Data.Entity;

public class Calculation
{
    [Key]
    public int ID { get; set; }

    public double FirstNumber { get; set; }

    public char Operator { get; set; }

    public double SecondNumber { get; set; }

    public double Result { get; set; }
}

public class CalculationContext : DbContext
{
    public CalculationContext() : base("name=CalculationDBConnectionString")
    {
    }

    public DbSet<Calculation> Calculations { get; set; }
}