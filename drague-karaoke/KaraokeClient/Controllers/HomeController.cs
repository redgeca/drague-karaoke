using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using KaraokeClient.Models;
using Lucene.Net.Analysis;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using KaraokeClient.lucene.analyzers;
using Lucene.Net.QueryParsers;
using Lucene.Net.Index;

namespace KaraokeClient.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            String singerNameCookie = Request.Cookies["SingerName"];

            SongRequest songRequest = new SongRequest();
            songRequest.singerName = singerNameCookie;
            songRequest.songId = 1;

            return View("Index", songRequest);
        }

        [HttpGet]
        public ActionResult KeywordSearch(string term)
        {
            Analyzer analyzer = new ASCIIFoldingAnalyzer(Lucene.Net.Util.Version.LUCENE_30);

            // Perform a search
            var searcher = getSearcher();
            var hits_limit = 10;

            BooleanQuery finalQuery = getPrefixQuery(term, analyzer);

            searcher.SetDefaultFieldSortScoring(true, true);
            ScoreDoc[] hits = searcher.Search(finalQuery, null, hits_limit, Sort.RELEVANCE).ScoreDocs;
            ScoreDoc[] fuzzyHits = searcher.Search(getFuzzyQuery(term, analyzer), null, hits_limit, Sort.RELEVANCE).ScoreDocs;

            List<ScoreDoc> scoreDocs = new List<ScoreDoc>();
            scoreDocs.AddRange(hits);
            if (hits.Length < 10)
            {
                scoreDocs.AddRange(fuzzyHits);
            }
            //            var sortedList = scoreDocs.OrderBy(d => d.Score);

            List<String> searchResults = new List<String>();
            foreach (ScoreDoc hit in scoreDocs)
            {
                var document = searcher.IndexReader.Document(hit.Doc);
                searchResults.Add(document.Get("Title") + " par " + document.Get("Artist"));
            }
            return Json(searchResults.ToArray().Take(10));
        }

        private IndexSearcher getSearcher()
        {
            String indexLocation = @"c:\karaoke\index";
            Directory indexDirectory = FSDirectory.Open(indexLocation);

            // Perform a search
            return new IndexSearcher(indexDirectory, false);
        }

        [HttpGet]
        public ActionResult ArtistSearch(string term)
        {
            Analyzer analyzer = new ASCIIFoldingAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
            var hits_limit = 5000;

            BooleanQuery finalQuery = getPrefixQuery("Artist", term, analyzer);
            IndexSearcher searcher = getSearcher();

            searcher.SetDefaultFieldSortScoring(true, true);
            ScoreDoc[] hits = searcher.Search(finalQuery, null, hits_limit, Sort.RELEVANCE).ScoreDocs;
            ScoreDoc[] fuzzyHits = searcher.Search(getFuzzyQuery("Artist", term, analyzer), null, hits_limit, Sort.RELEVANCE).ScoreDocs;

            List<String> searchResults = new List<String>();
            foreach (ScoreDoc hit in hits)
            {
                var document = searcher.IndexReader.Document(hit.Doc);
                string artist = document.Get("Artist");
                if (!searchResults.Contains(artist))
                {
                    searchResults.Add(document.Get("Artist"));
                }
            }

            foreach (ScoreDoc hit in fuzzyHits)
            {
                var document = searcher.IndexReader.Document(hit.Doc);
                string artist = document.Get("Artist");
                if (!searchResults.Contains(artist))
                {
                    searchResults.Add(document.Get("Artist"));
                }
            }

            searcher.Dispose();
            return Json(searchResults.ToArray().Take(10));
        }


        [HttpGet]
        public ActionResult CompleteSelect()
        {
            return View();
        }

        // Define the list which you have to show in Drop down List
        public List<SelectListItem> getCategories()
        {
            List<SelectListItem> myList = new List<SelectListItem>();
            var data = new[]{
                 new SelectListItem{ Value="1",Text="Monday"},
                 new SelectListItem{ Value="2",Text="Tuesday"},
                 new SelectListItem{ Value="3",Text="Wednesday"},
                 new SelectListItem{ Value="4",Text="Thrusday"},
                 new SelectListItem{ Value="5",Text="Friday"},
                 new SelectListItem{ Value="6",Text="Saturday"},
                 new SelectListItem{ Value="7",Text="Sunday"},
             };
            myList = data.ToList();
            return myList;
        }

        [HttpPost]
        public ActionResult SubmitBtn(SongRequest songRequest)
        {
            if (ModelState.IsValid)
            {
                string singerName = songRequest.singerName;

                ModelState.Clear();

                ViewData["SubmitSong"] = "Demande effectuée avec succès à " + String.Format("{0:HH:mm:ss}", DateTime.Now);
                CookieOptions cookieOption = new CookieOptions();

                cookieOption.Expires = DateTime.Now.AddDays(30);
                Response.Cookies.Append("SingerName", singerName, cookieOption);

                SongRequest newRequest = new SongRequest();
                newRequest.singerName = singerName;

                return View("Index", newRequest);
            }

            return View("Index");
        }

        private BooleanQuery getPrefixQuery(string pField, string pSearchQuery, Analyzer pAnalyzer)
        {
            var escapedTerm = QueryParser.Escape(pSearchQuery);
            var prefixedTerm = String.Concat("\"", escapedTerm, "\"");

            var queryParser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, pField, pAnalyzer);
            Query query = queryParser.Parse(prefixedTerm);

            ISet<Term> terms = new HashSet<Term>();
            query.ExtractTerms(terms);

            BooleanQuery termQuery = new BooleanQuery();
            termQuery.Add(getQueryFromTerms(terms), Occur.SHOULD);
            return termQuery;
        }

        private BooleanQuery getFuzzyQuery(string pField, string searchTerms, Analyzer pAnalyzer)
        {
            BooleanQuery resultQuery = new BooleanQuery();

            String[] terms = searchTerms.Split(" ");
            foreach (string term in terms)
            {
                resultQuery.Add(new FuzzyQuery(new Term(pField, term.ToLower())), Occur.SHOULD);
            }
            return resultQuery;
        }

        private BooleanQuery getPrefixQuery(string searchQuery, Analyzer analyzer)
        {
            BooleanQuery termQuery = new BooleanQuery();
            termQuery.Add(getPrefixQuery("Title", searchQuery, analyzer), Occur.SHOULD);
            termQuery.Add(getPrefixQuery("Artist", searchQuery, analyzer), Occur.SHOULD);
            termQuery.Add(getPrefixQuery("Category", searchQuery, analyzer), Occur.SHOULD);

            return termQuery;
        }

        private BooleanQuery getFuzzyQuery(string searchTerms, Analyzer pAnalyzer)
        {
            BooleanQuery resultQuery = new BooleanQuery();

            resultQuery.Add(getFuzzyQuery("Artist", searchTerms, pAnalyzer), Occur.SHOULD);
            resultQuery.Add(getFuzzyQuery("Title", searchTerms, pAnalyzer), Occur.SHOULD);
            resultQuery.Add(getFuzzyQuery("Category", searchTerms, pAnalyzer), Occur.SHOULD);

            return resultQuery;
        }


        public BooleanQuery getQueryFromTerms(ISet<Term> pTerms)
        {

            BooleanQuery query = new BooleanQuery();
            foreach (Term term in pTerms)
            {
                query.Add(new PrefixQuery(term), Occur.SHOULD);
            }

            return query;
        }

    }
}
