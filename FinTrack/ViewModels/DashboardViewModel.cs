﻿using CommunityToolkit.Mvvm.ComponentModel;
using FinTrack.Enums;
using FinTrack.Models.Dashboard;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;

namespace FinTrack.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<BudgetDashboard> _budgets_DashboardView_ItemsControl;

        [ObservableProperty]
        private ObservableCollection<CurrencyRateDashboard> _currencyRates_DashboardView_ItemsControl;

        [ObservableProperty]
        private ObservableCollection<AccountDashboard> _accounts_DashboardView_ItemsControl;

        [ObservableProperty]
        private string _totalBalance_DashboardView_TextBlock;

        [ObservableProperty]
        private ObservableCollection<TransactionDashboard> _transactions_DashboardView_ListView;

        [ObservableProperty]
        private MembershipDashboard _currentMembership_DashboardView_Multiple;

        [ObservableProperty]
        private DebtDashboard _currentDebt_DashboardView_Multiple;

        [ObservableProperty]
        private ObservableCollection<ReportDashboardModel> _reports_DashboardView_ItemsControl;

        [ObservableProperty]
        private string transactionSummary = string.Empty;

        private readonly ILogger<DashboardViewModel> _logger;

        private readonly IServiceProvider _serviceProvider;

        public DashboardViewModel(ILogger<DashboardViewModel> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            LoadData();
        }

        private void LoadData()
        {
            // [TEST]
            Budgets_DashboardView_ItemsControl = new ObservableCollection<BudgetDashboard>
            {
                new BudgetDashboard { Name = "Tasarruf", DueDate = "15.02.2025", Amount = "6.000$", RemainingTime = "15 gün kaldı", StatusBrush = (Brush)Application.Current.FindResource("StatusGreenBrush") },
                new BudgetDashboard { Name = "Harcama", DueDate = "20.02.2025", Amount = "2.000$", RemainingTime = "10 gün kaldı", StatusBrush = (Brush)Application.Current.FindResource("StatusRedBrush") },
                new BudgetDashboard { Name = "Yatırım", DueDate = "10.03.2025", Amount = "10.000$", RemainingTime = "30 gün kaldı", StatusBrush = (Brush)Application.Current.FindResource("StatusGreenBrush") },
                new BudgetDashboard { Name = "Eğlence", DueDate = "05.04.2025", Amount = "8.000$", RemainingTime = "20 gün kaldı", StatusBrush = (Brush)Application.Current.FindResource("StatusGreenBrush") }
            };

            // [TEST]
            CurrencyRates_DashboardView_ItemsControl = new ObservableCollection<CurrencyRateDashboard>
            {
                new CurrencyRateDashboard
                {
                    FromCurrencyFlagUrl = "https://flagcdn.com/w320/us.png", FromCurrencyCountry = "Amerika Birleşik Devletleri", FromCurrencyName = "Dolar", FromCurrencyAmount = "1.00",
                    ToCurrencyFlagUrl = "https://flagcdn.com/w320/tr.png", ToCurrencyCountry = "Türkiye", ToCurrencyName = "Türk Lirası", ToCurrencyAmount = "27.50", ToCurrencyImageHeight = 20
                },
                new CurrencyRateDashboard
                {
                    FromCurrencyFlagUrl = "https://flagcdn.com/w320/eu.png", FromCurrencyCountry = "Euro Bölgesi", FromCurrencyName = "Euro",FromCurrencyAmount = "1.00",
                    ToCurrencyFlagUrl = "https://flagcdn.com/w320/tr.png", ToCurrencyCountry = "Türkiye", ToCurrencyName = "Türk Lirası", ToCurrencyAmount = "30.00", ToCurrencyImageHeight = 20
                }
            };

            // [TEST]
            Accounts_DashboardView_ItemsControl = new ObservableCollection<AccountDashboard>
            {
                new AccountDashboard { Name = "Nakit", Percentage = 50, Balance = "5.000$", ProgressBarBrush = (Brush)Application.Current.FindResource("StatusGreenBrush") },
                new AccountDashboard { Name = "Banka", Percentage = 30, Balance = "3.000$", ProgressBarBrush = (Brush)Application.Current.FindResource("StatusGreenBrush") }
            };

            TotalBalance_DashboardView_TextBlock = "7.000$";

            // [TEST]
            Transactions_DashboardView_ListView = new ObservableCollection<TransactionDashboard>
            {
                new TransactionDashboard { DateText = "01.01.2025", Description = "Market Alışverişi", Amount = "-150$", Category = "Gıda", Type = Enums.TransactionType.Expense },
                new TransactionDashboard { DateText = "02.01.2025", Description = "Maaş", Amount = "+3.000$", Category = "Gelir", Type = Enums.TransactionType.Income },
                new TransactionDashboard { DateText = "03.01.2025", Description = "Elektrik Faturası", Amount = "-200$", Category = "Fatura", Type = Enums.TransactionType.Expense }
            };

            // [TEST]
            double totalIncome = Transactions_DashboardView_ListView
                .Where(t => t.Type == TransactionType.Income)
                .Sum(t =>
                {
                    var cleaned = t.Amount.Replace("+", string.Empty).Replace("$", string.Empty).Trim();
                    return double.TryParse(cleaned, out var value) ? value : 0;
                });
            double totalExpense = Transactions_DashboardView_ListView
                .Where(t => t.Type == TransactionType.Expense)
                .Sum(t =>
                {
                    var cleaned = t.Amount.Replace("-", string.Empty).Replace("$", string.Empty).Trim();
                    return double.TryParse(cleaned, out var value) ? value : 0;
                });
            double remainingBalance = totalIncome - totalExpense;
            TransactionSummary = $"Toplam {Transactions_DashboardView_ListView.Count} işlem bulundu. Gelir: +{totalIncome}$, Gider: -{totalExpense}$ Kalan: {remainingBalance}";

            // [TEST]
            CurrentMembership_DashboardView_Multiple = new MembershipDashboard { Level = "Pro | AKTF", StartDate = "01.01.2025", RenewalDate = "01.02.2025", Price = "9.99$" };

            // [TEST]
            CurrentDebt_DashboardView_Multiple = new DebtDashboard
            {
                LenderName = "Ali Veli",
                LenderIconPath = "https://i.pinimg.com/236x/be/a3/49/bea3491915571d34a026753f4a872000.jpg",
                BorrowerName = "Ahmet Mehmet",
                BorrowerIconPath = "https://pbs.twimg.com/profile_images/1144861916734451712/D76C3ugh_400x400.jpg",
                Status = "Ödenmemiş",
                StatusBrush = (Brush)Application.Current.FindResource("StatusGreenBrush"),
                Amount = "1.000$",
                CreationDate = "01.01.2025",
                DueDate = "01.02.2025",
                ReviewDate = "01.03.2025"
            };

            // [TEST]
            Reports_DashboardView_ItemsControl = new ObservableCollection<ReportDashboardModel>
            {
                CreateReport("2025 Yılı Finansal Raporu"),
                CreateReport("2024 Yılı Tasarruf Raporu"),
                CreateReport("2023 Yılı Gelir-Gider Raporu"),
                CreateReport("2022 Yılı Bütçe Raporu")
            };
        }

        private ReportDashboardModel CreateReport(string name)
        {
            var reportLogger = _serviceProvider.GetRequiredService<ILogger<ReportDashboardModel>>();
            var report = new ReportDashboardModel(reportLogger)
            {
                Name = name,
            };
            return report;
        }
    }
}
