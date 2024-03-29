﻿using DAL.Others;
using DAL.Repos;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Rows
{
    public class PenaltyType : Row
    {
        public string TenViPham { get; set; }
        public float TruLuongTheoPhanTram { get; set; }
        public float TruLuongTrucTiep { get; set; }

        public PenaltyType() { }
        public PenaltyType(PenaltyType penaltyType)
        {
            TenViPham = penaltyType.TenViPham;
            TruLuongTheoPhanTram = penaltyType.TruLuongTheoPhanTram;
            TruLuongTrucTiep = penaltyType.TruLuongTrucTiep;
        }

        public override string Add(UnitOfWork uow = null)
        {
            if (uow == null)
            {
                using (var uowNew = new UnitOfWork())
                {
                    PenaltyTypeRepo.Instance.Add(new PenaltyType(this), uowNew);

                    PenaltyType penalty = PenaltyTypeRepo.Instance.FindByID(new object[] { TenViPham });
                    if (penalty != null)
                    {
                        return "There is already a penalty of the same name!";
                    }

                    return ExecuteAndReturn(uowNew);
                }
            }

            if (PenaltyTypeRepo.Instance.Add(new PenaltyType(this), uow))
                return "";
            else
                return "Failed!";
        }

        public override string Save( UnitOfWork uow = null)
        {
            if (uow == null)
            {
                using (var uowNew = new UnitOfWork())
                {
                    PenaltyTypeRepo.Instance.Update(new object[] { TenViPham }, new PenaltyType(this), uowNew);
                    return ExecuteAndReturn(uowNew);
                }
            }

            if (PenaltyTypeRepo.Instance.Update(new object[] { TenViPham }, new PenaltyType(this), uow))
                return "";
            else
                return "Failed!";
        }

        public override string CheckForError()
        {
            if (string.IsNullOrEmpty(TenViPham))
            {
                return "Penalty name can't be empty!";
            }

            return "";
        }
    }
}
