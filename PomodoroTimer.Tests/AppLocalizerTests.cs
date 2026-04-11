using System.Globalization;
using PomodoroTimer.Localization;
using Xunit;

namespace PomodoroTimer.Tests;

public sealed class AppLocalizerTests
{
    [Theory]
    [InlineData("zh-CN", AppLanguage.SimplifiedChinese)]
    [InlineData("zh-TW", AppLanguage.TraditionalChinese)]
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
        var store = new InMemoryLanguagePreferenceStore("zh-TW");
        var localizer = new AppLocalizer(store, new CultureInfo("en-US"));

        Assert.Equal(AppLanguage.TraditionalChinese, localizer.CurrentLanguage);
        Assert.Equal("番茄鐘", localizer.GetText(LocalizedText.AppTitle));
    }

    [Fact]
    public void SettingLanguagePersistsManualOverride()
    {
        var store = new InMemoryLanguagePreferenceStore();
        var localizer = new AppLocalizer(store, new CultureInfo("fr-FR"));

        localizer.SetLanguage(AppLanguage.SimplifiedChinese);

        Assert.Equal("zh-CN", store.LoadLanguageCode());
        Assert.Equal("番茄钟", localizer.GetText(LocalizedText.AppTitle));
    }
}
