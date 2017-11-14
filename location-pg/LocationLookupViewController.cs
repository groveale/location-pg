using Foundation;
using System;
using UIKit;
using MapKit;
using System.Diagnostics;

namespace locationpg
{
    public partial class LocationLookupViewController : UIViewController
    {

        UISearchBar searchBar;
        UITableView searchResultsTableView;

        MKLocalSearchCompleter searchCompleter;
        public MKLocalSearchCompletion[] searchResults { get; set; } = {};

        public LocationLookupViewController (IntPtr handle) : base (handle)
        {
        }

      

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.
            //var searchController = new UISearchController(this);
            //searchController.SearchBar.SizeToFit();
            //searchBar = searchController.SearchBar;

            //NavigationItem.TitleView = searchBar;

            //searchResultsTableView = SearchResultsTable;
            //searchResultsTableView.TableHeaderView = searchBar;
            //searchResultsTableView.ScrollEnabled = false;

            ConfigureView();

            searchCompleter = new MKLocalSearchCompleter();
            searchCompleter.Delegate = new SearCompleterDelegate(SearchCompleterDelegate_DidUpdateResults);

            searchResultsTableView.Source = new SearchResultsSource(this);
            searchResultsTableView.Delegate = new SearchResultsDelegate(this);

            searchBar.TextChanged += SearchBar_TextChanged;
        }

        private void ConfigureView()
        {
            searchBar = SearchBar;
            searchResultsTableView = SearchResults;

            searchBar.Frame = new CoreGraphics.CGRect(0, NavigationController.NavigationBar.Frame.Bottom, View.Frame.Width, searchBar.Frame.Height);

            var resultsViewHeight = View.Frame.Height - searchBar.Frame.Bottom - TabBarController.TabBar.Frame.Height;

            searchResultsTableView.Frame = new CoreGraphics.CGRect(0, searchBar.Frame.Bottom, View.Frame.Width, resultsViewHeight);
        }

        private void SearchBar_TextChanged(object sender, UISearchBarTextChangedEventArgs e)
        {
            searchCompleter.QueryFragment = e.SearchText;
        }

        private void SearchCompleterDelegate_DidUpdateResults(object newResults, EventArgs e)
        {
            searchResults = (MKLocalSearchCompletion[])newResults;
            searchResultsTableView.ReloadData();
        }
    }

    internal class SearchResultsDelegate : UITableViewDelegate
    {
        //private MKLocalSearchCompletion[] searchResults;
        LocationLookupViewController owner;

        public SearchResultsDelegate(LocationLookupViewController owner)
        {
            this.owner = owner;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            tableView.DeselectRow(indexPath, true);

            var completion = owner.searchResults[indexPath.Row];

            // should work on new verison of xam
            var searchRequest = new MKLocalSearchRequest(completion);

            var search = new MKLocalSearch(request: searchRequest);

            search.Start((MKLocalSearchResponse response, NSError error) =>
            {
                var coordinate = response?.MapItems[0].Placemark.Coordinate;
                Debug.WriteLine("selected: {0}", coordinate);
            });

        }
    }

    internal class SearchResultsSource : UITableViewSource
    {

        //MKLocalSearchCompletion[] searchResults;
        LocationLookupViewController owner;

        public SearchResultsSource(LocationLookupViewController owner)
        {
            this.owner = owner;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var searchResult = owner.searchResults[indexPath.Row];
            var cell = tableView.DequeueReusableCell("sub") as UITableViewCell;

            if (cell == null)
            {
                cell = new UITableViewCell(UITableViewCellStyle.Subtitle, "sub");
            }
            //cell.TextLabel.Text = searchResult.Title;
            //cell.DetailTextLabel.Text = searchResult.Subtitle;

            cell.TextLabel.AttributedText = HighlightedText(searchResult.Title, searchResult.TitleHighlightRanges, 17.0f);
            cell.DetailTextLabel.AttributedText = HighlightedText(searchResult.Title, searchResult.TitleHighlightRanges, 12.0f);


            return cell;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return owner.searchResults.Length;
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return 1;
        }

        /**
          Highlights the matching search strings with the results
          - parameter text: The text to highlight
          - parameter ranges: The ranges where the text should be highlighted
          - parameter size: The size the text should be set at 
          - returns: A highlighted attributed string with the ranges highlighted
         */
        public NSAttributedString HighlightedText(string text, NSValue[] ranges, float size)
        {
            var attributedText = new NSMutableAttributedString(text);
            var regular = UIFont.SystemFontOfSize(size);

            UIStringAttributes uiString = new UIStringAttributes();
            uiString.Font = regular;

            attributedText.SetAttributes(uiString, new NSRange(0, text.Length));

            var bold = UIFont.BoldSystemFontOfSize(size);
            uiString.Font = bold;

            foreach (var value in ranges)
            {
                attributedText.SetAttributes(uiString, value.RangeValue);
            }


            return attributedText;
        }
    }

    public class SearCompleterDelegate : MKLocalSearchCompleterDelegate
    {
        private event EventHandler OnUpdated;

        public SearCompleterDelegate(EventHandler OnUpdated)
        {
            this.OnUpdated = OnUpdated;
        }

        public override void DidUpdateResults(MKLocalSearchCompleter completer)
        {
            var searchResults = completer.Results;
            OnUpdated(searchResults, new EventArgs());
        }

        public override void DidFail(MKLocalSearchCompleter completer, NSError error)
        {
            // handel fail
        }

    }
}