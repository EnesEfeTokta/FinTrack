﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinTrackForWindows.Core;
using FinTrackForWindows.Services;
using Microsoft.Extensions.Logging;
using System.Windows;

namespace FinTrackForWindows.ViewModels
{
    public partial class OtpVerificationViewModel : ObservableObject
    {
        [ObservableProperty]
        private string? verificationCode_OtpVerificationView_TextBox;

        [ObservableProperty]
        private string? counterText_OtpVerificationView_TextBlock;

        public event Action? NavigateToLoginRequested;

        public event Action? NavigateToRegisterRequested;

        private int _counter;
        private Task? _counterTask;

        private readonly IAuthService _authService;
        private readonly ILogger<OtpVerificationViewModel> _logger;

        public OtpVerificationViewModel(IAuthService authService, ILogger<OtpVerificationViewModel> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        public void StartCounter()
        {
            _counter = 300;
            CounterText_OtpVerificationView_TextBlock = $"{_counter / 60}:{_counter % 60:D2}";
            Task.Run(async () =>
            {
                while (_counter > 0)
                {
                    await Task.Delay(1000);
                    _counter--;
                    CounterText_OtpVerificationView_TextBlock = $"{_counter / 60}:{_counter % 60:D2}";
                }
                CounterText_OtpVerificationView_TextBlock = "[X]";
            });
        }

        [RelayCommand]
        private async Task VerifyOtpCode_OtpVerificationView_Button()
        {
            if (string.IsNullOrWhiteSpace(VerificationCode_OtpVerificationView_TextBox) || VerificationCode_OtpVerificationView_TextBox.Length != 6)
            {
                MessageBox.Show("Lütfen geçerli bir OTP kodu girin (6 haneli).", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                _logger.LogWarning("Geçersiz OTP kodu girişi. Kullanıcıdan tekrar denemesi istendi.");
                return;
            }

            bool isVerify = await _authService.VerifyOtpAndRegisterCodeAsync(NewUserInformationManager.Email ?? null!, VerificationCode_OtpVerificationView_TextBox ?? null!);
            if (!isVerify)
            {
                MessageBox.Show("OTP doğrulama başarısız oldu. Lütfen kodu kontrol edin ve tekrar deneyin.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                _logger.LogError("OTP doğrulama başarısız. E-posta: {Email}, Kod: {Code}", NewUserInformationManager.Email, VerificationCode_OtpVerificationView_TextBox);
                return;
            }
            MessageBox.Show("OTP doğrulama başarılı! Hoş geldiniz!", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
            _logger.LogInformation("OTP doğrulama başarılı. Kullanıcı kaydı tamamlandı. E-posta: {Email}", NewUserInformationManager.Email);

            NewUserInformationManager.FullName = null; // Clear the stored user information
            NewUserInformationManager.Email = null; // Clear the stored user information
            NewUserInformationManager.Password = null; // Clear the stored user information

            NavigateToLoginRequested?.Invoke();
        }

        [RelayCommand]
        private async Task CodeNotFound_OtpVerificationView_Button()
        {
            if (string.IsNullOrEmpty(NewUserInformationManager.FirstName) ||
                string.IsNullOrEmpty(NewUserInformationManager.LastName) ||
                string.IsNullOrEmpty(NewUserInformationManager.Email) ||
                string.IsNullOrEmpty(NewUserInformationManager.Password))
            {
                bool isInitiateRegistration = await _authService.InitiateRegistrationAsnc(
                    NewUserInformationManager.FirstName,
                    NewUserInformationManager.LastName,
                    NewUserInformationManager.Email,
                    NewUserInformationManager.Password);

                if (!isInitiateRegistration)
                {
                    MessageBox.Show("Kayıt işlemi başarısız oldu. Lütfen daha sonra tekrar deneyin.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                    _logger.LogError("Kayıt işlemi başarısız. E-posta: {Email}", NewUserInformationManager.Email);
                    return;
                }
                MessageBox.Show("Doğrulama kodu tekrar gönderildi.", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
                _logger.LogInformation("Doğrulama kodu tekrar gönderildi. E-posta: {Email}", NewUserInformationManager.Email);
            }
            else
            {
                MessageBox.Show("Kod gönderirken bir sorun oluştu. Bilgileri doğru giriniz.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                _logger.LogWarning("Kod gönderme sırasında eksik kullanıcı bilgileri. Kullanıcı kayıt sayfasına yönlendiriliyor.");
                NavigateToRegisterRequested?.Invoke();
            }
        }

        [RelayCommand]
        private void NavigateToRegister_OtpVerificationView_Button()
        {
            NavigateToRegisterRequested?.Invoke();
            _logger.LogInformation("Kullanıcı giriş sayfasına yönlendirildi.");
        }
    }
}
