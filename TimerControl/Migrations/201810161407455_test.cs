namespace TimerControl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class test : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Events", "TimeLimit", c => c.Time(precision: 7));
            AlterColumn("dbo.Events", "ChipId", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Events", "ChipId", c => c.Int(nullable: false));
            AlterColumn("dbo.Events", "TimeLimit", c => c.Time(nullable: false, precision: 7));
        }
    }
}
