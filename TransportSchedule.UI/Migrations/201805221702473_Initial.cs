namespace TransportScheduleClasses.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Favourites",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                        Station_Id = c.Int(),
                        User_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Stations", t => t.Station_Id)
                .ForeignKey("dbo.Users", t => t.User_Id)
                .Index(t => t.Station_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.Stations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Email = c.String(),
                        PasswordHash = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Lines",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Number = c.Int(nullable: false),
                        Interval = c.Int(nullable: false),
                        FirstDeparture = c.Int(nullable: false),
                        LastDeparture = c.Int(nullable: false),
                        Chosen = c.Boolean(nullable: false),
                        Destination_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Stations", t => t.Destination_Id)
                .Index(t => t.Destination_Id);
            
            CreateTable(
                "dbo.LineStations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TimeFromOrigin = c.Int(nullable: false),
                        Line_Id = c.Int(),
                        Station_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Lines", t => t.Line_Id)
                .ForeignKey("dbo.Stations", t => t.Station_Id)
                .Index(t => t.Line_Id)
                .Index(t => t.Station_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LineStations", "Station_Id", "dbo.Stations");
            DropForeignKey("dbo.LineStations", "Line_Id", "dbo.Lines");
            DropForeignKey("dbo.Lines", "Destination_Id", "dbo.Stations");
            DropForeignKey("dbo.Favourites", "User_Id", "dbo.Users");
            DropForeignKey("dbo.Favourites", "Station_Id", "dbo.Stations");
            DropIndex("dbo.LineStations", new[] { "Station_Id" });
            DropIndex("dbo.LineStations", new[] { "Line_Id" });
            DropIndex("dbo.Lines", new[] { "Destination_Id" });
            DropIndex("dbo.Favourites", new[] { "User_Id" });
            DropIndex("dbo.Favourites", new[] { "Station_Id" });
            DropTable("dbo.LineStations");
            DropTable("dbo.Lines");
            DropTable("dbo.Users");
            DropTable("dbo.Stations");
            DropTable("dbo.Favourites");
        }
    }
}
