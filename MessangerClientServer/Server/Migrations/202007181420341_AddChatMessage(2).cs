namespace Server.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddChatMessage2 : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.ChatMessages", "UserId");
            AddForeignKey("dbo.ChatMessages", "UserId", "dbo.Users", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ChatMessages", "UserId", "dbo.Users");
            DropIndex("dbo.ChatMessages", new[] { "UserId" });
        }
    }
}
