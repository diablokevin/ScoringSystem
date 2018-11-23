namespace ScoringSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddChipsInEvent : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Chips",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Serial = c.Int(nullable: false),
                        Name = c.String(),
                        EventId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Events", t => t.EventId, cascadeDelete: true)
                .Index(t => t.EventId);
            
            DropColumn("dbo.Events", "ChipId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Events", "ChipId", c => c.Int());
            DropForeignKey("dbo.Chips", "EventId", "dbo.Events");
            DropIndex("dbo.Chips", new[] { "EventId" });
            DropTable("dbo.Chips");
        }
    }
}
