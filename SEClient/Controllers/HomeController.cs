using SEClient.Contract;
using SEClient.Models;
using SEClient.Profiles;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SEClient.Controllers
{
    public class HomeController : Controller
    {        
        private ObservableCollection<Category> _categorycombo = new ObservableCollection<Category>();
        private ObservableCollection<BusinessUnit> _BusinessUnitcombo = new ObservableCollection<BusinessUnit>();
        private ObservableCollection<Profile> _profilecombo = new ObservableCollection<Profile>();        

        public ActionResult GetCategory()
        {
            ProfileDataSource _objDataSource = new ProfileDataSource();
            _categorycombo = new ObservableCollection<Category>(_objDataSource.CategoryList());
            
            return Json(_categorycombo.Select(x => new
            {
                CategoryCode = x.CategoryCode,
                CategoryName = x.CategoryName
            }).ToList(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBusinessUnits()
        {
            ProfileDataSource _objDataSource = new ProfileDataSource();
            _BusinessUnitcombo = new ObservableCollection<BusinessUnit>(_objDataSource.BusinessUnitList());

            return Json(_BusinessUnitcombo.Select(x => new
            {
                BusinessUnitId = x.BusinessUnitId,
                BusinessUnitName = x.BusinessUnitName
            }).ToList(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetProfiles(string value)
        {
            ProfileDataSource _objDataSource = new ProfileDataSource();
            _profilecombo = new ObservableCollection<Profile>
                    (from profile in _objDataSource.ProfileList()
                     where profile.CategoryCode.Equals(value)
                     select profile);

            return Json(_profilecombo.Select(x => new
            {
                TagIndex = x.TagIndex,
                ProfileName = x.ProfileName
            }).ToList(), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Index()
        {            
            CreateIndexPath();
            return View();
        }


        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Index(FormCollection formCollection, IEnumerable<HttpPostedFileBase> file)
        {
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult IndexByDB(FormCollection formCollection)
        {
            StartIndexing();
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult IndexOnFly(FormCollection formCollection)
        {
            StartOnTheFly();
            return View();
        }

        public ActionResult Home()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Search()
        {
            CreateIndexPath();
            return View();
        }
        [HttpPost]
        public ActionResult LoadData(FormCollection collection)
        {
            var Category = collection["CategoryList"];
            var Profile = collection["ProfileList"];
            var BU = collection["BUList"];
            var Barcode = collection["barcode"];
            var Startdate = collection["startdate"];
            var Enddate = collection["enddate"];
            var result = StartSearching(Category, Profile, BU, Startdate, Enddate);
            return Json(result, JsonRequestBehavior.AllowGet);            
        }
        
        private void CreateIndexPath()
        {
            // Construct a machine-independent path for the index
            var basePath = Environment.GetFolderPath(
                Environment.SpecialFolder.CommonApplicationData);
            var indexPath = System.IO.Path.Combine(basePath, "index");
            //IndexPathtextBox.Text = indexPath;
            SerachEngineControler.indexPath = indexPath;
        }
        private void StartIndexing()
        {
            //MainDockPanel.IsEnabled = false;
            //progressBar.Visibility = Visibility.Visible;
            //progressBar.Value = 0;            
            SerachEngineControler.StartIndexingFromDB();
            //MainDockPanel.IsEnabled = true;
            //progressBar.Visibility = Visibility.Hidden;
            //ProgressBarlabel.Content = "";
            //IndexingFromDb.Background = Brushes.Green;
        }

        private List<ListViewRow> StartSearching(string Category,string TagIndex, string _businessUnitId, string FromDate, string EndDate)
        {
            var _IndexPath = SerachEngineControler.indexPath;
            CultureInfo culture = new CultureInfo("en-US");

            string _FromDate = Convert.ToDateTime(FromDate, culture).ToString("yyyyMMdd");
            string _EndDate = Convert.ToDateTime(EndDate, culture).ToString("yyyyMMdd");
            
            ProfileData.SetProfileData(TagIndex, _businessUnitId, _FromDate, _EndDate, "");
            List<ListViewRow> ListViewRowResults = SerachEngineControler.GetDataToSerachEngine(_IndexPath);

            return ListViewRowResults;

        }
        private void StartOnTheFly()
        {
            Task.Factory.StartNew(() =>
            SerachEngineControler.GetDataFromRabbitToSerachEngine());
        }
    }
}