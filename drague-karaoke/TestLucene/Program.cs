using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using System;
using System.Collections.Generic;

namespace TestLucene
{
    class Program
    {
        static void Main(string[] args)
        {

            String indexLocation = @"c:\karaoke\index";
            Directory indexDirectory = FSDirectory.Open(indexLocation);

            Analyzer analyzer = new ASCIIFoldingAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
            /*
            var writer = new IndexWriter(indexDirectory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED);

            // First, we clear index so we won't get duplicates...
            writer.DeleteAll();
 
            writer.AddDocument(getSongDocument("1", "Your Song", "Elton John", "Y"));
            writer.AddDocument(getSongDocument("2", "Candle in the wind", "Elton John", "C"));
            writer.AddDocument(getSongDocument("3", "Inaudible Melodies", "Jack Johnson", "I"));
            writer.AddDocument(getSongDocument("4", "Middle Man", "Jack Johnson", "M"));
            writer.AddDocument(getSongDocument("5", "Rêver mieux", "Daniel Bélanger", "R"));
            writer.AddDocument(getSongDocument("6", "Sèche tes pleurs", "Daniel Bélanger", "S"));

            writer.Dispose();
            */
            // Perform a search
            var searcher = new IndexSearcher(indexDirectory, false);
            var hits_limit = 1000;

            BooleanQuery termQuery = parseQuery("i", analyzer);
            BooleanQuery fQuery = fuzzyQuery("hania wain", analyzer);

            searcher.SetDefaultFieldSortScoring(true, true);
            ScoreDoc[] hits = searcher.Search(termQuery, null, hits_limit, Sort.RELEVANCE).ScoreDocs;
            ScoreDoc[] hits2 = searcher.Search(fQuery, null, hits_limit, Sort.RELEVANCE).ScoreDocs;

            foreach (ScoreDoc hit in hits2)
            {
                var document = searcher.IndexReader.Document(hit.Doc);
                System.Console.WriteLine(document.Get("Title") + " (" + document.Get("Artist") + ") " + hit.Score);
            }
            System.Console.WriteLine(hits.ToString());
        }

        private static Document getSongDocument(String pId, String pName, String pArtist, String pCategory)
        {
            Document luceneDocument = new Document();
            
            luceneDocument.Add(new Field("Id", pId, Field.Store.YES, Field.Index.ANALYZED));
            luceneDocument.Add(new Field("Title", pName, Field.Store.NO, Field.Index.ANALYZED));
            luceneDocument.Add(new Field("Category", pCategory, Field.Store.NO, Field.Index.ANALYZED));
            luceneDocument.Add(new Field("Artist", pArtist, Field.Store.NO, Field.Index.ANALYZED));

            return luceneDocument;
        }

        private static BooleanQuery parseQuery(string searchQuery, Analyzer analyzer)
        {
            var escapedTerm = QueryParser.Escape(searchQuery);
            var prefixedTerm = String.Concat("\"", escapedTerm, "\"");

            var qpName = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, "Title", analyzer);
            var qpArtist = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, "Artist", analyzer);
            var qpCategory = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, "Category", analyzer);

            qpName.AllowLeadingWildcard = true;
            qpArtist.AllowLeadingWildcard = true;
            qpCategory.AllowLeadingWildcard = true;
            Query queryName = qpName.Parse(prefixedTerm);
            Query queryArtist = qpArtist.Parse(prefixedTerm);
            Query queryCategory = qpCategory.Parse(prefixedTerm);

            ISet<Term> nameTerms = new HashSet<Term>();
            ISet<Term> artistTerms = new HashSet<Term>();
            ISet<Term> categoryTerms = new HashSet<Term>();

            queryName.ExtractTerms(nameTerms);
            queryArtist.ExtractTerms(artistTerms);
            queryCategory.ExtractTerms(categoryTerms);

            BooleanQuery termQuery = new BooleanQuery();
            termQuery.Add(getQueryFromTerms(nameTerms), Occur.SHOULD);
            termQuery.Add(getQueryFromTerms(artistTerms), Occur.SHOULD);
            termQuery.Add(getQueryFromTerms(categoryTerms), Occur.SHOULD);

            return termQuery;
        }

        public static BooleanQuery getQueryFromTerms(ISet<Term> pTerms)
        {

            BooleanQuery query = new BooleanQuery();
            foreach (Term term in pTerms)
            {
                query.Add(new PrefixQuery(term), Occur.MUST);
            }

            return query;
        }
        public static BooleanQuery fuzzyQuery(string searchTerms, Analyzer pAnalyzer)
        {
            var mtQueryParser = new MultiFieldQueryParser(Lucene.Net.Util.Version.LUCENE_30, new[] { "Title", "Artist", "Category" }, pAnalyzer);
 
            BooleanQuery resultQuery = new BooleanQuery();
            resultQuery.Add(new FuzzyQuery(new Term("Artist", "aneil")), Occur.SHOULD);
            resultQuery.Add(new FuzzyQuery(new Term("Title", "aneel")), Occur.SHOULD);
            resultQuery.Add(new FuzzyQuery(new Term("Category", "aneel")), Occur.SHOULD);
            return resultQuery;
        }
    }
}
