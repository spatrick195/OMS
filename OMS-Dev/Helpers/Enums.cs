using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OMS_Dev.Helpers
{
    public class Enums
    {
        public enum Province
        {
            [Display(Name = "Northland")]
            NTL,

            [Display(Name = "Auckland")]
            AUK,

            [Display(Name = "Waikato")]
            WKO,

            [Display(Name = "Bay of Plenty")]
            BOP,

            [Display(Name = "Gisborne")]
            GIS,

            [Display(Name = "Hawke's Bay")]
            HKB,

            [Display(Name = "Taranaki")]
            TKI,

            [Display(Name = "Manawatū-Whanganui")]
            MWT,

            [Display(Name = "Wellington")]
            WGN,

            [Display(Name = "Tasman")]
            TAS,

            [Display(Name = "Nelson")]
            NSN,

            [Display(Name = "Marlborough")]
            MBH,

            [Display(Name = "West Coast")]
            WTC,

            [Display(Name = "Canterbury")]
            CAN,

            [Display(Name = "Otago")]
            OTA,

            [Display(Name = "Southland")]
            STL
        }

        public enum Country
        {
            NZ, AU
        }

        public enum Month
        {
            [Display(Name = "January")]
            January = 01,

            [Display(Name = "February")]
            February = 02,

            [Display(Name = "March")]
            March = 03,

            [Display(Name = "April")]
            April = 04,

            [Display(Name = "May")]
            May = 05,

            [Display(Name = "June")]
            June = 06,

            [Display(Name = "July")]
            July = 07,

            [Display(Name = "August")]
            August = 08,

            [Display(Name = "September")]
            September = 09,

            [Display(Name = "October")]
            October = 10,

            [Display(Name = "November")]
            November = 11,

            [Display(Name = "December")]
            December = 12
        }

        public enum SubscriptionStatus
        {
            NotInitialized = 0,
            TrialWithoutCard = 1,
            Active = 2,
            TrialExpired = -1,
            Suspended = -2,
            Canceled = -3
        }

        public enum IntervalCounts
        {
            [Display(Name = "1 Cycle")]
            One = 1,

            [Display(Name = "2 Cycles")]
            Two = 2,

            [Display(Name = "3 Cycles")]
            Three = 3,

            [Display(Name = "4 Cycles")]
            Four = 4,

            [Display(Name = "5 Cycles")]
            Five = 5,

            [Display(Name = "6 Cycles")]
            Six = 6,

            [Display(Name = "7 Cycles")]
            Seven = 7,

            [Display(Name = "8 Cycles")]
            Eight = 8,

            [Display(Name = "9 Cycles")]
            Nine = 9,

            [Display(Name = "10 Cycles")]
            Ten = 10,

            [Display(Name = "11 Cycles")]
            Eleven = 11,

            [Display(Name = "12 Cycles")]
            Twelve = 12
        }

        public static class EnumHelper
        {
            public static string GetProvinceName(Province province)
            {
                var description = Enum.GetName(typeof(Province), province);
                return description;
            }

            public static string GetMonthName(Month month)
            {
                var desc = Enum.GetName(typeof(Month), month);
                return desc;
            }
        }
    }
}