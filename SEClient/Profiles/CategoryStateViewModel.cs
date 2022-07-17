using SEClient.Contract;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace SEClient.Profiles
{
    public class CategoryStateViewModel
    {
        public event PropertyChangedEventHandler PropertyChanged;
        ProfileDataSource _objDataSource = new ProfileDataSource();
        private ObservableCollection<Category> _categorycombo = new ObservableCollection<Category>();
        private ObservableCollection<Profile> _profilecombo = new ObservableCollection<Profile>();
        private ObservableCollection<BusinessUnit> _BusinessUnitcombo = new ObservableCollection<BusinessUnit>();
        public void LoadCategory()
        {
            _categorycombo = new ObservableCollection<Category>(_objDataSource.CategoryList());

        }

        public void LoadBusinessUnit()
        {
            _BusinessUnitcombo = new ObservableCollection<BusinessUnit>(_objDataSource.BusinessUnitList());

        }
        public ObservableCollection<Category> SingleCategoryList
        {
            get
            {
                return _categorycombo;
            }
        }

        public ObservableCollection<BusinessUnit> SingleBusinessUnitList
        {
            get
            {
                return _BusinessUnitcombo;
            }
        }

        public ObservableCollection<Profile> SingleProfileList
        {
            get
            {
                return _profilecombo;
            }
        }
        public void LoadProfile(string categorycode)
        {
            _profilecombo = new ObservableCollection<Profile>
                    (from profile in _objDataSource.ProfileList()
                     where profile.CategoryCode.Equals(categorycode)
                     select profile);

            OnPropertyChanged("SingleProfileList");
        }

        protected void OnPropertyChanged(string PropertyName)
        {
            VerifyPropertyName(PropertyName);

            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
            }
        }

        private void VerifyPropertyName(string propertyName)
        {
            // Verify that the property name matches a real,  
            // public, instance property on this object.
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
            {
                string msg = "Invalid property name: " + propertyName;
                throw new Exception(msg);
            }
        }
    }
}