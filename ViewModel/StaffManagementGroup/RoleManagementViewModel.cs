﻿using Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel
{
    public class RoleManagementViewModel : Navigation.CRUDViewModelBase
    {
        private static RoleManagementViewModel _instance = null;
        public static RoleManagementViewModel Instance { get => _instance; }

        public ObservableCollection<RoleModel> Roles { get; set; }

        public RoleManagementViewModel()
        {
            _instance = this;
            Roles = RoleModel.LoadAll();
        }
    }
}