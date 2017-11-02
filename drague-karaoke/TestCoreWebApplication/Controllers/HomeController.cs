using Lucene.Net.Analysis;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
            var hits_limit = 1000;

            BooleanQuery termQuery = parseQuery(term, analyzer);

            searcher.SetDefaultFieldSortScoring(true, true);
            ScoreDoc[] hits = searcher.Search(termQuery, null, hits_limit, Sort.RELEVANCE).ScoreDocs;

            List<String> searchResults = new List<String>();
            foreach (ScoreDoc hit in hits)
            {
                var document = searcher.IndexReader.Document(hit.Doc);
                searchResults.Add(document.Get("Title") + " par " + document.Get("Artist"));
            }
            return Json(searchResults.ToArray());
        }

        [HttpPost]
        public ActionResult SubmitBtn(SongRequest songRequest)
      {
            songRequest.songId = 100;
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

        private BooleanQuery parseQuery(string searchQuery, Analyzer analyzer)
        {
            var escapedTerm = QueryParser.Escape(searchQuery);
            var prefixedTerm = String.Concat("\"", escapedTerm, "\"");
            var mtQueryParser = new MultiFieldQueryParser(Lucene.Net.Util.Version.LUCENE_30, new[] { "Title", "Artist", "Category" }, analyzer);

            var qpName = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, "Title", analyzer);
            var qpArtist = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, "Artist", analyzer);
            var qpCategory = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, "Category", analyzer);

            /*
            qpName.AllowLeadingWildcard = true;
            qpArtist.AllowLeadingWildcard = true;
            qpCategory.AllowLeadingWildcard = true;
            */

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

        public BooleanQuery getQueryFromTerms(ISet<Term> pTerms)
        {

            BooleanQuery query = new BooleanQuery();
            foreach (Term term in pTerms)
            {
                query.Add(new PrefixQuery(term), Occur.MUST);
            }

            return query;
        }

    }
}
