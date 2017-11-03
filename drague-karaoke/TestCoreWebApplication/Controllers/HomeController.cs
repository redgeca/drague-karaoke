using Lucene.Net.Analysis;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using TestCoreWebApplication.Models;

namespace TestCoreWebApplication.Controllers
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
        public ActionResult Autocomplete(string term)
        {
            Analyzer analyzer = new ASCIIFoldingAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
            String indexLocation = @"c:\karaoke\index";
            Directory indexDirectory = FSDirectory.Open(indexLocation);

            // Perform a search
            var searcher = new IndexSearcher(indexDirectory, false);
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

        private BooleanQuery getPrefixQuery(string searchQuery, Analyzer analyzer)
        {
            var escapedTerm = QueryParser.Escape(searchQuery);
            var prefixedTerm = String.Concat("\"", escapedTerm, "\"");

            var qpName = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, "Title", analyzer);
            var qpArtist = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, "Artist", analyzer);
            var qpCategory = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, "Category", analyzer);

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

        private BooleanQuery getFuzzyQuery(string searchTerms, Analyzer pAnalyzer)
        {
            BooleanQuery resultQuery = new BooleanQuery();

            String[] terms = searchTerms.Split(" ");
            foreach(string term in terms)
            {
                resultQuery.Add(new FuzzyQuery(new Term("Artist", term.ToLower())), Occur.SHOULD);
                resultQuery.Add(new FuzzyQuery(new Term("Title", term.ToLower())), Occur.SHOULD);
                resultQuery.Add(new FuzzyQuery(new Term("Category", term.ToLower())), Occur.SHOULD);
            }
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
