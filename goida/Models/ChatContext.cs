using Microsoft.EntityFrameworkCore;
using goida.Models;
using Microsoft.EntityFrameworkCore.Storage;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace goida.Models
{
	public class ChatContext : DbContext
	{
		public ChatContext(DbContextOptions<ChatContext> options) : base(options) { }

		public DbSet<Message> Messages { get; set; }
		
        public DbSet<User> Users { get; set; }
        
        public DbSet<PrivateChat> PrivateChats { get; set; }
        public DbSet<PrivateMessage> PrivateMessages { get; set; }



    }
}
