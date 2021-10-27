﻿using DAL;
using Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ViewModel.Helper;
using DAL.Rows;

namespace ViewModel
{
    public class StaffsViewModel : Navigation.ViewModelBase
    {
        public enum Mode
        {
            Insert,
            Edit
        }

        public override string ViewName => "Staffs";

        private bool inCommand = false;

        public Mode CurrentMode { get; set; }

        [PropertyChanged.OnChangedMethod(nameof(OnSelectedChange))]
        public Employee SelectedEmployee { get; set; }

        public Employee CurrentEmployee { get; set; }
        public ObservableCollection<Employee> EmployeeList { get; set; }

        protected RelayCommand<object> _addStaffRelayCommand;
        public ICommand AddStaffCommand => _addStaffRelayCommand ?? (_addStaffRelayCommand = new RelayCommand<object>(ExecuteAddStaff, CanExecuteAddStaff));

        protected RelayCommand<object> _editStaffRelayCommand;
        public ICommand EditStaffCommand => _editStaffRelayCommand ?? (_editStaffRelayCommand = new RelayCommand<object>(ExecuteEditStaff, CanExecuteEditStaff));

        protected RelayCommand<object> _changeModeRelayCommand;
        public ICommand ChangeModeCommand => _changeModeRelayCommand ?? (_changeModeRelayCommand = new RelayCommand<object>(ExecuteChangeMode, CanExecuteChangeMode));

        public StaffsViewModel()
        {
            CurrentMode = Mode.Edit;
            EmployeeList = EmployeeModel.GetList();

            if (EmployeeList.Count > 0)
                SelectedEmployee = EmployeeList[0];
        }

        public void OnSelectedChange()
        {
            if (inCommand)
                return;

            CurrentEmployee = new Employee(SelectedEmployee);
        }

        private bool CanExecuteAddStaff(object obj)
        {
            return true;
        }

        private void ExecuteAddStaff(object obj)
        {
            // LoginInfo here
            if (EmployeeModel.CheckStaffInfo(CurrentEmployee) || EmployeeModel.CheckStaffID(CurrentEmployee))
            {
                return;
            }

            EmployeeModel.AddStaff(CurrentEmployee, LoginInfo.AccessToken);

            EmployeeList.Add(CurrentEmployee);
        }


        private bool CanExecuteEditStaff(object obj)
        {
            return true;
        }

        private void ExecuteEditStaff(object obj)
        {
            if (!EmployeeModel.CheckStaffID(CurrentEmployee))
                return;

            EmployeeModel.UpdateStaff(CurrentEmployee, LoginInfo.AccessToken);

            inCommand = true;
            var found = EmployeeList.FirstOrDefault(x => x.ID == CurrentEmployee.ID);
            EmployeeList[EmployeeList.IndexOf(found)] = CurrentEmployee;
            inCommand = false;
        }


        private bool CanExecuteChangeMode(object obj)
        {
            return true;
        }

        private void ExecuteChangeMode(object obj)
        {
            
            switch (CurrentMode)
            {
                case Mode.Insert:
                    CurrentMode = Mode.Edit;
                    break;
                case Mode.Edit:
                    CurrentMode = Mode.Insert;
                    break;
                default:
                    break;
            }
        }
    }
}
