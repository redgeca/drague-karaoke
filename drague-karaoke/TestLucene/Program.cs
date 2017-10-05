using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using System;

namespace TestLucene
{
    class Program
    {
        static void Main(string[] args)
        {

            String indexLocation = @"D:\rejean";
            Directory indexDirectory = FSDirectory.Open(indexLocation);

            Analyzer analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);

            var writer = new IndexWriter(indexDirectory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED);

            // First, we clear index so we won't get duplicates...
            writer.DeleteAll();
 
            writer.AddDocument(getSongDocument("1", "Your Song", "Elton John", "Y"));
            writer.AddDocument(getSongDocument("2", "Candle in the wind", "Elton John", "C"));
            writer.AddDocument(getSongDocument("3", "Inaudible Melodies", "Jack Johnson", "I"));
            writer.AddDocument(getSongDocument("4", "Middle Man", "Jack Johnson", "Y"));

            writer.Dispose();


            // Perform a search
            var searcher = new IndexSearcher(indexDirectory, false);
            var hits_limit = 1000;
            var parser = new MultiFieldQueryParser(Lucene.Net.Util.Version.LUCENE_30, new[] { "Name", "Artist" }, analyzer);
            var query = parseQuery("Elton* john*", parser);
            searcher.SetDefaultFieldSortScoring(true, true);
            var hits = searcher.Search(query, null, hits_limit, Sort.RELEVANCE).ScoreDocs;

            System.Console.WriteLine("TOTO");
        }

        private static Document getSongDocument(String pId, String pName, String pArtist, String pCategory)
        {
            Document luceneDocument = new Document();
            luceneDocument.Add(new Field("Id", pId, Field.Store.NO, Field.Index.ANALYZED));
            luceneDocument.Add(new Field("Name", pName, Field.Store.NO, Field.Index.ANALYZED));
            luceneDocument.Add(new Field("Category", pCategory, Field.Store.NO, Field.Index.ANALYZED));
            luceneDocument.Add(new Field("Artist", pArtist, Field.Store.NO, Field.Index.ANALYZED));

            return luceneDocument;
        }

        private static Query parseQuery(string searchQuery, QueryParser parser)
        {
            Query query;
            try
            {
                query = parser.Parse(searchQuery.Trim());
            }
            catch (ParseException)
            {
                query = parser.Parse(QueryParser.Escape(searchQuery.Trim()));
            }
            return query;
        }
    }
}
