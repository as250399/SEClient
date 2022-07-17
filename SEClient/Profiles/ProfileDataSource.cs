using SEClient.Contract;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;

namespace SEClient.Profiles
{
    public class ProfileDataSource
    {
        ObservableCollection<Category> _category = new ObservableCollection<Category>();
        ObservableCollection<Profile> _profile = new ObservableCollection<Profile>();
        ObservableCollection<BusinessUnit> _BusinessUnit = new ObservableCollection<BusinessUnit>();
        public ObservableCollection<Category> CategoryList()
        {
            _category.Clear();
            CategoryData();
            return _category;
        }

        public ObservableCollection<BusinessUnit> BusinessUnitList()
        {
            _BusinessUnit.Clear();
            BusinessUnitData();

            return _BusinessUnit;
        }
        public ObservableCollection<Profile> ProfileList()
        {
            _profile.Clear();
            ProfileData();
            return _profile;
        }

        private void CategoryData()
        {
            _category.Add(new Category { CategoryName = "Froud", CategoryCode = "FR" });
            _category.Add(new Category { CategoryName = "Sales report", CategoryCode = "SB" });
            _category.Add(new Category { CategoryName = "Max performance", CategoryCode = "MA" });
            _category.Add(new Category { CategoryName = "Z report", CategoryCode = "ZR" });
        }

        private void BusinessUnitData()
        {
            _BusinessUnit.Add(new BusinessUnit { BusinessUnitId = "0", BusinessUnitName = "Global" });
            _BusinessUnit.Add(new BusinessUnit { BusinessUnitId = "10072", BusinessUnitName = "Hillc" });
            _BusinessUnit.Add(new BusinessUnit { BusinessUnitId = "10438", BusinessUnitName = "Jmp" });
        }

        private void ProfileData()
        {
            _profile.Add(new Profile { ProfileName = "Qty of sell transactions with high tare weight, group by cashier", CategoryCode = "FR", TagIndex = "0" });
            _profile.Add(new Profile { ProfileName = "Qty of return transactions, group by cashier", CategoryCode = "FR", TagIndex = "1" });
            _profile.Add(new Profile { ProfileName = "Qty of sell item", CategoryCode = "SB", TagIndex = "2" });
            _profile.Add(new Profile { ProfileName = "Get all fund transfer transaction", CategoryCode = "SB", TagIndex = "3" });
            _profile.Add(new Profile { ProfileName = "Qty of sell transactions group by store", CategoryCode = "SB", TagIndex = "4" });
            _profile.Add(new Profile { ProfileName = "Qty of sell transactions group by cashier", CategoryCode = "SB", TagIndex = "5" });
            _profile.Add(new Profile { ProfileName = "Qty of lock pos action group by cashier", CategoryCode = "MA", TagIndex = "6" });
            _profile.Add(new Profile { ProfileName = "Get Z Report", CategoryCode = "ZR", TagIndex = "7" });
            _profile.Add(new Profile { ProfileName = "Get the most sell products", CategoryCode = "MA", TagIndex = "8" });
            _profile.Add(new Profile { ProfileName = "Get the most sell promotions", CategoryCode = "MA", TagIndex = "9" });
            _profile.Add(new Profile { ProfileName = "Get the transaction with the highest amount", CategoryCode = "MA", TagIndex = "10" });
            _profile.Add(new Profile { ProfileName = "Get the transaction with the lowest amount", CategoryCode = "MA", TagIndex = "11" });
        }
    }
}