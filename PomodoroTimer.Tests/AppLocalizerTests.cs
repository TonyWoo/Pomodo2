using System.Globalization;
using PomodoroTimer.Localization;
using Xunit;

namespace PomodoroTimer.Tests;

public sealed class AppLocalizerTests
{
    [Theory]
    [InlineData("zh-CN", AppLanguage.SimplifiedChinese)]
    [InlineData("zh-Hans", AppLanguage.SimplifiedChinese)]
    [InlineData("zh-TW", AppLanguage.TraditionalChinese)]
    [InlineData("zh-Hant", AppLanguage.TraditionalChinese)]
    [InlineData("zh-HK", AppLanguage.TraditionalChinese)]
    [InlineData("en-US", AppLanguage.English)]
    [InlineData("fr-FR", AppLanguage.English)]
    public void ResolvesSupportedLanguageFromCulture(string cultureName, AppLanguage expectedLanguage)
    {
        var language = AppLocalizer.ResolveSupportedLanguage(new CultureInfo(cultureName));

        Assert.Equal(expectedLanguage, language);
    }

    [Fact]
    public void UsesPersistedLanguagePreferenceWhenAvailable()
    {
        var store = new InMemoryLanguagePreferenceStore("zh-Hant");
        var localizer = new AppLocalizer(store, new CultureInfo("en-US"));

        Assert.Equal(AppLanguage.TraditionalChinese, localizer.CurrentLanguage);
        Assert.Equal("計時", localizer.GetText(LocalizedText.NavTimer));
    }

    [Fact]
    public void SettingLanguagePersistsManualOverride()
    {
        var store = new InMemoryLanguagePreferenceStore();
        var localizer = new AppLocalizer(store, new CultureInfo("fr-FR"));

        localizer.SetLanguage(AppLanguage.SimplifiedChinese);

        Assert.Equal("zh-Hans", store.LoadLanguageCode());
        Assert.Equal("计时", localizer.GetText(LocalizedText.NavTimer));
    }

    [Fact]
    public void EnglishCatalogContainsRequiredTimerCopy()
    {
        var localizer = new AppLocalizer("en");

        Assert.Equal("Timer", localizer.GetText(LocalizedText.NavTimer));
        Assert.Equal("Untitled Task", localizer.GetText(LocalizedText.TaskUntitled));
        Assert.Equal("Start", localizer.GetText(LocalizedText.TimerStart));
    }
}
