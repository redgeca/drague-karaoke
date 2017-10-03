
using KaraokeCoreObjects;
using System;

namespace DBSetupAndDataSeed
{
    class Program
    {
        static void Main(string[] args)
        {
            using(var db = new SongDBContext())
            {
                db.Categories.Add(new Category());
                db.SaveChanges();
            }
        }
    }
}
