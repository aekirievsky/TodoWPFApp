using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TodoWPFApp.Models;
using TodoWPFApp.UserControls;

namespace TodoWPFApp.Data.DataBase
{
    public class AppDbContext : DbContext
    {
        public DbSet<TodoModel> Notes { get; set; }


        public AppDbContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string dataBasePath = $"{desktopPath}/NotesDB.db";
            optionsBuilder.UseSqlite($"Filename = {dataBasePath}");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TodoModel>().Property(t => t.NoteId).ValueGeneratedOnAdd();
            modelBuilder.Entity<TodoModel>().HasKey(t => t.NoteId);
        }

        public void AddToDo(TodoModel note)
        {
            Notes.Add(note);
            SaveChanges();
        }

        public List<TodoModel> GetNotesByDate(DateTime date)
        {
            return Notes.Where(n => n.Time.Date == date.Date).ToList();
        }


    }

}
