using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

/// <summary>
/// EF Core DbContext. Verwaltet die Verbindung zur Datenbank und das Mapping der Entit�ten.
/// </summary>
public class AppDbContext( DbContextOptions<AppDbContext> options ) : DbContext( options )
{
      /// <summary>
      /// Tabelle/DbSet f�r Sensoren.
      /// </summary>
      public DbSet<Sensor> Sensors => Set<Sensor>( );

      /// <summary>
      /// Tabelle/DbSet f�r Messungen.
      /// </summary>
      public DbSet<Measurement> Measurements => Set<Measurement>( );

      /// <summary>
      /// Fluent-API Konfigurationen f�r EF Core.
      /// </summary>
      protected override void OnModelCreating( ModelBuilder modelBuilder )
      {
            base.OnModelCreating( modelBuilder );

            modelBuilder.Entity<Sensor>( sensor =>
            {
                  sensor.Property( s => s.Location ).HasMaxLength( 100 ).IsRequired( );
                  sensor.Property( s => s.Name ).HasMaxLength( 100 ).IsRequired( );

                  // RowVersion f�r Optimistic Concurrency
                  sensor.Property( s => s.RowVersion ).IsRowVersion( );
                  // Uniqueness constraint (Location + Name)
                  sensor.HasIndex( s => new { s.Location , s.Name } )
                    .IsUnique( )
                    .HasDatabaseName( "UX_Sensors_Location_Name" );
            } );

            modelBuilder.Entity<Measurement>( measurement =>
            {
                  // Index f�r h�ufige Abfragen nach Sensor und Zeitpunkt
                  measurement.HasIndex( m => new { m.SensorId , m.Timestamp } );
                  // Beziehung: Jede Messung geh�rt zu genau einem Sensor (Cascade Delete)
                  measurement.HasOne( m => m.Sensor )
                  .WithMany( s => s.Measurements )
                  .HasForeignKey( m => m.SensorId )
                  .OnDelete( DeleteBehavior.Cascade );
                  // RowVersion f�r Optimistic Concurrency
                  measurement.Property( m => m.RowVersion ).IsRowVersion( );
            } );
      }
}
