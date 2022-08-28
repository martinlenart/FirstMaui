using System.Web;

namespace FirstMaui.Views
{
    public partial class ArticleView : ContentPage
    {

        public ArticleView()
        {
            InitializeComponent();
         }
        public ArticleView(string Url)
        {
            InitializeComponent();
            BindingContext = new UrlWebViewSource
            {
                Url = HttpUtility.UrlDecode(Url)
            };
        }
    }
}
